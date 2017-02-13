using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions;
using Pulse.Data;

namespace Pulse.Aggregates
{


    public class AggregateCountStar : Aggregate
    {

        public AggregateCountStar(Filter Filter)
            : base(Filter)
        {
        }

        public override int SignitureLength()
        {
            return 1;
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
            return new AggregateCountStar(this._Filter);
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = Cell.NULL_INT;
        }

        public override void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
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
