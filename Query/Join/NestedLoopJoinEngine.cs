using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Util;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Join
{

    public sealed class NestedLoopJoinEngine : JoinEngine
    {

        /// <summary>
        /// Creates a nested loop engine
        /// </summary>
        public NestedLoopJoinEngine()
            : base()
        {
        }

        /// <summary>
        /// Renders the join over two tables
        /// </summary>
        /// <param name="Left">The left table</param>
        /// <param name="Right">The right table</param>
        /// <param name="Predicate">The join predicate</param>
        /// <param name="Fields">The fields to keep</param>
        /// <param name="Where">The filter to apply</param>
        /// <param name="Type">The type of join to perform</param>
        /// <param name="ActualCost">Output of the actual cost of running this join</param>
        public override void Render(WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type, JoinMetaData MetaData)
        {

            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Get the expected join cost //
            MetaData.ExpectedJoinCost = CostCalculator.NestedLoopJoinCost(Left.RecordCount, Right.RecordCount);

            // Get the join tags //
            bool Intersection = (Type == JoinType.INNER || Type == JoinType.LEFT), Antisection = (Type == JoinType.LEFT || Type == JoinType.ANTI_LEFT);

            // Open a read stream //
            ReadStream lstream = Left.OpenReader();

            // Create a FieldResolver //
            FieldResolver variant = new FieldResolver(Left.Host);
            variant.AddSchema("L", Left.Columns);
            variant.AddSchema("R", Right.Columns);

            // Loop through //
            while (lstream.CanAdvance)
            {

                // Open the right stream //
                ReadStream rstream = Right.OpenReader();
                Record lrec = lstream.ReadNext();
                MetaData.LeftReadCount++;

                // Create the matcher tag //
                bool MatchFound = false;

                // Loop through the right stream //
                while (rstream.CanAdvance)
                {

                    // Read the records for the predicate //
                    Record rrec = rstream.ReadNext();
                    MetaData.RightReadCount++;

                    // Check the predicate //
                    if (Predicate.Compare(lrec, rrec) == 0 && Intersection)
                    {

                        // Load the variant //
                        variant.SetValue(0, lrec);
                        variant.SetValue(1, rrec);

                        // Tag taht we found a match //
                        MatchFound = true;
                        MetaData.MatchSuccess++;

                        // Evaluate the where //
                        if (Where.Evaluate(variant))
                        {
                            Record x = Fields.Evaluate(variant);
                            Output.Insert(x);
                            MetaData.WriteCount++;
                        }

                    }

                }

                // Handle the fail match //
                if (!MatchFound && Antisection)
                {

                    // Load the variant //
                    variant.SetValue(0, lrec);
                    variant.SetValue(1, Right.Columns.NullRecord);

                    // Tag taht we found a match //
                    MatchFound = true;
                    MetaData.MatchFailes++;

                    // Evaluate the where //
                    if (Where.Evaluate(variant))
                    {
                        Record x = Fields.Evaluate(variant);
                        Output.Insert(x);
                        MetaData.WriteCount++;
                    }

                }

            }

            // Turn off the timer //
            sw.Stop();
            MetaData.RunTime = sw.Elapsed;

        }

    }


}
