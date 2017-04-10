using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Query.Select
{


    /// <summary>
    /// 
    /// </summary>
    public class SelectMetaData
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
        /// Meta data about the select
        /// </summary>
        public string MetaData
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Runtime: " + this.RunTime.ToString());
                sb.AppendLine("Reads: " + this.ReadCount.ToString());
                sb.AppendLine("Writes: " + this.WriteCount.ToString());
                return sb.ToString();
            }
        }

    }

}
