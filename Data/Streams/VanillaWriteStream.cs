using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{


    /// <summary>
    /// A vanilla write stream
    /// </summary>
    public class VanillaWriteStream : WriteStream
    {

        private Table _Parent;
        private long _Ticks = 0;

        public VanillaWriteStream(Table Data)
            : base()
        {
            this._Parent = Data;
        }

        public override Schema Columns
        {
            get { return this._Parent.Columns; }
        }

        public override void Close()
        {
            // do nothing
        }

        public override void Insert(Record Value)
        {
            this._Parent.Insert(Value);
        }

        public override void BulkInsert(IEnumerable<Record> Value)
        {
            this._Parent.Insert(Value);
        }

        public override long WriteCount()
        {
            return this._Ticks;
        }

    }


}
