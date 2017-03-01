using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;
using Pulse.Query;

namespace Pulse.Query.Join
{

    /// <summary>
    /// Performs a nested loop join, with cost N x M
    /// </summary>
    public class NestedLoopJoinStream : JoinStream
    {

        private bool _MatchFound = false; // true = a match was found this cycle, false = match wasnt found this round
        protected ReadStream _Left;
        protected ReadStream _Right;
        protected bool _FirstRun = true;

        public NestedLoopJoinStream(Host Host, FieldResolver Variants, ReadStream LeftStream, ReadStream RightStream, RecordMatcher JoinPredicate, JoinType Affinity)
            : base(Host, Variants, JoinPredicate, Affinity)
        {

            this._Left = LeftStream;
            this._Right = RightStream;

        }

        public override bool CanAdvance
        {
            get 
            { 
                return !this._Left.IsLast || !this._Right.IsLast; 
            }
        }

        public override void JoinAdvance()
        {

            if (this._FirstRun)
            {
                this._FirstRun = false;
                return;
            }

            // Check if we can advance the right stream first //
            if (this._Right.CanAdvance)
            {
                this._Right.Advance();
                return;
            }
            // We're at the end of the right stream and need to reset
            else if (this._Left.CanAdvance)
            {
                this._Left.Advance();
                this._Right.Reset();
                this._MatchFound = false;
                return;
            }
            else
            {
                return;
            }

        }

        public override bool JoinMatchFound()
        {

                
            if (!this._Left.CanAdvance || !this._Right.CanAdvance)
                return false;
            
            if (this._JoinAffinity == JoinType.INNER || this._JoinAffinity == JoinType.LEFT)
            {

                if (this._JoinPredicate.Compare(this._Left.Read(), this._Right.Read()) == 0)
                {

                    this.Variants.SetValue(LEFT_IDX, this._Left.Read());
                    this.Variants.SetValue(RIGHT_IDX, this._Right.Read());
                    this._MatchFound = true;
                    return true;

                }

            }

            if (this._JoinAffinity == JoinType.LEFT || this._JoinAffinity == JoinType.ANTI_LEFT)
            {


                if (this._Right.IsLast && !this._MatchFound)
                {

                    this.Variants.SetValue(LEFT_IDX, this._Left.Read());
                    this.Variants.SetValue(RIGHT_IDX, this._Right.Columns.NullRecord);

                    return true;

                }

            }

            return false;

        }

    }


}
