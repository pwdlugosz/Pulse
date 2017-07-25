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

        public static JoinAlgorithm LowestCost(long LCount, long RCount)
        {

            double nl = Util.CostCalculator.NestedLoopJoinCost(LCount, RCount);
            double qnl = Util.CostCalculator.QuasiNestedLoopJoinCost(LCount, RCount, false);
            double sm = Util.CostCalculator.SortMergeNestedLoopJoinCost(LCount, RCount, false, false);

            if (sm < nl && sm < qnl)
                return JoinAlgorithm.SortMerge;
            else if (qnl < nl)
                return JoinAlgorithm.QuasiNestedLoop;

            return JoinAlgorithm.NestedLoop;


        }

    }

}
