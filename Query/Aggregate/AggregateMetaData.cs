using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Aggregate
{

    /// <summary>
    /// 
    /// </summary>
    public class AggregateMetaData
    {

        /// <summary>
        /// The actual cost of the grouping
        /// </summary>
        public double ActualCost
        {
            get;
            set;
        }

        /// <summary>
        /// The expected cost of running the query
        /// </summary>
        public double ExpectedCost { get; set; }

        /// <summary>
        /// The time it took to run the grouping
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
        /// The read count for all records passing the filter
        /// </summary>
        public long FilteredReadCount { get; set; }

    }

}
