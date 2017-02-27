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
    /// Represents a sort-merge join, where both the left and right tables must be indexed; cost is Max(N,M)
    /// </summary>
    public class SortMergeJoinStream : JoinStream
    {

        protected ReadStream _Left;
        protected ReadStream _Right;
        protected int _TupleCount = -1;
        protected bool _InNestedLoop = false;
        protected int _Compare = 0;

        public SortMergeJoinStream(FieldResolver Variants, ReadStream LeftStream, ReadStream RightStream, RecordMatcher JoinPredicate, JoinType Affinity)
            : base(Variants, JoinPredicate, Affinity)
        {
            this._Left = LeftStream;
            this._Right = RightStream;
            this.Variants.SetValue(LEFT_IDX, this._Left.Read());
            this.Variants.SetValue(RIGHT_IDX, this._Right.Read());
            if (!this.JoinMatchFound())
                this.Advance();
        }

        public override bool CanAdvance
        {
            get
            {
                return this._Left.CanAdvance || this._Right.CanAdvance;
            }
        }

        public override void JoinAdvance()
        {

            // Calculate Compare //
            this.CalculateCompare();

            // Check if we're now breaking the nested loop //
            if (this._InNestedLoop && this._Compare != 0)
            {
                this._Right.Revert(this._TupleCount);
                this._Left.Advance();
                this._InNestedLoop = false;
                this._TupleCount = -1;
                return;
            }

            // Left < Right, need to increase left //
            if (this._Compare < 0)
            {
                this._Left.Advance();
            }
            // Left > Right, need to increase right
            else if (this._Compare > 0)
            {
                this._Right.Advance();
            }
            // Right == Left, need to increase right, and the tuple count
            else
            {
                this._InNestedLoop = true;
                this._TupleCount++;
                this._Right.Advance();
            }

        }

        public override bool JoinMatchFound()
        {

            if (!this._Left.CanAdvance)
                return false;

            this.CalculateCompare();

            if ((this._JoinAffinity == JoinType.INNER || this._JoinAffinity == JoinType.LEFT) && (this._Compare == 0))
            {
                this.Variants.SetValue(LEFT_IDX, this._Left.Read());
                this.Variants.SetValue(RIGHT_IDX, this._Right.Read());
                return true;
            }

            if ((this._JoinAffinity == JoinType.LEFT || this._JoinAffinity == JoinType.ANTI_LEFT) && (this._TupleCount < 0 && this._Compare < 0))
            {
                this.Variants.SetValue(LEFT_IDX, this._Left.Read());
                this.Variants.SetValue(RIGHT_IDX, this._Right.Columns.NullRecord);
                return true;
            }

            return false;

        }

        private void CalculateCompare()
        {

            // Get the compare key //
            this._Compare = 0;
            bool l = this._Left.CanAdvance, r = this._Right.CanAdvance;
            if (!l && r)
            {
                this._Compare = 1;
            }
            else if (l && !r)
            {
                this._Compare = -1;
            }
            else if (!l && !r)
            {
                this._Compare = 0;
            }
            else
            {
                this._Compare = this._JoinPredicate.Compare(this._Left.Read(), this._Right.Read());
            }

        }

    }


}
