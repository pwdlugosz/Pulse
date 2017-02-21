using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions;
using Pulse.Data;

namespace Pulse.Aggregates
{


    public class AggregateCount : Aggregate
    {

        private Expression _Value;

        public AggregateCount(Expression Value, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
        }

        public AggregateCount(Expression Value)
            : this(Value, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 1;
        }
        
        public override Schema WorkSchema()
        {
            return new Schema("A INT");
        }

        public override CellAffinity AggregateAffinity()
        {
            return CellAffinity.INT;
        }

        public override int AggregateSize()
        {
            return Schema.FixSize(CellAffinity.INT, 0);
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateCount(this._Value.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = Cell.NULL_INT;
        }

        public override void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._Value.Evaluate(Variants);

            if (x.IsNull)
                return;

            if (Work[Offset].IsNull)
                Work[Offset] = new Cell(1);
            else
                Work[Offset]++;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            if (MergeIntoWork[Offset].IsNull)
            {
                MergeIntoWork[Offset] = Work[Offset];
            }
            else if (Work[Offset].IsNull)
            {
                return;
            }
            else
            {
                MergeIntoWork[Offset] += Work[Offset];
            }

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset];
        }

    }


}
