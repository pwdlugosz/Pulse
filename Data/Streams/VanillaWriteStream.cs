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

        protected Table _Parent;
        protected long _Ticks = 0;

        public VanillaWriteStream(Table Data)
            : base()
        {
            this._Parent = Data;
        }

        public override Table Source
        {
            get { return this._Parent; }
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
