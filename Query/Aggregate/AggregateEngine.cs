using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Aggregates;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Aggregate
{

    /// <summary>
    /// 
    /// </summary>
    public class AggregateMetaData
    {

        /// <summary>
        /// The actual cost of the grouping
        /// </summary>
        public double ActualCost
        {
            get;
            set;
        }

        /// <summary>
        /// The expected cost of running the query
        /// </summary>
        public double ExpectedCost { get; set; }

        /// <summary>
        /// The time it took to run the grouping
        /// </summary>
        public TimeSpan RunTime { get; set; }

        /// <summary>
        /// The number of records written to the stream passed
        /// </summary>
        public long WriteCount { get; set; }

        /// <summary>
        /// The number of record read from the source table
        /// </summary>
        public long ReadCount { get; set; }

        /// <summary>
        /// The read count for all records passing the filter
        /// </summary>
        public long FilteredReadCount { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class AggregateEngine
    {

        /// <summary>
        /// Groups values by keys
        /// </summary>
        /// <param name="Output">The stream to write the data to</param>
        /// <param name="Data">The source data</param>
        /// <param name="Keys">The expressions that form the unique set in the data</param>
        /// <param name="Values">The aggregate functions over which to consolidate the data</param>
        /// <param name="Where">The filter to apply to the data</param>
        /// <param name="MetaData">The meta data to update</param>
        public abstract void Render(WriteStream Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, AggregateMetaData MetaData);

        /// <summary>
        /// Groups values by keys
        /// </summary>
        /// <param name="Output">The stream to write the data to</param>
        /// <param name="Data">The source data</param>
        /// <param name="Keys">The expressions that form the unique set in the data</param>
        /// <param name="Values">The aggregate functions over which to consolidate the data</param>
        /// <param name="Where">The filter to apply to the data</param>
        /// <param name="MetaData">The meta data to update</param>
        public void Render(WriteStream Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where)
        {
            AggregateMetaData meta = new AggregateMetaData();
            this.Render(Output, Data, Keys, Values, Where, meta);
        }

        /// <summary>
        /// Aggregates data into a clustered table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="Data"></param>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <param name="Where"></param>
        /// <returns></returns>
        public ClusteredScribeTable Aggregate(string DB, string Name, Key ClusterColumns, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where)
        {

            ClusteredScribeTable t = Data.Host.CreateTable(DB, Name, this.GetOutputSchema(Keys, Values), ClusterColumns);
            WriteStream w = t.OpenWriter();
            this.Render(w, Data, Keys, Values, Where);
            w.Close();
            return t;

        }

        /// <summary>
        /// Aggregates data into a clustered table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="Data"></param>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public ClusteredScribeTable Aggregate(string DB, string Name, Key ClusterColumns, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            return this.Aggregate(DB, Name, ClusterColumns, Data, Keys, Values, Filter.TrueForAll);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <param name="Data"></param>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <param name="Where"></param>
        /// <returns></returns>
        public HeapScribeTable Aggregate(string DB, string Name, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where)
        {

            HeapScribeTable t = Data.Host.CreateTable(DB, Name, this.GetOutputSchema(Keys, Values));
            WriteStream w = t.OpenWriter();
            this.Render(w, Data, Keys, Values, Where);
            w.Close();
            return t;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <param name="Data"></param>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public HeapScribeTable Aggregate(string DB, string Name, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            return this.Aggregate(DB, Name, Data, Keys, Values, Filter.TrueForAll);
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

    }

    /// <summary>
    /// Creates an aggregate engine assuming the data is ordered
    /// </summary>
    public class OrderedAggregateEngine : AggregateEngine
    {

        /// <summary>
        /// 
        /// </summary>
        public OrderedAggregateEngine()
            : base()
        {
        }

        /// <summary>
        /// Groups values by keys
        /// </summary>
        /// <param name="Output">The stream to write the data to</param>
        /// <param name="Data">The source data</param>
        /// <param name="Keys">The expressions that form the unique set in the data</param>
        /// <param name="Values">The aggregate functions over which to consolidate the data</param>
        /// <param name="Where">The filter to apply to the data</param>
        /// <param name="MetaData">The meta data to update</param>
        public override void Render(WriteStream Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, AggregateMetaData MetaData)
        {

            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Create the working record //
            Record r = this.GetWorkRecord(Keys, Values);

            // Create the field resolver //
            FieldResolver variant = new FieldResolver(Data.Host);
            variant.AddSchema(Data.Name, Data.Columns);

            // Open a reader //
            ReadStream reader = Data.OpenReader();

            // Set up the pre-itteration steps //
            variant.SetValue(0, reader.Read());
            Record CurrentKey = Keys.Evaluate(variant);
            Record LastKey = CurrentKey;

            // Create the work data //
            Record WorkData = Values.WorkColumns.NullRecord;
            Values.Initialize(WorkData, 0);

            // Loop //
            while (reader.CanAdvance)
            {

                // Prime the resolver //
                variant.SetValue(0, reader.ReadNext());
                MetaData.ReadCount++;
                MetaData.ActualCost++;

                // First, check that we satisfy the where statement //
                if (Where.Evaluate(variant))
                {

                    MetaData.FilteredReadCount++;

                    // Calculate the key //
                    CurrentKey = Keys.Evaluate(variant);

                    // If the key changes, append the stream //
                    if (Record.Compare(CurrentKey, LastKey) != 0)
                    {

                        // Create the final record //
                        Record x = Record.Join(CurrentKey, Values.Evaluate(WorkData, 0));
                        
                        // Append it to the stream //
                        Output.Insert(x);
                        MetaData.WriteCount++;

                        // Rebuild the work record //
                        WorkData = Values.WorkColumns.NullRecord;
                        Values.Initialize(WorkData, 0);

                    }

                    // Update the work data //
                    Values.Accumulate(variant, WorkData, 0);

                    // Set the lag key //
                    LastKey = CurrentKey;


                }


            }

            // Append the last working record //
            Record y = Record.Join(CurrentKey, Values.Evaluate(WorkData, 0));

            // Append it to the stream //
            Output.Insert(y);
            MetaData.WriteCount++;

            // Stop the timer //
            sw.Stop();
            MetaData.RunTime = sw.Elapsed;

        }

    }

    /// <summary>
    /// Represents an engine that aggregates a table using a b+ tree
    /// </summary>
    public class DictionaryAggregateEngine : AggregateEngine
    {


        /// <summary>
        /// 
        /// </summary>
        public DictionaryAggregateEngine()
            : base()
        {
            
        }

        /// <summary>
        /// Groups values by keys
        /// </summary>
        /// <param name="Output">The stream to write the data to</param>
        /// <param name="Data">The source data</param>
        /// <param name="Keys">The expressions that form the unique set in the data</param>
        /// <param name="Values">The aggregate functions over which to consolidate the data</param>
        /// <param name="Where">The filter to apply to the data</param>
        /// <param name="MetaData">The meta data to update</param>
        public override void Render(WriteStream Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, AggregateMetaData MetaData)
        {

            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Create a dictionary table //
            DictionaryScribeTable Storage = Output.Source.Host.CreateTable(Host.TEMP, Host.RandomName, Keys.Columns, Values.Columns);

            // Create the working record //
            Record r = this.GetWorkRecord(Keys, Values);

            // Create the field resolver //
            FieldResolver variant = new FieldResolver(Data.Host);
            variant.AddSchema(Data.Name, Data.Columns);

            // Open a reader //
            ReadStream reader = Data.OpenReader();

            // Scan the entire table //
            while (reader.CanAdvance)
            {

                // Prime the resolver //
                variant.SetValue(0, reader.ReadNext());
                MetaData.ReadCount++;
                MetaData.ActualCost++;

                // Evaluate the record //
                Record k = Keys.Evaluate(variant);

                // Try to get a key //
                Record v = Storage.GetValue(k);

                // Check if the dictionary contains this key, then update //
                if (v != null)
                {

                    // Accumulate the value //
                    Values.Accumulate(variant, v, 0);

                    // Update the working data //
                    Storage.SetValue(k, v);

                }
                else
                {

                    // Create a new work data record //
                    v = Values.WorkColumns.NullRecord;
                    Values.Initialize(v, 0);

                    // Accumulate the workd record //
                    Values.Accumulate(variant, v, 0);

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

                Record k = Record.Split(work, Storage.KeyFields);

                // Render the final values //
                Record v = Values.Evaluate(work, Offset);

                // Append the data //
                Output.Insert(Record.Join(k, v));

            }

            // Burn the temp table //
            Host h = Storage.Host;
            h.PageCache.DropTable(Storage.Key);

        }

    }

}
