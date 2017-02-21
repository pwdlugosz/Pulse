﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions;
using Pulse.Data;


namespace Pulse.Aggregates
{


    public class AggregateMax : Aggregate
    {

        private Expression _Value;

        public AggregateMax(Expression Value, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
        }

        public AggregateMax(Expression Value)
            : this(Value, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 1;
        }

        public override Schema WorkSchema()
        {
            Schema s = new Schema();
            s.Add("A", this.AggregateAffinity(), this.AggregateSize());
            return s;
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
            return new AggregateMax(this._Value.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = Cell.MinValue(this.AggregateAffinity(), this.AggregateSize());
        }

        public override void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._Value.Evaluate(Variants);

            if (x.IsNull)
                return;

            if (Work[Offset].IsNull)
            {
                Work[Offset] = x;
            }
            else if (Work[Offset] < x)
            {
                Work[Offset] = x;
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
            else if (MergeIntoWork[Offset] < Work[Offset])
            {
                MergeIntoWork[Offset] = Work[Offset];
            }

        }

        public override Cell Evaluate(Record Work, int Offset)
        {
            return Work[Offset];
        }

    }


}
