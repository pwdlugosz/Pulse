using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Query.Beacons;

namespace Pulse.Query.Join
{

    /// <summary>
    /// Represents the base class for all join streams
    /// </summary>
    public abstract class JoinStream : BeaconStream
    {

        /*
         * The user must override:
         * -- CanAdvance
         * -- JoinAdvance
         * -- JoinMatchFound
         * 
         */

        public const int LEFT_IDX = 0;
        public const int RIGHT_IDX = 1;

        public enum JoinType
        {
            INNER,
            LEFT,
            ANTI_LEFT,
            FULL_OUTER
        }

        public enum JoinAlgorithm
        {
            NESTED_LOOP,
            QUASI_NESTED_LOOP,
            SORT_MERGE
        }

        protected RecordMatcher _JoinPredicate;
        protected long _Steps = 0;
        protected JoinType _JoinAffinity;
        
        public JoinStream(Host Host, FieldResolver Variants, RecordMatcher JoinPredicate, JoinType Affinity)
            : base(Host, Variants)
        {
            this._JoinPredicate = JoinPredicate;
            this._JoinAffinity = Affinity;
        }

        public override void Advance()
        {
            do
            {
                this.JoinAdvance();
            } while (!this.JoinMatchFound() && this.CanAdvance);
        }

        public override long Position()
        {
            return this._Steps;
        }

        public abstract void JoinAdvance();

        public abstract bool JoinMatchFound();

    
    }


}
