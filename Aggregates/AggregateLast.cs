using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Aggregates
{


    public class AggregateLast : Aggregate
    {

        private Expression _Value;

        public AggregateLast(Expression Value, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
        }

        public AggregateLast(Expression Value)
            : this(Value, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 1;
        }

        public override CellAffinity AggregateAffinity()
        {
            return this._Value.ExpressionReturnAffinity();
        }

        public override int AggregateSize()
        {
            return this._Value.ExpressionSize();
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateLast(this._Value, this._Filter);
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(this.AggregateAffinity());
        }

        public override void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._Value.Evaluate(Variants);
            Work[Offset] = x;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            MergeIntoWork[Offset] = Work[Offset];

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset];
        }

    }


}
