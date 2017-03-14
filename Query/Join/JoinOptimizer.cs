using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Util;

namespace Pulse.Query.Join
{

    /// <summary>
    /// Helper methods to pick the best join stream
    /// </summary>
    public static class JoinOptimizer
    {

        public static double NestedLoopCost(Table Left, Table Right, RecordMatcher Predicate, JoinAlgorithm Type)
        {
            return CostCalculator.NestedLoopJoinCost(Left.RecordCount, Right.RecordCount);
        }

        public static double QuasiNestedLoopCost(Table Left, Table Right, RecordMatcher Predicate, JoinAlgorithm Type)
        {

            bool IsIndexed = Right.IsIndexedBy(Predicate.RightKey);
            return CostCalculator.QuasiNestedLoopJoinCost(Left.RecordCount, Right.RecordCount, !IsIndexed);

        }

        public static double SortMergeCost(Table Left, Table Right, RecordMatcher Predicate, JoinAlgorithm Type)
        {

            bool LeftIsIndexed = Left.IsIndexedBy(Predicate.LeftKey);
            bool RightIsIndexed = Right.IsIndexedBy(Predicate.RightKey);
            return CostCalculator.SortMergeNestedLoopJoinCost(Left.RecordCount, Right.RecordCount, !LeftIsIndexed, !RightIsIndexed);

        }

        public static JoinAlgorithm LowestCost(Table Left, Table Right, RecordMatcher Predicate, JoinAlgorithm Type)
        {

            double nl = JoinOptimizer.NestedLoopCost(Left, Right, Predicate, Type);
            double qnl = JoinOptimizer.QuasiNestedLoopCost(Left, Right, Predicate, Type);
            double sm = JoinOptimizer.SortMergeCost(Left, Right, Predicate, Type);

            if (sm < nl && sm < qnl)
                return JoinAlgorithm.SortMerge;
            else if (qnl < nl)
                return JoinAlgorithm.QuasiNestedLoop;

            return JoinAlgorithm.NestedLoop;


        }


    }

}
