using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Expressions.SuperExpressions
{

    /// <summary>
    /// The affinity of a SuperExpression
    /// </summary>
    public enum SuperAffinity : byte
    {
        ScalarExpression,
        RecordExpression,
        MatrixExpression,
        TableExpression
    }

}
