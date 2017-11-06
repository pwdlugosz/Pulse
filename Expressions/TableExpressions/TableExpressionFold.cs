using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.TableExpressions
{

    /// <summary>
    /// Provides support for aggregating data
    /// </summary>
    public abstract class TableExpressionFold : TableExpression
    {

        protected RecordExpression _Keys;
        protected AggregateCollection _Values;
        protected RecordExpression _Select;
        protected Filter _Where;
        protected int _RecordRef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Parent"></param>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <param name="Where"></param>
        /// <param name="Select"></param>
        public TableExpressionFold(Host Host, TableExpression Parent, RecordExpression Keys, AggregateCollection Values, Filter Where, 
            RecordExpression Select)
            : base(Host, Parent)
        {
            this._Keys = Keys;
            this._Values = Values;
            this._Where = Where;
            this._Select = Select;
            this.Alias = "FOLD";
        }

        /// <summary>
        /// Gets the underlying columns
        /// </summary>
        public override Schema Columns
        {
            get { return this.GetOutputSchema(this._Keys, this._Values); }
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
        /// Gets the pointer to the position in the field resolver to pull from
        /// </summary>
        public int RecordRef
        {
            get { return this._RecordRef; }
            set { this._RecordRef = value; }
        }

        // Supporting aggregates //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetOutputSchema(RecordExpression Keys, AggregateCollection Values)
        {
            return Schema.Join(Keys.Columns, Values.Columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetWorkSchema(RecordExpression Keys, AggregateCollection Values)
        {
            return Schema.Join(Keys.Columns, Values.WorkColumns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkData"></param>
        /// <param name="Key"></param>
        public virtual void OverLay(Record WorkData, Record Key)
        {
            Array.Copy(Key._data, WorkData._data, Key.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public virtual Record GetWorkRecord(RecordExpression Keys, AggregateCollection Values)
        {
            int woffset = Keys.Count;
            Record r = this.GetWorkSchema(Keys, Values).NullRecord;
            Values.Initialize(r, woffset);
            return r;
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {

            FieldResolver x = Variants.CloneOfMeFull();
            x.AddSchema(this._Children[0].Alias, this._Children[0].Columns, out this._RecordRef);
            return x;

        }

        public FieldResolver CreateSecondaryResolver(FieldResolver Variants, string Alias)
        {
            FieldResolver x = Variants.CloneOfMeFull();
            x.AddSchema(Alias, this.GetOutputSchema(this._Keys, this._Values), out this._RecordRef);
            return x;
        }

        /// <summary>
        /// Expressions.Aggregates data using a dictionary
        /// </summary>
        public sealed class TableExpressionFoldDictionary : TableExpressionFold
        {

            public TableExpressionFoldDictionary(Host Host, TableExpression Parent, RecordExpression Keys, AggregateCollection Values, Filter Where,
                RecordExpression Select)
                : base(Host, Parent, Keys, Values, Where, Select)
            {
            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                // Get the source table //
                Table t = this._Children[0].Select(Variants);

                // Get the resolver we're going to use //
                FieldResolver pointer = this.CreateResolver(Variants);

                // Create a dictionary table //
                DictionaryTable Storage = this._Host.CreateTable(Host.TEMP, Host.RandomName, this._Keys.Columns, this._Values.WorkColumns);

                // Create the working record //
                Record r = this.GetWorkRecord(this._Keys, this._Values);

                // Open a reader //
                RecordReader reader = t.OpenReader();

                // Scan the entire table //
                while (reader.CanAdvance)
                {

                    // Prime the resolver //
                    pointer.SetValue(this._RecordRef, reader.ReadNext());

                    // Evaluate the record //
                    Record k = this._Keys.Evaluate(pointer);

                    // Try to get a key //
                    Record v = Storage.GetValue(k);

                    // Check if the dictionary contains this key, then update //
                    if (v != null)
                    {

                        // Accumulate the value //
                        this._Values.Accumulate(pointer, v, 0);

                        // Update the working data //
                        Storage.SetValue(k, v);

                    }
                    else
                    {

                        // Create a new work data record //
                        v = this._Values.WorkColumns.NullRecord;
                        this._Values.Initialize(v, 0);

                        // Accumulate the workd record //
                        this._Values.Accumulate(pointer, v, 0);

                        // Add it to the storage //
                        Storage.Add(k, v);

                    }

                }

                // Now that we're done, we have to walk the entire dictionary //
                // Get the offset //
                int Offset = Storage.KeyFields.Count;

                // Open a reader //
                reader = Storage.OpenReader();
                FieldResolver pointer2 = this.CreateSecondaryResolver(Variants, "T"); 

                // Itterate over all key-values //
                while (reader.CanAdvance)
                {

                    // Get the work data //
                    Record work = reader.ReadNext();

                    Record k = Record.Split(work, Storage.KeyFields);

                    // Render the final values //
                    Record v = this._Values.Evaluate(work, Offset);

                    // Append the data //
                    pointer2.SetValue(this._RecordRef, Record.Join(k, v));
                    Writer.Insert(this._Select.Evaluate(pointer2));

                }

                // Burn the temp table //
                this._Host.Store.DropTable(Storage.Key);

                // Burn the source table //
                if (this._Host.IsSystemTemp(t))
                    this._Host.Store.DropTable(t.Key);

            }

        }

        /// <summary>
        /// Expressions.Aggregates data using an index
        /// </summary>
        public sealed class TableExpressionFoldIndexed : TableExpressionFold
        {

            public TableExpressionFoldIndexed(Host Host, TableExpression Parent, RecordExpression Keys, AggregateCollection Values, Filter Where,
                RecordExpression Select)
                : base(Host, Parent, Keys, Values, Where, Select)
            {
            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                Table t = this._Children[0].Select(Variants);

                //  Create a resolver //
                FieldResolver pointer = this.CreateResolver(Variants);

                // Create the working record //
                Record r = this.GetWorkRecord(this._Keys, this._Values);

                // Open a reader //
                RecordReader reader = t.OpenReader();

                // Set up the pre-itteration steps //
                pointer.SetValue(this._RecordRef, reader.Read());
                Record CurrentKey = this._Keys.Evaluate(pointer);
                Record LastKey = CurrentKey;

                // Create the work data //
                Record WorkData = this._Values.WorkColumns.NullRecord;
                this._Values.Initialize(WorkData, 0);

                // Create select resolver //
                FieldResolver pointer2 = this.CreateSecondaryResolver(Variants, "T");

                // Loop //
                while (reader.CanAdvance)
                {

                    // Prime the resolver //
                    pointer.SetValue(this._RecordRef, reader.ReadNext());

                    // First, check that we satisfy the where statement //
                    if (this._Where.Evaluate(pointer))
                    {

                        // Calculate the key //
                        CurrentKey = this._Keys.Evaluate(pointer);

                        // If the key changes, append the stream //
                        if (Record.Compare(CurrentKey, LastKey) != 0)
                        {

                            // Create the final record //
                            Record x = Record.Join(CurrentKey, this._Values.Evaluate(WorkData, 0));
                            pointer2.SetValue(this._RecordRef, x);
                            x = this._Select.Evaluate(pointer2);

                            // Append it to the stream //
                            Writer.Insert(x);

                            // Rebuild the work record //
                            WorkData = this._Values.WorkColumns.NullRecord;
                            this._Values.Initialize(WorkData, 0);

                        }

                        // Update the work data //
                        this._Values.Accumulate(pointer, WorkData, 0);

                        // Set the lag key //
                        LastKey = CurrentKey;


                    }


                }

                // Append the last working record //
                Record y = Record.Join(CurrentKey, this._Values.Evaluate(WorkData, 0));
                pointer2.SetValue(this._RecordRef, y);

                // Select //
                y = this._Select.Evaluate(pointer2);

                // Append it to the stream //
                Writer.Insert(y);

                // Clean up //
                if (this._Host.IsSystemTemp(t))
                    this._Host.Store.DropTable(t.Key);

            }

        }

    }



}
