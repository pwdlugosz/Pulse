using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.TableExpressions
{

    /// <summary>
    /// Joins two tables together
    /// </summary>
    public abstract class TableExpressionJoin : TableExpression
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

        protected ScalarExpressionSet _Fields;
        protected RecordMatcher _Predicate;
        protected Filter _Where;
        protected JoinType _Type;
        protected string _LeftAlias = "L";
        protected string _RightAlias = "R";

        public TableExpressionJoin(Host Host, TableExpression Parent, ScalarExpressionSet Fields, RecordMatcher Predicate, Filter Where, 
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
        public string LeftAlias
        {
            get { return this._LeftAlias; }
            set { this._LeftAlias = value; }
        }

        /// <summary>
        /// Gets or sets the right table record pointer
        /// </summary>
        public string RightAlias
        {
            get { return this._RightAlias; }
            set { this._RightAlias = value; }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants;
        }

        /// <summary>
        /// Checks if the expression can render
        /// </summary>
        protected void CheckRender()
        {

            if (this._LeftAlias == this._RightAlias)
                throw new Exception("The left and right record pointers cannot be identical");
            if (this._Children.Count == 0)
                throw new Exception("Missing the left table expression");
            if (this._Children.Count == 1)
                throw new Exception("Missing the right table expression");
            if (this._Children.Count != 2)
                throw new Exception("Cannot process more than two table expressions");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void InitializeResolver(FieldResolver Variants)
        {
            if (!Variants.Local.ExistsRecord(this.LeftAlias))
                Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(this._Children[0].Columns));
            if (!Variants.Local.ExistsRecord(this.RightAlias))
                Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(this._Children[1].Columns));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void CleanUpResolver(FieldResolver Variants)
        {
            if (Variants.Local.ExistsRecord(this.LeftAlias))
                Variants.Local.RemoveRecord(this.LeftAlias);
            if (Variants.Local.ExistsRecord(this.RightAlias))
                Variants.Local.RemoveRecord(this.RightAlias);
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

        /// <summary>
        /// Creates a table expression implenting the sort merge algorithm
        /// </summary>
        public sealed class TableExpressionJoinSortMerge : TableExpressionJoin
        {

            public TableExpressionJoinSortMerge(Host Host, TableExpression Parent, ScalarExpressionSet Fields, RecordMatcher Predicate, Filter Where,
                JoinType Type)
                : base(Host, Parent, Fields, Predicate, Where, Type)
            {

            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                // Check each table //
                this.CheckRender();

                // Render each table //
                Table Left = this._Children[0].Select(Variants);
                Table Right = this._Children[1].Select(Variants);

                // Fix the resolver //
                this.InitializeResolver(Variants);

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
                            Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                            Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns));
                            if (this._Where.Evaluate(Variants))
                            {
                                Writer.Insert(this._Fields.Evaluate(Variants));
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
                            Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                            Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns, rstream.Read()));
                            
                            if (this._Where.Evaluate(Variants))
                            {
                                Writer.Insert(this._Fields.Evaluate(Variants));
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
                    Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns));

                    // Walk the rest of the left table //
                    while (lstream.CanAdvance)
                    {

                        Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                        if (this._Where.Evaluate(Variants))
                        {
                            Writer.Insert(this._Fields.Evaluate(Variants));
                        }

                    }

                }

                // Clean up //
                if (this._Host.IsSystemTemp(Left))
                    this._Host.TableStore.DropTable(Left.Key);
                if (this._Host.IsSystemTemp(Right))
                    this._Host.TableStore.DropTable(Right.Key);

            }

        }

        /// <summary>
        /// Implents the quasi-nested loop algorithm, where the right table uses an index
        /// </summary>
        public sealed class TableExpressionJoinQuasiNestedLoop : TableExpressionJoin
        {

            public TableExpressionJoinQuasiNestedLoop(Host Host, TableExpression Parent, ScalarExpressionSet Fields, RecordMatcher Predicate, Filter Where,
                JoinType Type)
                : base(Host, Parent, Fields, Predicate, Where, Type)
            {

            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                // Check each table //
                this.CheckRender();

                // Render each table //
                Table Left = this._Children[0].Select(Variants);
                Table Right = this._Children[1].Select(Variants);

                // Create a resolver //
                this.InitializeResolver(Variants);

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
                    RecordReader rstream = ridx.OpenStrictReader(Elements.Record.Split(lrec, this._Predicate.RightKey));

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
                                Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                                Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns, rstream.Read()));
                            
                                // Evaluate the where //
                                if (this._Where.Evaluate(Variants))
                                {
                                    Record x = this._Fields.Evaluate(Variants);
                                    Writer.Insert(x);
                                }

                            } // Inner Predicate check

                        } // Right While

                    } // Inner

                    else if (Antisection)
                    {

                        // Load the variant //
                        Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                        Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns));
                            
                        // Evaluate the where //
                        if (this._Where.Evaluate(Variants))
                        {
                            Record x = this._Fields.Evaluate(Variants);
                            Writer.Insert(x);
                        }

                    }// Anti

                } // Left main loop

                // Clean up //
                if (this._Host.IsSystemTemp(Left))
                    this._Host.TableStore.DropTable(Left.Key);
                if (this._Host.IsSystemTemp(Right))
                    this._Host.TableStore.DropTable(Right.Key);

                this.CleanUpResolver(Variants);

            }

        }

        /// <summary>
        /// Implements the nested loop join algorithm
        /// </summary>
        public sealed class TableExpressionJoinNestedLoop : TableExpressionJoin
        {

            public TableExpressionJoinNestedLoop(Host Host, TableExpression Parent, ScalarExpressionSet Fields, RecordMatcher Predicate, Filter Where,
                JoinType Type)
                : base(Host, Parent, Fields, Predicate, Where, Type)
            {

            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                // Check each table //
                this.CheckRender();

                // Render each table //
                Table Left = this._Children[0].Select(Variants);
                Table Right = this._Children[1].Select(Variants);

                // Create a resolver //
                this.InitializeResolver(Variants);

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
                            Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                            Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns, rstream.Read()));
                            
                            // Tag taht we found a match //
                            MatchFound = true;

                            // Evaluate the where //
                            if (this._Where.Evaluate(Variants))
                            {
                                Record x = this._Fields.Evaluate(Variants);
                                Writer.Insert(x);
                            }

                        }

                    }

                    // Handle the fail match //
                    if (!MatchFound && Antisection)
                    {

                        // Load the variant //
                        Variants.Local.SetRecord(this.LeftAlias, new AssociativeRecord(lstream.Columns, lstream.Read()));
                        Variants.Local.SetRecord(this.RightAlias, new AssociativeRecord(rstream.Columns));
                            
                        // Tag taht we found a match //
                        MatchFound = true;

                        // Evaluate the where //
                        if (this._Where.Evaluate(Variants))
                        {
                            Record x = this._Fields.Evaluate(Variants);
                            Writer.Insert(x);
                        }

                    }

                    // Clean up //
                    if (this._Host.IsSystemTemp(Left))
                        this._Host.TableStore.DropTable(Left.Key);
                    if (this._Host.IsSystemTemp(Right))
                        this._Host.TableStore.DropTable(Right.Key);

                }

            }

        }


    }

}
