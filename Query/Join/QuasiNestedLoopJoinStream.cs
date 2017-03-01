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
    /// Performs a quasi-nested loop join where the right element is indexed; this costs N x Log(M)
    /// </summary>
    public class QuasiNestedLoopJoinStream : JoinStream
    {

        protected ReadStream _Left;
        protected ReadStream _Right;
        protected Index _RightIndex;
        private bool _TriggerNullMatch = false;

        public QuasiNestedLoopJoinStream(Host Host, FieldResolver Variants, ReadStream LeftStream, Index RightIndex, RecordMatcher JoinPredicate, JoinType Affinity)
            : base(Host, Variants, JoinPredicate, Affinity)
        {

            if (!LeftStream.CanAdvance)
                throw new Exception("Cannot join with an empty stream");

            this._Left = LeftStream;
            this._RightIndex = RightIndex;
            this.SetRightStream();

            this.Variants.SetValue(LEFT_IDX, this._Left.Read());
            this.Variants.SetValue(RIGHT_IDX, this._TriggerNullMatch ? this._RightIndex.Parent.Columns.NullRecord : this._Right.Read());
            if (!this.JoinMatchFound())
                this.Advance();

        }

        public override bool CanAdvance
        {
            get
            {
                if (!this._Left.CanAdvance && this._TriggerNullMatch)
                    return false;
                if (this._TriggerNullMatch)
                    return this._Left.CanAdvance;
                return this._Left.CanAdvance || this._Right.CanAdvance;
            }
        }

        public override void JoinAdvance()
        {

            // Check if we're a null match //
            if (this._TriggerNullMatch)
            {
                this.SetRightStream();
                this._Left.Advance();
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
                this.SetRightStream();
                return;
            }
            else
            {
                return;
            }

        }

        public override bool JoinMatchFound()
        {

            if (this._JoinAffinity == JoinType.INNER || (this._JoinAffinity == JoinType.LEFT && !this._TriggerNullMatch))
            {

                if (!this._Left.CanAdvance || !this._Right.CanAdvance)
                    return false;

                if (this._JoinPredicate.Compare(this._Left.Read(), this._Right.Read()) == 0)
                {

                    this.Variants.SetValue(LEFT_IDX, this._Left.Read());
                    this.Variants.SetValue(RIGHT_IDX, this._Right.Read());
                    return true;

                }

            }

            if (this._JoinAffinity == JoinType.LEFT || this._JoinAffinity == JoinType.ANTI_LEFT)
            {

                if (!this._Left.CanAdvance)
                    return false;

                if (this._TriggerNullMatch)
                {

                    this.Variants.SetValue(LEFT_IDX, this._Left.Read());
                    this.Variants.SetValue(RIGHT_IDX, this._RightIndex.Parent.Columns.NullRecord);

                    return true;

                }

            }

            return false;

        }

        /// <summary>
        /// Positions the right stream to the left stream
        /// </summary>
        private void SetRightStream()
        {

            this._TriggerNullMatch = false;
            if (!this._Left.CanAdvance)
                return;
            Record lk = Record.Split(this._Left.Read(), this._JoinPredicate.LeftKey);
            RecordKey x = this._RightIndex.Tree.SeekFirst(lk, true);
            RecordKey y = this._RightIndex.Tree.SeekLast(lk, true);
            if (x.IsNotFound || y.IsNotFound)
            {
                this._TriggerNullMatch = true;
                this._Right = null;
                return;
            }
            this._Right = new VanillaReadStream(this._RightIndex.Parent, x, y);
        }

    }


}
