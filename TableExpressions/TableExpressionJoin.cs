using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;


namespace Pulse.TableExpressions
{

    /// <summary>
    /// Represents a join algorithm
    /// </summary>
    public enum JoinAlgorithm
    {
        NestedLoop,
        QuasiNestedLoop,
        SortMerge
    }

    /// <summary>
    /// Represents a join type
    /// </summary>
    public enum JoinType
    {
        INNER,
        LEFT,
        ANTI_LEFT
    }

    /// <summary>
    /// Joins two tables together
    /// </summary>
    public abstract class TableExpressionJoin : TableExpression
    {

        protected ScalarExpressionCollection _Fields;
        protected RecordMatcher _Predicate;
        protected Filter _Where;
        protected JoinType _Type;
        protected int _LRecordRef = -1;
        protected int _RRecordRef = -1;

        public TableExpressionJoin(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, RecordMatcher Predicate, Filter Where, 
            JoinType Type)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Predicate = Predicate;
            this._Where = Where;
            this._Type = Type;
            this.Alias = "JOIN";
        }

        /// <summary>
        /// Gets or sets the left table record pointer
        /// </summary>
        public int LeftRecordRef
        {
            get { return this._LRecordRef; }
            set { this._LRecordRef = value; }
        }

        /// <summary>
        /// Gets or sets the right table record pointer
        /// </summary>
        public int RightRecordRef
        {
            get { return this._RRecordRef; }
            set { this._RRecordRef = value; }
        }

        /// <summary>
        /// Gets the underlying columns
        /// </summary>
        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        /// <summary>
        /// Gets the estimated record count
        /// </summary>
        public override long EstimatedCount
        {
            get
            {
                return this._Children.Max((x) => { return x.EstimatedCount; });
            }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {

            FieldResolver x = Variants.CloneOfMeFull();
            x.AddSchema(this._Children[0].Alias, this._Children[0].Columns, out this._LRecordRef);
            x.AddSchema(this._Children[1].Alias, this._Children[1].Columns, out this._RRecordRef);
            return x;

        }

        /// <summary>
        /// Checks if the expression can render
        /// </summary>
        protected void CheckRender()
        {

            if (this._LRecordRef == -1)
                throw new Exception("The left record pointer is invalid");
            if (this._RRecordRef == -1)
                throw new Exception("The right record pointer is invalid");
            if (this._LRecordRef == this._RRecordRef)
                throw new Exception("The left and right record pointers cannot be identical");
            if (this._Children.Count == 0)
                throw new Exception("Missing the left table expression");
            if (this._Children.Count == 1)
                throw new Exception("Missing the right table expression");
            if (this._Children.Count != 2)
                throw new Exception("Cannot process more than two table expressions");

        }

        // Optimizer //
        /// <summary>
        /// Calculates the optiminal join
        /// </summary>
        /// <param name="LCount"></param>
        /// <param name="RCount"></param>
        /// <returns></returns>
        public static JoinAlgorithm Optimize(long LCount, long RCount, bool LIndex, bool RIndex)
        {

            double nl = Util.CostCalculator.NestedLoopJoinCost(LCount, RCount);
            double qnl = Util.CostCalculator.QuasiNestedLoopJoinCost(LCount, RCount, LIndex);
            double sm = Util.CostCalculator.SortMergeNestedLoopJoinCost(LCount, RCount, LIndex, RIndex);

            if (sm < nl && sm < qnl)
                return JoinAlgorithm.SortMerge;
            else if (qnl < nl)
                return JoinAlgorithm.QuasiNestedLoop;

            return JoinAlgorithm.NestedLoop;

        }

    }

    /// <summary>
    /// Creates a table expression implenting the sort merge algorithm
    /// </summary>
    public sealed class TableExpressionJoinSortMerge : TableExpressionJoin
    {

        public TableExpressionJoinSortMerge(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, RecordMatcher Predicate, Filter Where, 
            JoinType Type)
            : base(Host, Parent, Fields, Predicate, Where, Type)
        {

        }

        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {

            // Check each table //
            this.CheckRender();

            // Render each table //
            Table Left = this._Children[0].Evaluate(Variants);
            Table Right = this._Children[1].Evaluate(Variants);

            // Create a resolver //
            FieldResolver pointer = this.CreateResolver(Variants);

            // Get the left and right join index //
            Index lidx = Left.GetIndex(this._Predicate.LeftKey);
            if (lidx == null)
                lidx = Index.BuildTemporaryIndex(Left, this._Predicate.LeftKey);
            Index ridx = Right.GetIndex(this._Predicate.RightKey);
            if (ridx == null)
                ridx = Index.BuildTemporaryIndex(Right, this._Predicate.RightKey);

            // Get the join tags //
            bool Intersection = (this._Type == JoinType.INNER || this._Type == JoinType.LEFT);
            bool Antisection = (this._Type == JoinType.LEFT || this._Type == JoinType.ANTI_LEFT);

            // Open a read stream //
            RecordReader lstream = lidx.OpenReader();
            RecordReader rstream = ridx.OpenReader();
            
            // Main loop through both left and right
            while (lstream.CanAdvance && rstream.CanAdvance)
            {

                int Compare = this._Predicate.Compare(lstream.Read(), rstream.Read());

                // Left is less than right, control left
                if (Compare < 0)
                {
                    lstream.Advance();
                }
                // Right is less than left, control right, but also output an anti join record
                else if (Compare > 0)
                {

                    if (Antisection)
                    {
                        pointer.SetValue(this._LRecordRef, lstream.Read());
                        pointer.SetValue(this._RRecordRef, rstream.Columns.NullRecord);
                        if (this._Where.Evaluate(pointer))
                        {
                            Writer.Insert(this._Fields.Evaluate(pointer));
                        }
                    }
                    rstream.Advance();

                }
                else if (Intersection) // Compare == 0
                {

                    // Save the loop-result //
                    int NestedLoopCount = 0;

                    // Loop through all possible tuples //
                    while (Compare == 0)
                    {

                        // Render the record and potentially output //
                        pointer.SetValue(this._LRecordRef, lstream.Read());
                        pointer.SetValue(this._RRecordRef, rstream.Read());

                        if (this._Where.Evaluate(pointer))
                        {
                            Writer.Insert(this._Fields.Evaluate(pointer));
                        }

                        // Advance the right table //
                        rstream.Advance();
                        NestedLoopCount++;

                        // Check if this advancing pushed us to the end of the table //
                        if (!rstream.CanAdvance)
                            break;

                        // Reset the compare token //
                        Compare = this._Predicate.Compare(lstream.Read(), rstream.Read());

                    }

                    // Revert the nested loops //
                    rstream.Revert(NestedLoopCount);

                    // Step the left stream //
                    lstream.Advance();

                }
                else
                {
                    lstream.Advance();
                }

            }

            // Do Anti-Join //
            if (Antisection)
            {

                // Assign the right table to null //
                pointer.SetValue(this._RRecordRef, rstream.Columns.NullRecord);

                // Walk the rest of the left table //
                while (lstream.CanAdvance)
                {

                    pointer.SetValue(this._LRecordRef, lstream.ReadNext());
                    if (this._Where.Evaluate(pointer))
                    {
                        Writer.Insert(this._Fields.Evaluate(pointer));
                    }

                }

            }

            // Clean up //
            if (this._Host.IsSystemTemp(Left)) 
                this._Host.Store.DropTable(Left.Key);
            if (this._Host.IsSystemTemp(Right)) 
                this._Host.Store.DropTable(Right.Key);

        }

    }

    /// <summary>
    /// Implents the quasi-nested loop algorithm, where the right table uses an index
    /// </summary>
    public sealed class TableExpressionJoinQuasiNestedLoop : TableExpressionJoin
    {

        public TableExpressionJoinQuasiNestedLoop(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, RecordMatcher Predicate, Filter Where, 
            JoinType Type)
            : base(Host, Parent, Fields, Predicate, Where, Type)
        {

        }

        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {

            // Check each table //
            this.CheckRender();

            // Render each table //
            Table Left = this._Children[0].Evaluate(Variants);
            Table Right = this._Children[1].Evaluate(Variants);

            // Create a resolver //
            FieldResolver pointer = this.CreateResolver(Variants);

            // Get the right join index //
            Index ridx = Right.GetIndex(this._Predicate.RightKey);
            if (ridx == null)
                ridx = Index.BuildTemporaryIndex(Right, this._Predicate.RightKey);

            // Get the join tags //
            bool Intersection = (this._Type == JoinType.INNER || this._Type == JoinType.LEFT), Antisection = (this._Type == JoinType.LEFT || this._Type == JoinType.ANTI_LEFT);

            // Open a read stream //
            RecordReader lstream = Left.OpenReader();

            // Loop through //
            while (lstream.CanAdvance)
            {

                // Open the right stream //
                Record lrec = lstream.ReadNext();
                RecordReader rstream = ridx.OpenStrictReader(Record.Split(lrec, this._Predicate.RightKey));
                
                // Only Loop through if there's actually a match //
                if (rstream != null)
                {

                    // Loop through the right stream //
                    while (rstream.CanAdvance)
                    {

                        // Read the records for the predicate //
                        Record rrec = rstream.ReadNext();
                        
                        // Check the predicate //
                        if (this._Predicate.Compare(lrec, rrec) == 0 && Intersection)
                        {

                            // Load the variant //
                            pointer.SetValue(this._LRecordRef, lrec);
                            pointer.SetValue(this._RRecordRef, rrec);

                            // Evaluate the where //
                            if (this._Where.Evaluate(pointer))
                            {
                                Record x = this._Fields.Evaluate(pointer);
                                Writer.Insert(x);
                            }

                        } // Inner Predicate check

                    } // Right While

                } // Inner

                else if (Antisection)
                {

                    // Load the variant //
                    pointer.SetValue(this._LRecordRef, lrec);
                    pointer.SetValue(this._RRecordRef, Right.Columns.NullRecord);

                    // Evaluate the where //
                    if (this._Where.Evaluate(pointer))
                    {
                        Record x = this._Fields.Evaluate(pointer);
                        Writer.Insert(x);
                    }

                }// Anti

            } // Left main loop

            // Clean up //
            if (this._Host.IsSystemTemp(Left))
                this._Host.Store.DropTable(Left.Key);
            if (this._Host.IsSystemTemp(Right))
                this._Host.Store.DropTable(Right.Key);

        }

    }

    /// <summary>
    /// Implements the nested loop join algorithm
    /// </summary>
    public sealed class TableExpressionJoinNestedLoop : TableExpressionJoin
    {

        public TableExpressionJoinNestedLoop(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, RecordMatcher Predicate, Filter Where, 
            JoinType Type)
            : base(Host, Parent, Fields, Predicate, Where, Type)
        {

        }

        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {

            // Check each table //
            this.CheckRender();

            // Render each table //
            Table Left = this._Children[0].Evaluate(Variants);
            Table Right = this._Children[1].Evaluate(Variants);

            // Create a resolver //
            FieldResolver pointer = this.CreateResolver(Variants);

            // Get the join tags //
            bool Intersection = (this._Type == JoinType.INNER || this._Type == JoinType.LEFT), Antisection = (this._Type == JoinType.LEFT || this._Type == JoinType.ANTI_LEFT);

            // Open a read stream //
            RecordReader lstream = Left.OpenReader();

            // Loop through //
            while (lstream.CanAdvance)
            {

                // Open the right stream //
                RecordReader rstream = Right.OpenReader();
                Record lrec = lstream.ReadNext();
                
                // Create the matcher tag //
                bool MatchFound = false;

                // Loop through the right stream //
                while (rstream.CanAdvance)
                {

                    // Read the records for the predicate //
                    Record rrec = rstream.ReadNext();
                    
                    // Check the predicate //
                    if (this._Predicate.Compare(lrec, rrec) == 0 && Intersection)
                    {

                        // Load the variant //
                        pointer.SetValue(this._LRecordRef, lrec);
                        pointer.SetValue(this._RRecordRef, rrec);

                        // Tag taht we found a match //
                        MatchFound = true;

                        // Evaluate the where //
                        if (this._Where.Evaluate(pointer))
                        {
                            Record x = this._Fields.Evaluate(pointer);
                            Writer.Insert(x);
                        }

                    }

                }

                // Handle the fail match //
                if (!MatchFound && Antisection)
                {

                    // Load the variant //
                    pointer.SetValue(this._LRecordRef, lrec);
                    pointer.SetValue(this._RRecordRef, Right.Columns.NullRecord);

                    // Tag taht we found a match //
                    MatchFound = true;

                    // Evaluate the where //
                    if (this._Where.Evaluate(pointer))
                    {
                        Record x = this._Fields.Evaluate(pointer);
                        Writer.Insert(x);
                    }

                }

                // Clean up //
                if (this._Host.IsSystemTemp(Left))
                    this._Host.Store.DropTable(Left.Key);
                if (this._Host.IsSystemTemp(Right))
                    this._Host.Store.DropTable(Right.Key);

            }

        }


    }

}
