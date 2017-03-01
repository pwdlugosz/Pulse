using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;
using Pulse.Aggregates;
using Pulse.Query.Acceptors;

namespace Pulse.Query
{

    /// <summary>
    /// Represents the base class for all aggregate streams
    /// </summary>
    public abstract class AggregateStream : AcceptorStream, IColumns
    {

        protected WriteStream _Writer;
        protected ExpressionCollection _Keys;
        protected AggregateCollection _Values;
        protected FieldResolver _Resolver;
        protected Key _WeakKeyColumns;
        protected int _WorkOffset = 0;

        public AggregateStream(WriteStream OutWriter, ExpressionCollection Keys, AggregateCollection Values)
        {
        
            this._Writer = OutWriter;
            this._Keys = Keys;
            this._Values = Values;
            this._WeakKeyColumns = Key.Build(this._Keys.Count);
            this._WorkOffset = this._Keys.Count;

        }

        public Schema Columns
        {
            get 
            { 
                return Schema.Join(this._Keys.Columns, this._Values.Columns); 
            }
        }

        public Schema WorkColumns
        {
            get { return Schema.Join(this._Keys.Columns, this._Values.WorkColumns); }
        }

        public ExpressionCollection Keys
        {
            get { return this._Keys; }
        }

        public AggregateCollection Values
        {
            get { return this._Values; }
        }

        public Record BuildWorkRecord()
        {
            Record r = this.WorkColumns.NullRecord;
            this._Values.Initialize(r, this._WorkOffset);
            return r;
        }

        public void OverLay(Record WorkData, Record Key)
        {
            Array.Copy(Key._data, WorkData._data, Key.Count);
        }

        public Record Evaluate(Record WorkData)
        {
            Record k = Record.Split(WorkData, this._WeakKeyColumns);
            Record v = this._Values.Evaluate(WorkData, this._WorkOffset);
            return Record.Join(k, v);
        }


    }

}
