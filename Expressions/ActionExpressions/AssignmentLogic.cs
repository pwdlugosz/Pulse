using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Expressions.ActionExpressions
{

    public enum Assignment
    {
        Equals,
        PlusEquals,
        MinusEquals,
        ProductEquals,
        DivideEquals,
        CheckDivideEquals,
        ModEquals
        // Auto increment and decrement are handeled via the parser
    }

}
