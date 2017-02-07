using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Reads data from an index
    /// </summary>
    public class IndexDataReadStream : VanillaReadStream
    {

        private IndexHeader _Header;
        private Table _Parent;

        /// <summary>
        /// Opens an indexed reader
        /// </summary>
        /// <param name="Header">The index header</param>
        /// <param name="Storage">The table that stores the index pages</param>
        /// <param name="Parent">The table that stores the data pages; may be the same object as 'Storage'</param>
        /// <param name="LKey">The lower bound key</param>
        /// <param name="RKey">The upper bound key</param>
        public IndexDataReadStream(IndexHeader Header, Table Storage, Table Parent, RecordKey LKey, RecordKey RKey)
            : base(Storage, LKey, RKey)
        {
            this._Header = Header;
            this._Parent = Parent;
        }

        /// <summary>
        /// Opens an indexed reader
        /// </summary>
        /// <param name="Header">The index header</param>
        /// <param name="Storage">The table that stores the index pages</param>
        /// <param name="Parent">The table that stores the data pages; may be the same object as 'Storage'</param>
        public IndexDataReadStream(IndexHeader Header, Table Storage, Table Parent)
            : this(Header, Storage, Parent, VanillaReadStream.OriginKey(Storage, Header), VanillaReadStream.TerminalKey(Storage, Header))
        {
        }

        public override Schema Columns
        {
            get
            {
                return this._Parent.Columns;
            }
        }

        public RecordKey ReadKey()
        {
            return new RecordKey(base.Read()[this._Header.PointerIndex]);
        }

        public RecordKey ReadNextKey()
        {
            return new RecordKey(base.ReadNext()[this._Header.PointerIndex]);
        }

        public override Record Read()
        {
            RecordKey x = new RecordKey(base.Read()[this._Header.PointerIndex]);
            return this._Parent.GetPage(x.PAGE_ID).Select(x.ROW_ID);
        }

    }

}
