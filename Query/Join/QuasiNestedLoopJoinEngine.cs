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
    /// Creates a quasi-nested loop join
    /// </summary>
    public sealed class QuasiNestedLoopJoinEngine : JoinEngine
    {

        /// <summary>
        /// Creates a quasi-nested loop join
        /// </summary>
        public QuasiNestedLoopJoinEngine()
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
        public override void Render(Host Host, WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type, JoinMetaData MetaData)
        {

            // Start the timer //
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Get the right join index //
            Index ridx = Right.GetIndex(Predicate.RightKey);
            if (ridx == null)
                throw new Exception("The right table must have an index over the right join columns");

            // Get the expected join cost //
            MetaData.ExpectedJoinCost = CostCalculator.QuasiNestedLoopJoinCost(Left.RecordCount, Right.RecordCount, false);

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
                Record lrec = lstream.ReadNext();
                ReadStream rstream = ridx.OpenStrictReader(Record.Split(lrec, Predicate.RightKey));
                MetaData.LeftReadCount++;

                // Only Loop through if there's actually a match //
                if (rstream != null)
                {

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
                            MetaData.MatchSuccess++;

                            // Evaluate the where //
                            if (Where.Evaluate(variant))
                            {
                                Record x = Fields.Evaluate(variant);
                                Output.Insert(x);
                                MetaData.WriteCount++;
                            }

                        } // Inner Predicate check

                    } // Right While

                } // Inner

                else if (Antisection)
                {

                    // Load the variant //
                    variant.SetValue(0, lrec);
                    variant.SetValue(1, Right.Columns.NullRecord);

                    // Tag taht we found a match //
                    MetaData.MatchFailes++;

                    // Evaluate the where //
                    if (Where.Evaluate(variant))
                    {
                        Record x = Fields.Evaluate(variant);
                        Output.Insert(x);
                        MetaData.WriteCount++;
                    }

                }// Anti

            } // Left main loop

            // Turn off the timer //
            sw.Stop();
            MetaData.RunTime = sw.Elapsed;

        }

    }


}
