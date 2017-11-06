using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Tables;

namespace Pulse.Elements.Structures
{
    
    public sealed class Structure
    {

        private Record _record;
        private Schema _columns;

        public Structure(Schema Columns, Record Values)
        {
            this._columns = Columns;
            this._record = Columns.NullRecord;
        }

        public Structure(Schema Columns)
            : this(Columns, Columns.NullRecord)
        {
        }

        public Schema Columns
        {
            get { return this._columns; }
        }

        public Record Values
        {
            get { return this._record; }
        }

        public Cell this[int Index]
        {
            get { return this._record[Index]; }
            set { this._record[Index] = value; }
        }

        public Cell this[string Name]
        {
            get { return this._record[this._columns.ColumnIndex(Name)]; }
            set { this._record[this._columns.ColumnIndex(Name)] = value; }
        }


    }



}
