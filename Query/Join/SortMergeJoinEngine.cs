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

    /// <summary>
    /// Creates a sort merge join engine
    /// </summary>
    public sealed class SortMergeJoinEngine : JoinEngine
    {

        public SortMergeJoinEngine()
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
        /// <param name="MetaData">Output of the actual cost of running this join</param>
        public override void Render(Host Host, WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type, JoinMetaData MetaData)
        {

            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Get the left and right join index //
            Index lidx = Left.GetIndex(Predicate.LeftKey);
            if (lidx == null)
                throw new Exception("The right table must have an index over the right join columns");
            Index ridx = Right.GetIndex(Predicate.RightKey);
            if (ridx == null)
                throw new Exception("The right table must have an index over the right join columns");

            // Get the expected join cost //
            MetaData.ExpectedJoinCost = CostCalculator.SortMergeNestedLoopJoinCost(Left.RecordCount, Right.RecordCount, false, false);

            // Get the join tags //
            bool Intersection = (Type == JoinType.INNER || Type == JoinType.LEFT), Antisection = (Type == JoinType.LEFT || Type == JoinType.ANTI_LEFT);

            // Open a read stream //
            ReadStream lstream = Left.OpenReader();
            ReadStream rstream = Right.OpenReader();
            MetaData.LeftReadCount++;
            MetaData.RightReadCount++;

            // Create a FieldResolver //
            FieldResolver variant = new FieldResolver(Left.Host);
            variant.AddSchema("L", Left.Columns);
            variant.AddSchema("R", Right.Columns);

            // Main loop through both left and right
            while (lstream.CanAdvance && rstream.CanAdvance)
            {

                int Compare = Predicate.Compare(lstream.Read(), rstream.Read());

                // Left is less than right, step left
                if (Compare < 0)
                {
                    lstream.Advance();
                }
                // Right is less than left, step right, but also output an anti join record
                else if (Compare > 0)
                {

                    if (Antisection)
                    {
                        variant.SetValue(0, lstream.Read());
                        variant.SetValue(1, rstream.Columns.NullRecord);
                        MetaData.LeftReadCount++;
                        MetaData.MatchFailes++;
                        if (Where.Evaluate(variant))
                        {
                            Output.Insert(Fields.Evaluate(variant));
                            MetaData.WriteCount++;
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
                        variant.SetValue(0, lstream.Read());
                        variant.SetValue(1, rstream.Read());
                        MetaData.LeftReadCount++;
                        MetaData.RightReadCount++;
                        MetaData.MatchSuccess++;

                        if (Where.Evaluate(variant))
                        {
                            Output.Insert(Fields.Evaluate(variant));
                            MetaData.WriteCount++;
                        }

                        // Advance the right table //
                        rstream.Advance();
                        NestedLoopCount++;

                        // Check if this advancing pushed us to the end of the table //
                        if (!rstream.CanAdvance)
                            break;

                        // Reset the compare token //
                        Compare = Predicate.Compare(lstream.Read(), rstream.Read());
                        MetaData.LeftReadCount++;
                        MetaData.RightReadCount++;

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
                variant.SetValue(1, rstream.Columns.NullRecord);

                // Walk the rest of the left table //
                while (lstream.CanAdvance)
                {

                    variant.SetValue(0, lstream.ReadNext());
                    MetaData.MatchFailes++;
                    if (Where.Evaluate(variant))
                    {
                        Output.Insert(Fields.Evaluate(variant));
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
