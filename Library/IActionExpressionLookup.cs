using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.ActionExpressions;

namespace Pulse.Libraries
{
    
    public interface IActionExpressionLookup
    {

        bool Exists(string Name);

        ActionExpression Lookup(string Name);

    }

}
