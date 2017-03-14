using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Aggregates;
using Pulse.ScalarExpressions;

namespace Pulse.Query
{

    /// <summary>
    /// Represents an aggregate stream that's based on a pre-existing index
    /// </summary>
    public sealed class OrderedAggregateStream : AggregateStream
    {

        private Record _WorkData = null;
        private long _WriteCount = 0;

        public OrderedAggregateStream(WriteStream OutWriter, ScalarExpressionCollection Keys, AggregateCollection Values)
            : base(OutWriter, Keys, Values)
        {

        }

        public override long WriteCount()
        {
            return this._WriteCount;
        }

        public override void Accept(FieldResolver Variants)
        {

            Record key = this._Keys.Evaluate(Variants);
            
            // Very first record //
            if (this._WorkData == null)
            {
                this._WorkData = this.BuildWorkRecord();
                this.OverLay(this._WorkData, key);
            }

            // Different key as last itteration //
            if (Record.Compare(key, this._WorkData, this._WeakKeyColumns) != 0)
            {

                // Output the last key-value //
                Record x = this.Evaluate(this._WorkData);

                // Insert into the stream //
                this._Writer.Insert(x);

                // Re-build the worker record //
                this._WorkData = this.BuildWorkRecord();
                this.OverLay(this._WorkData, key);

            }

            // Accumulate //
            this._Values.Accumulate(Variants, this._WorkData, this._WorkOffset);

            this._WriteCount++;

        }

        public override void Close()
        {

            // Output the last key-value //
            Record x = this.Evaluate(this._WorkData);

            // Insert into the stream //
            this._Writer.Insert(x);

        }

    }


}
