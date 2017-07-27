using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Query;
using Pulse.ScalarExpressions;
using Pulse.Aggregates;
using Pulse.Query.Select;
using Pulse.Query.Aggregate;
using Pulse.Query.Join;
using Pulse.Query.Union;

namespace Pulse.TableExpressions
{

    /// <summary>
    /// Provides support for aggregating data
    /// </summary>
    public abstract class TableExpressionFold : TableExpression
    {

        protected ScalarExpressionCollection _Keys;
        protected AggregateCollection _Values;
        protected ScalarExpressionCollection _Select;
        protected Filter _Where;
        protected FieldResolver _Resolver;
        protected int _RecordRef;

        public TableExpressionFold(Host Host, TableExpression Parent, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, 
            ScalarExpressionCollection Select, FieldResolver Variant, int RecordRef)
            : base(Host, Parent)
        {
            this._Keys = Keys;
            this._Values = Values;
            this._Where = Where;
            this._Select = Select;
            this._Resolver = Variant;
            this._RecordRef = RecordRef;
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
        /// Gets the base resolver
        /// </summary>
        public FieldResolver BaseResolver
        {
            get { return this._Resolver; }
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
        public Schema GetOutputSchema(ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            return Schema.Join(Keys.Columns, Values.Columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetWorkSchema(ScalarExpressionCollection Keys, AggregateCollection Values)
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
        public virtual Record GetWorkRecord(ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            int woffset = Keys.Count;
            Record r = this.GetWorkSchema(Keys, Values).NullRecord;
            Values.Initialize(r, woffset);
            return r;
        }

        /// <summary>
        /// Aggregates data using a dictionary
        /// </summary>
        public sealed class TableExpressionFoldDictionary : TableExpressionFold
        {

            public TableExpressionFoldDictionary(Host Host, TableExpression Parent, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where,
                ScalarExpressionCollection Select, FieldResolver Variant, int RecordRef)
                : base(Host, Parent, Keys, Values, Where, Select, Variant, RecordRef)
            {
            }

            public override void Evaluate(RecordWriter Writer)
            {

                Table t = this._Children[0].Evaluate();

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
                    this._Resolver.SetValue(this._RecordRef, reader.ReadNext());

                    // Evaluate the record //
                    Record k = this._Keys.Evaluate(this._Resolver);

                    // Try to get a key //
                    Record v = Storage.GetValue(k);

                    // Check if the dictionary contains this key, then update //
                    if (v != null)
                    {

                        // Accumulate the value //
                        this._Values.Accumulate(this._Resolver, v, 0);

                        // Update the working data //
                        Storage.SetValue(k, v);

                    }
                    else
                    {

                        // Create a new work data record //
                        v = this._Values.WorkColumns.NullRecord;
                        this._Values.Initialize(v, 0);

                        // Accumulate the workd record //
                        this._Values.Accumulate(this._Resolver, v, 0);

                        // Add it to the storage //
                        Storage.Add(k, v);

                    }

                }

                // Now that we're done, we have to walk the entire dictionary //
                // Get the offset //
                int Offset = Storage.KeyFields.Count;

                // Open a reader //
                reader = Storage.OpenReader();
                FieldResolver x = this._Resolver.CloneOfMe();
                x.Reclaim(this._RecordRef, this.GetOutputSchema(this._Keys, this._Values));

                // Itterate over all key-values //
                while (reader.CanAdvance)
                {

                    // Get the work data //
                    Record work = reader.ReadNext();

                    Record k = Record.Split(work, Storage.KeyFields);

                    // Render the final values //
                    Record v = this._Values.Evaluate(work, Offset);

                    // Append the data //
                    x.SetValue(this._RecordRef, Record.Join(k, v));
                    Writer.Insert(this._Select.Evaluate(x));

                }

                // Burn the temp table //
                Host h = Storage.Host;
                h.Store.DropTable(Storage.Key);

            }

        }

        /// <summary>
        /// Aggregates data using an index
        /// </summary>
        public sealed class TableExpressionFoldIndexed : TableExpressionFold
        {

            public TableExpressionFoldIndexed(Host Host, TableExpression Parent, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where,
                ScalarExpressionCollection Select, FieldResolver Variant, int RecordRef)
                : base(Host, Parent, Keys, Values, Where, Select, Variant, RecordRef)
            {
            }

            public override void Evaluate(RecordWriter Writer)
            {

                Table t = this._Children[0].Evaluate();

                // Create the working record //
                Record r = this.GetWorkRecord(this._Keys, this._Values);

                // Open a reader //
                RecordReader reader = t.OpenReader();

                // Set up the pre-itteration steps //
                this._Resolver.SetValue(this._RecordRef, reader.Read());
                Record CurrentKey = this._Keys.Evaluate(this._Resolver);
                Record LastKey = CurrentKey;

                // Create the work data //
                Record WorkData = this._Values.WorkColumns.NullRecord;
                this._Values.Initialize(WorkData, 0);

                // Create select resolver //
                FieldResolver q = this._Resolver.CloneOfMe();
                q.Reclaim(this._RecordRef, this.GetOutputSchema(this._Keys, this._Values));

                // Loop //
                while (reader.CanAdvance)
                {

                    // Prime the resolver //
                    this._Resolver.SetValue(this._RecordRef, reader.ReadNext());

                    // First, check that we satisfy the where statement //
                    if (this._Where.Evaluate(this._Resolver))
                    {

                        // Calculate the key //
                        CurrentKey = this._Keys.Evaluate(this._Resolver);

                        // If the key changes, append the stream //
                        if (Record.Compare(CurrentKey, LastKey) != 0)
                        {

                            // Create the final record //
                            Record x = Record.Join(CurrentKey, this._Values.Evaluate(WorkData, 0));
                            q.SetValue(this._RecordRef, x);
                            x = this._Select.Evaluate(q);

                            // Append it to the stream //
                            Writer.Insert(x);

                            // Rebuild the work record //
                            WorkData = this._Values.WorkColumns.NullRecord;
                            this._Values.Initialize(WorkData, 0);

                        }

                        // Update the work data //
                        this._Values.Accumulate(this._Resolver, WorkData, 0);

                        // Set the lag key //
                        LastKey = CurrentKey;


                    }


                }

                // Append the last working record //
                Record y = Record.Join(CurrentKey, this._Values.Evaluate(WorkData, 0));
                q.SetValue(this._RecordRef, y);

                // Select //
                y = this._Select.Evaluate(q);

                // Append it to the stream //
                Writer.Insert(y);


            }


        }

    }



}
