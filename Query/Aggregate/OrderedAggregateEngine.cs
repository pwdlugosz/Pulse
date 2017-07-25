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
        public override void Render(Host Host, RecordWriter Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, 
            ScalarExpressionCollection Select, AggregateMetaData MetaData)
        {

            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Create the working record //
            Record r = this.GetWorkRecord(Keys, Values);

            // Create the field resolver //
            FieldResolver variant = new FieldResolver(Data.Host);
            variant.AddSchema(Data.Name, Data.Columns);

            // Open a reader //
            RecordReader reader = Data.OpenReader();

            // Set up the pre-itteration steps //
            variant.SetValue(0, reader.Read());
            Record CurrentKey = Keys.Evaluate(variant);
            Record LastKey = CurrentKey;

            // Create the work data //
            Record WorkData = Values.WorkColumns.NullRecord;
            Values.Initialize(WorkData, 0);

            // Create select resolver //
            FieldResolver q = new FieldResolver(Host);
            q.AddSchema("T", this.GetOutputSchema(Keys, Values));

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
            q.SetValue(0, y);

            // Select //
            y = Select.Evaluate(q);

            // Append it to the stream //
            Output.Insert(y);
            MetaData.WriteCount++;

            // Stop the timer //
            sw.Stop();
            MetaData.RunTime = sw.Elapsed;

        }

    }


}
