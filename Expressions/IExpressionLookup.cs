using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Expressions
{
    
    /// <summary>
    /// Supports expression function lookups
    /// </summary>
    public interface IExpressionLookup
    {

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        bool Exists(string Name);

        /// <summary>
        /// Looksup a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        ExpressionFunction Lookup(string Name);

    }

}
