using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Query.Join
{

    public enum JoinAlgorithm
    {
        NestedLoop,
        QuasiNestedLoop,
        SortMerge
    }

}
