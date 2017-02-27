using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;
using Pulse.Util;

namespace Pulse.Query.Join
{

    /// <summary>
    /// Helper methods to pick the best join stream
    /// </summary>
    public static class JoinOptimizer
    {

        public static double NestedLoopCost(Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {
            return CostCalculator.NestedLoopJoinCost(Left.RecordCount, Right.RecordCount);
        }

        public static double QuasiNestedLoopCost(Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {

            bool IsIndexed = Right.IsIndexedBy(Predicate.RightKey);
            return CostCalculator.QuasiNestedLoopJoinCost(Left.RecordCount, Right.RecordCount, !IsIndexed);

        }

        public static double SortMergeCost(Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {

            bool LeftIsIndexed = Left.IsIndexedBy(Predicate.LeftKey);
            bool RightIsIndexed = Right.IsIndexedBy(Predicate.RightKey);
            return CostCalculator.SortMergeNestedLoopJoinCost(Left.RecordCount, Right.RecordCount, !LeftIsIndexed, !RightIsIndexed);

        }

        public static JoinStream.JoinAlgorithm LowestCost(Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {

            double nl = JoinOptimizer.NestedLoopCost(Left, Right, Predicate, Type);
            double qnl = JoinOptimizer.QuasiNestedLoopCost(Left, Right, Predicate, Type);
            double sm = JoinOptimizer.SortMergeCost(Left, Right, Predicate, Type);

            if (sm < nl && sm < qnl)
                return JoinStream.JoinAlgorithm.SORT_MERGE;
            else if (qnl < nl)
                return JoinStream.JoinAlgorithm.QUASI_NESTED_LOOP;

            return JoinStream.JoinAlgorithm.NESTED_LOOP;


        }

        public static JoinStream RenderNestedLoopJoinStream(Host Host, Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {
            return new NestedLoopJoinStream(new FieldResolver(Host), Left.OpenReader(), Right.OpenReader(), Predicate, Type);
        }

        public static JoinStream RenderQuasiNestedLoopJoinStream(Host Host, Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {

            Index idx = Right.GetIndex(Predicate.RightKey);
            if (idx == null) idx = Right.CreateTemporyIndex(Predicate.RightKey);

            return new QuasiNestedLoopJoinStream(new FieldResolver(Host), Left.OpenReader(), idx, Predicate, Type);

        }

        public static JoinStream RenderSortMergeJoinStream(Host Host, Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {

            Index lidx = Left.GetIndex(Predicate.LeftKey);
            if (lidx == null) lidx = Left.CreateTemporyIndex(Predicate.LeftKey);
            Index ridx = Right.GetIndex(Predicate.RightKey);
            if (ridx == null) ridx = Right.CreateTemporyIndex(Predicate.RightKey);

            return new SortMergeJoinStream(new FieldResolver(Host), lidx.OpenReader(), ridx.OpenReader(), Predicate, Type);

        }

        public static JoinStream Optimize(Host Host, Table Left, Table Right, RecordMatcher Predicate, JoinStream.JoinType Type)
        {

            JoinStream.JoinAlgorithm engine = LowestCost(Left, Right, Predicate, Type);
            if (engine == JoinStream.JoinAlgorithm.NESTED_LOOP)
                return RenderNestedLoopJoinStream(Host, Left, Right, Predicate, Type);
            else if (engine == JoinStream.JoinAlgorithm.QUASI_NESTED_LOOP)
                return RenderQuasiNestedLoopJoinStream(Host, Left, Right, Predicate, Type);
            else
                return RenderSortMergeJoinStream(Host, Left, Right, Predicate, Type);

        }

    }

}
