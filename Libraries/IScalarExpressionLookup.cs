using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Libraries
{
    
    /// <summary>
    /// Supports expression function lookups
    /// </summary>
    public interface IScalarExpressionLookup
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
        ScalarExpressionFunction Lookup(string Name);

    }

}
