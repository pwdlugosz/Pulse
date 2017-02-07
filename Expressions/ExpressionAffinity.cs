using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    /// <summary>
    /// Represents an expression type
    /// </summary>
    public enum ExpressionAffinity
    {

        /// <summary>
        /// Represents a field in a table
        /// </summary>
        Field,

        /// <summary>
        /// Represents a variable in a heap
        /// </summary>
        Heap,

        /// <summary>
        /// Represents a unit in a matrix
        /// </summary>
        Matrix,
        
        /// <summary>
        /// Represents a constant value
        /// </summary>
        Value,

        /// <summary>
        /// Represents a function
        /// </summary>
        Function,

        /// <summary>
        /// Represents a pointer
        /// </summary>
        Pointer


    }

}
