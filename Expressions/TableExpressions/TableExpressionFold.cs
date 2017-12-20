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

        protected ScalarExpressionSet _Keys;
        protected AggregateCollection _Values;
        protected ScalarExpressionSet _Select;
        protected Filter _Where;
        protected int _RecordRef;
        public const string SECOND_ALIAS_PREFIX = "&";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Parent"></param>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <param name="Where"></param>
        /// <param name="Select"></param>
        public TableExpressionFold(Host Host, TableExpression Parent, ScalarExpressionSet Keys, AggregateCollection Values, Filter Where, 
            ScalarExpressionSet Select, string Alias)
            : base(Host, Parent)
        {
            this._Keys = Keys;
            this._Values = Values;
            this._Where = Where;
            this._Select = Select;
            this.Alias = Alias;
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
        /// 
        /// </summary>
        public string SecondaryAlias
        {
            get { return SECOND_ALIAS_PREFIX + this.Alias; }
        }

        // Supporting aggregates //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetOutputSchema(ScalarExpressionSet Keys, AggregateCollection Values)
        {
            return Schema.Join(Keys.Columns, Values.Columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetWorkSchema(ScalarExpressionSet Keys, AggregateCollection Values)
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
        public virtual Record GetWorkRecord(ScalarExpressionSet Keys, AggregateCollection Values)
        {
            int woffset = Keys.Count;
            Record r = this.GetWorkSchema(Keys, Values).NullRecord;
            Values.Initialize(r, woffset);
            return r;
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
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public FieldResolver CreateSecondaryResolver(FieldResolver Variants, string Alias)
        {
            return Variants;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void InitializeResolver(FieldResolver Variants)
        {
            // Fix the resolver //
            if (!Variants.Local.ExistsRecord(this.Alias))
                Variants.Local.DeclareRecord(this.Alias, new AssociativeRecord(this._Children[0].Columns));
            if (!Variants.Local.ExistsRecord(this.SecondaryAlias))
                Variants.Local.DeclareRecord(this.SecondaryAlias, new AssociativeRecord(this.GetOutputSchema(this._Keys, this._Values)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void CleanUpResolver(FieldResolver Variants)
        {
            // Fix the resolver //
            //if (Variants.Local.ExistsRecord(this.Alias)) Variants.Local.RemoveRecord(this.Alias);
            //if (Variants.Local.ExistsRecord(this.SecondaryAlias)) Variants.Local.RemoveRecord(this.SecondaryAlias);
        }

        /// <summary>
        /// Expressions.Aggregates data using a dictionary
        /// </summary>
        public sealed class TableExpressionFoldDictionary : TableExpressionFold
        {

            public TableExpressionFoldDictionary(Host Host, TableExpression Parent, ScalarExpressionSet Keys, AggregateCollection Values, Filter Where,
                ScalarExpressionSet Select, string Alias)
                : base(Host, Parent, Keys, Values, Where, Select, Alias)
            {
            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                // Get the source table //
                Table t = this._Children[0].Select(Variants);

                // Initialize //
                this.InitializeResolver(Variants);

                // Create a dictionary table //
                DictionaryTable Storage = this._Host.CreateTable(Host.TEMP, Host.RandomName, this._Keys.Columns, this._Values.WorkColumns);
                Schema SecondSchema = this.GetOutputSchema(this._Keys, this._Values);

                // Create the working record //
                Record r = this.GetWorkRecord(this._Keys, this._Values);

                // Open a reader //
                RecordReader reader = t.OpenReader();

                // Scan the entire table //
                while (reader.CanAdvance)
                {

                    // Prime the resolver //
                    Variants.Local.SetRecord(this.Alias, new AssociativeRecord(t.Columns, reader.ReadNext()));

                    // Evaluate the record //
                    Record k = this._Keys.Evaluate(Variants);

                    // Try to get a key //
                    Record v = Storage.GetValue(k);

                    // Check if the dictionary contains this key, then update //
                    if (v != null)
                    {

                        // Accumulate the value //
                        this._Values.Accumulate(Variants, v, 0);

                        // Update the working data //
                        Storage.SetValue(k, v);

                    }
                    else
                    {

                        // Create a new work data record //
                        v = this._Values.WorkColumns.NullRecord;
                        this._Values.Initialize(v, 0);

                        // Accumulate the workd record //
                        this._Values.Accumulate(Variants, v, 0);

                        // Add it to the storage //
                        Storage.Add(k, v);

                    }

                }

                // Now that we're done, we have to walk the entire dictionary //
                // Get the offset //
                int Offset = Storage.KeyFields.Count;

                // Open a reader //
                reader = Storage.OpenReader();
                
                // Itterate over all key-values //
                while (reader.CanAdvance)
                {

                    // Get the work data //
                    Record work = reader.ReadNext();

                    Record k = Elements.Record.Split(work, Storage.KeyFields);

                    // Render the final values //
                    Record v = this._Values.Evaluate(work, Offset);

                    // Append the data //
                    Variants.Local.SetRecord(this.SecondaryAlias, new AssociativeRecord(SecondSchema, Elements.Record.Join(k, v)));
                    Writer.Insert(this._Select.Evaluate(Variants));

                }

                // Burn the temp table //
                this._Host.TableStore.DropTable(Storage.Key);

                // Burn the source table //
                if (this._Host.IsSystemTemp(t))
                    this._Host.TableStore.DropTable(t.Key);

                // Clean up //
                this.CleanUpResolver(Variants);

            }

        }

        /// <summary>
        /// Expressions.Aggregates data using an index
        /// </summary>
        public sealed class TableExpressionFoldIndexed : TableExpressionFold
        {

            public TableExpressionFoldIndexed(Host Host, TableExpression Parent, ScalarExpressionSet Keys, AggregateCollection Values, Filter Where,
                ScalarExpressionSet Select, string Alias)
                : base(Host, Parent, Keys, Values, Where, Select, Alias)
            {
            }

            public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
            {

                Table t = this._Children[0].Select(Variants);

                // Initialize resolver //
                this.InitializeResolver(Variants);

                // Create the working record //
                Record r = this.GetWorkRecord(this._Keys, this._Values);

                // Open a reader //
                RecordReader reader = t.OpenReader();

                // Set up the pre-itteration steps //
                Variants.Local.SetRecord(this.Alias, new AssociativeRecord(t.Columns, reader.Read()));
                Record CurrentKey = this._Keys.Evaluate(Variants);
                Record LastKey = CurrentKey;
                Schema s = this.GetOutputSchema(this._Keys, this._Values);

                // Create the work data //
                Record WorkData = this._Values.WorkColumns.NullRecord;
                this._Values.Initialize(WorkData, 0);

                // Loop //
                while (reader.CanAdvance)
                {

                    // Prime the resolver //
                    Variants.Local.SetRecord(this.Alias, new AssociativeRecord(t.Columns, reader.Read()));

                    // First, check that we satisfy the where statement //
                    if (this._Where.Evaluate(Variants))
                    {

                        // Calculate the key //
                        CurrentKey = this._Keys.Evaluate(Variants);

                        // If the key changes, append the stream //
                        if (Elements.Record.Compare(CurrentKey, LastKey) != 0)
                        {

                            // Create the final record //
                            Record x = Elements.Record.Join(CurrentKey, this._Values.Evaluate(WorkData, 0));
                            Variants.Local.SetRecord(this.SecondaryAlias, new AssociativeRecord(s,x));
                            x = this._Select.Evaluate(Variants);

                            // Append it to the stream //
                            Writer.Insert(x);

                            // Rebuild the work record //
                            WorkData = this._Values.WorkColumns.NullRecord;
                            this._Values.Initialize(WorkData, 0);

                        }

                        // Update the work data //
                        this._Values.Accumulate(Variants, WorkData, 0);

                        // Set the lag key //
                        LastKey = CurrentKey;


                    }


                }

                // Append the last working record //
                Record y = Elements.Record.Join(CurrentKey, this._Values.Evaluate(WorkData, 0));
                Variants.Local.SetRecord(this.SecondaryAlias, new AssociativeRecord(s, y));
                            
                // Select //
                y = this._Select.Evaluate(Variants);

                // Append it to the stream //
                Writer.Insert(y);

                // Clean up //
                if (this._Host.IsSystemTemp(t))
                    this._Host.TableStore.DropTable(t.Key);
                this.CleanUpResolver(Variants);

            }

        }

    }



}
