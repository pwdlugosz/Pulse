using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Aggregates.AggregateStream
{

    public abstract class AggregateStream : AcceptorStream, IColumns
    {

        protected WriteStream _Writer;
        protected ExpressionCollection _Keys;
        protected AggregateCollection _Values;
        protected ExpressionCollection _FinalValues;
        protected Key _WeakKeyColumns;

        public AggregateStream(WriteStream OutWriter, ExpressionCollection Keys, AggregateCollection Values, ExpressionCollection FinalValues)
        {
        
            this._Writer = OutWriter;
            this._Keys = Keys;
            this._Values = Values;
            if (FinalValues == null)
            {
                this._FinalValues = new ExpressionCollection(Schema.Join(this._Keys.Columns, this._Values.Columns), 0);
            }
            else
            {
                this._FinalValues = FinalValues;
            }
            this._WeakKeyColumns = Key.Build(this._Keys.Count);

        }

        public Schema Columns
        {
            get
            {
                return this._FinalValues.Columns;
            }
        }

        public Schema KeyValueColumns
        {
            get { return Schema.Join(this._Keys.Columns, this._Values.Columns); }
        }

        public ExpressionCollection Keys
        {
            get { return this._Keys; }
        }

        public AggregateCollection Values
        {
            get { return this._Values; }
        }

        public ExpressionCollection OutputValues
        {
            get { return this._FinalValues; }
        }

        public Record BuildWorkRecord(FieldResolver Variants)
        {
            int len = this._Keys.Count + this._Values.SignitureLength();
            Record r = new Record(len);
            Record key = this._Keys.Evaluate(Variants);
            Array.Copy(key._data, 0, r._data, 0, this._Keys.Count);
            return r;

        }

        public abstract void Accept(FieldResolver Variants);

        public abstract void Close();

    }

    public sealed class OrderedAggregateStream : AggregateStream
    {

        private Record _WorkData;
        private Key _KeyColumns;

        public OrderedAggregateStream(WriteStream OutWriter, ExpressionCollection Keys, AggregateCollection Values, ExpressionCollection FinalValues)
            : base(OutWriter, Keys, Values, FinalValues)
        {
            
        }

        public override void Accept(FieldResolver Variants)
        {
            


        }

        public override void Close()
        {
            // Do nothing
        }
    
    }

}
