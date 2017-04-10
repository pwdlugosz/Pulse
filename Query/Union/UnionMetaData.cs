using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Union
{

    /// <summary>
    /// 
    /// </summary>
    public class UnionMetaData
    {

        /// <summary>
        /// The time it took to run the select
        /// </summary>
        public TimeSpan RunTime { get; set; }

        /// <summary>
        /// The number of records written to the stream passed
        /// </summary>
        public long WriteCount { get; set; }

        /// <summary>
        /// The number of record read from the source table
        /// </summary>
        public long ReadCount { get; set; }

        /// <summary>
        /// Count of all tables selected from
        /// </summary>
        public long TableCount { get; set; }

    }

}
