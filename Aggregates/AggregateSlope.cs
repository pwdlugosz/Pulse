using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Aggregates
{

    public class AggregateSlope : AggregateCovariance
    {

        public AggregateSlope(Expression ValueX, Expression ValueY, Expression Weight, Filter Filter)
            : base(ValueX, ValueY, Weight, Filter)
        {
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateSlope(this._ValueX.CloneOfMe(), this._ValueY.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return Cell.NULL_DOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell x2 = Work[Offset + 2] / Work[Offset] - x * x;
            Cell y = Work[Offset + 3] / Work[Offset];
            Cell y2 = Work[Offset + 4] / Work[Offset] - y * y;
            Cell xy = Work[Offset + 5] / Work[Offset] - x * y;
            return xy / x2;

        }

    }


}
