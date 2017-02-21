﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Aggregates
{


    public class AggregateVarP : Aggregate
    {

        protected Expression _Value;
        protected Expression _Weight;

        public AggregateVarP(Expression Value, Expression Weight, Filter Filter)
            : base(Filter)
        {
            this._Value = Value;
            this._Weight = Weight;
        }

        public AggregateVarP(Expression Value, Filter Filter)
            : this(Value, Expression.OneNUM, Filter)
        {
        }

        public AggregateVarP(Expression Value, Expression Weight)
            : this(Value, Weight, Filter.TrueForAll)
        {
        }

        public AggregateVarP(Expression Value)
            : this(Value, Expression.OneNUM, Filter.TrueForAll)
        {
        }

        public override int SignitureLength()
        {
            return 3;
        }
        
        public override Schema WorkSchema()
        {
            return new Schema("A DOUBLE, B DOUBLE, C DOUBLE, D DOUBLE");
        }

        public override CellAffinity AggregateAffinity()
        {
            return CellAffinity.DOUBLE;
        }

        public override int AggregateSize()
        {
            return Schema.FixSize(CellAffinity.DOUBLE, 0);
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateVarP(this._Value.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override void Initialize(Record Work, int Offset)
        {
            Work[Offset] = new Cell(0D);        // COUNT x
            Work[Offset + 1] = new Cell(0D);    // SUM x
            Work[Offset + 2] = new Cell(0D);    // SUM x * x
        }

        public override void Accumulate(FieldResolver Variants, Record Work, int Offset)
        {

            if (!this._Filter.Evaluate(Variants))
                return;

            Cell x = this._Value.Evaluate(Variants);
            Cell w = this._Weight.Evaluate(Variants);

            if (x.IsNull || w.IsNull || !(CellAffinityHelper.IsNumeric(x.AFFINITY) && CellAffinityHelper.IsNumeric(w.AFFINITY)))
                return;

            Work[Offset] += w;
            Work[Offset + 1] += x;
            Work[Offset + 2] += x * x;

        }

        public override void Merge(Record MergeIntoWork, Record Work, int Offset)
        {

            MergeIntoWork[Offset] += Work[Offset];
            MergeIntoWork[Offset + 1] += Work[Offset + 1];
            MergeIntoWork[Offset + 2] += Work[Offset + 2];

        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return Cell.NULL_DOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell y = Work[Offset + 2] / Work[Offset];
            return y - x * x;

        }

    }


}