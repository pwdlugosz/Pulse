using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions;
using Pulse.Data;


namespace Pulse.Aggregates
{


    public class AggregateMinOf : Aggregate
    {

        private Expression _Key;
        private Expression _Value;

        public AggregateMinOf(Expression Key, Expression Value, Filter Filter)
            : base(Filter)
        {
            this._Key = Key;
            this._Value = Value;
        }

        public AggregateMinOf(Expression Key, Expression Value)
            : this(Key, Value, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 2;
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
            return new AggregateMinOf(this._Key, this._Value, this._Filter);
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(this._Key.ExpressionSize());
            Work[Offset + 1] = new Cell(this.AggregateAffinity());
        }

        public override void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell k = this._Key.Evaluate(Variants);
            Cell v = this._Value.Evaluate(Variants);

            if (k.IsNull)
                return;

            if (Work[Offset].IsNull)
            {
                Work[Offset] = v;
            }
            else if (Work[Offset] > k)
            {
                Work[Offset] = v;
            }

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
            else if (MergeIntoWork[Offset] > Work[Offset])
            {
                MergeIntoWork[Offset] = Work[Offset];
                MergeIntoWork[Offset + 1] = Work[Offset + 1];
            }

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset + 1];
        }

    }


}
