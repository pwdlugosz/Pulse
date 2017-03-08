using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Query.Join
{

    /// <summary>
    /// Meta data about joins
    /// </summary>
    public class JoinMetaData
    {

        /// <summary>
        /// The actual cost of the join
        /// </summary>
        public double ActualJoinCost
        {
            get
            {
                return (double)this.MatchSuccess + (double)this.MatchFailes;
            }
        }

        /// <summary>
        /// The expected cost of the join
        /// </summary>
        public double ExpectedJoinCost { get; set; }

        /// <summary>
        /// The number of match fails; this is the number of null right records that would appear in a left join; this number is 0 for inner joins or left joins that match every record
        /// </summary>
        public long MatchFailes { get; set; }

        /// <summary>
        /// This is the number of matches found; this correlates to the number of record that matches that occur in an inner join abscent of tuples
        /// </summary>
        public long MatchSuccess { get; set; }

        /// <summary>
        /// The time it took to run the join
        /// </summary>
        public TimeSpan RunTime { get; set; }

        /// <summary>
        /// The number of record written to the stream passed
        /// </summary>
        public long WriteCount { get; set; }

        /// <summary>
        /// The number of record read from the left table
        /// </summary>
        public long LeftReadCount { get; set; }

        /// <summary>
        /// The number of record read form the right table
        /// </summary>
        public long RightReadCount { get; set; }

    }

}
