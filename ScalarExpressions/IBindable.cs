using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.ScalarExpressions
{
    
    /// <summary>
    /// Contains methods that allow for expression binding. Expression binding is the process of replacing pointer expressions with non-pointer expressions
    /// </summary>
    public interface IBindable
    {

        void Bind(string PointerRef, ScalarExpression Value);

    }

}
