using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Reads the record keys from an index
    /// </summary>
    public class IndexKeyReadStream : VanillaReadStream
    {

        private IndexHeader _Header;

        public IndexKeyReadStream(IndexHeader Header, Table Storage, RecordKey LKey, RecordKey RKey)
            : base(Storage, LKey, RKey)
        {
            this._Header = Header;
        }

        public IndexKeyReadStream(IndexHeader Header, Table Storage)
            : this(Header, Storage, VanillaReadStream.OriginKey(Storage, Header), VanillaReadStream.TerminalKey(Storage, Header))
        {
        }

        public RecordKey ReadKey()
        {
            return new RecordKey(this.Read()[this._Header.PointerIndex]);
        }

        public RecordKey ReadNextKey()
        {
            return new RecordKey(this.ReadNext()[this._Header.PointerIndex]);
        }

    }


}
