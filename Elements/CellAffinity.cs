using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Elements
{

    /// <summary>
    /// Pulse data type
    /// </summary>
    public enum CellAffinity : byte
    {
        BOOL = 0,
        DATE = 1,
        BYTE = 2,
        SHORT = 3,
        INT = 4,
        LONG = 5,
        FLOAT = 6,
        DOUBLE = 7,
        BLOB = 8,
        TEXT = 9,
        STRING = 10
    }

}
