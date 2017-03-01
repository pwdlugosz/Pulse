using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;
using Pulse.Aggregates;

namespace Pulse.Query
{


    public sealed class DictionaryAggregateStream : AggregateStream
    {

        private DictionaryScribeTable _Storage;
        private long _Ticks = 0;

        public DictionaryAggregateStream(WriteStream OutWriter, ExpressionCollection Keys, AggregateCollection Values)
            : base(OutWriter, Keys, Values)
        {

            this._Storage = OutWriter.Source.Host.CreateTable(Host.TEMP, Host.RandomName, Keys.Columns, Values.Columns);

        }

        public override long WriteCount()
        {
            return this._Ticks;
        }

        public override void Accept(FieldResolver Variants)
        {

            // Evaluate the record //
            Record k = this._Keys.Evaluate(Variants);

            // Try to get a key //
            Record v = this._Storage.GetValue(k);

            // Check if the dictionary contains this key, then update //
            if (v != null)
            {

                // Accumulate the value //
                this._Values.Accumulate(Variants, v, 0);

                // Update the working data //
                this._Storage.SetValue(k, v);

            }
            else
            {

                // Create a new work data record //
                v = this._Values.WorkColumns.NullRecord;
                this._Values.Initialize(v, 0);

                // Accumulate the workd record //
                this._Values.Accumulate(Variants, v, 0);

                // Add it to the storage //
                this._Storage.Add(k, v);

            }

        }

        public override void Close()
        {

            // Get the offset //
            int Offset = this._Storage.KeyFields.Count;

            // Open a reader //
            ReadStream reader = this._Storage.OpenReader();

            // Itterate over all key-values //
            while (reader.CanAdvance)
            {

                // Get the work data //
                Record work = reader.ReadNext();

                Record k = Record.Split(work, this._Storage.KeyFields);

                // Render the final values //
                Record v = this._Values.Evaluate(work, Offset);

                // Append the data //
                this._Writer.Insert(Record.Join(k, v));

            }

            // Burn the temp table //
            Host h = this._Storage.Host;
            h.PageCache.DropTable(this._Storage.Key);

        }

    }


}
