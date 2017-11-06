using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;


namespace Pulse.Tables
{

    /// <summary>
    /// Writes data to the underlying table, removing duplicate records first
    /// </summary>
    public class RecordWriterDistinct : RecordWriterBase
    {

        private DictionaryTable _t;

        public RecordWriterDistinct(Table Data)
            : base(Data)
        {
            this._t = new DictionaryTable(Data.Host, Host.RandomName, Host.TEMP, Data.Columns, Data.Columns, Page.DEFAULT_SIZE);
        }

        public override void Insert(Record Value)
        {
            
            // If the key does not exists, insert it //
            if (!this._t.ContainsKey(Value))
            {
                base.Insert(Value);
                this._t.Insert(Value);
            }

        }

        public override void Close()
        {
            this._Parent.Host.Store.DropTable(this._t.Key);
        }

    }

}
