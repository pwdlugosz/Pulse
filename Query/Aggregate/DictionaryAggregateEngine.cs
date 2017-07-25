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
        public override void Render(Host Host, RecordWriter Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, 
            ScalarExpressionCollection Select, AggregateMetaData MetaData)
        {

            
            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Create a dictionary table //
            DictionaryTable Storage = Output.Source.Host.CreateTable(Host.TEMP, Host.RandomName, Keys.Columns, Values.WorkColumns);

            // Create the working record //
            Record r = this.GetWorkRecord(Keys, Values);

            // Create the field resolver //
            FieldResolver variant = new FieldResolver(Data.Host);
            variant.AddSchema(Data.Name, Data.Columns);

            // Open a reader //
            RecordReader reader = Data.OpenReader();

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
            FieldResolver x = new FieldResolver(Host);
            x.AddSchema("T", this.GetOutputSchema(Keys, Values));

            // Itterate over all key-values //
            while (reader.CanAdvance)
            {
                
                // Get the work data //
                Record work = reader.ReadNext();

                Record k = Record.Split(work, Storage.KeyFields);

                // Render the final values //
                Record v = Values.Evaluate(work, Offset);

                // Append the data //
                x.SetValue(0, Record.Join(k, v));
                Output.Insert(Select.Evaluate(x));

            }

            // Burn the temp table //
            Host h = Storage.Host;
            h.Store.DropTable(Storage.Key);

        }

    }


}
