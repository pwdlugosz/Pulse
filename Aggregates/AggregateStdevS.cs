using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;

namespace Pulse.Aggregates
{


    public class AggregateStdevS : AggregateVarS
    {

        public AggregateStdevS(Expression Value, Expression Weight, Filter Filter)
            : base(Value, Weight, Filter)
        {
        }

        public AggregateStdevS(Expression Value, Filter Filter)
            : this(Value, Expression.OneNUM, Filter)
        {
        }

        public AggregateStdevS(Expression Value, Expression Weight)
            : this(Value, Weight, Filter.TrueForAll)
        {
        }

        public AggregateStdevS(Expression Value)
            : this(Value, Expression.OneNUM, Filter.TrueForAll)
        {
        }

        public override Aggregate CloneOfMe()
        {
            return new AggregateStdevS(this._Value.CloneOfMe(), this._Weight.CloneOfMe(), this._Filter.CloneOfMe());
        }

        public override Cell Evaluate(Record Work, int Offset)
        {

            if (Work[Offset].valueDOUBLE == 0)
                return Cell.NULL_DOUBLE;
            Cell x = Work[Offset + 1] / Work[Offset];
            Cell y = Work[Offset + 2] / Work[Offset];
            Cell z = Work[Offset + 3] / (Work[Offset + 3] - new Cell(1D));
            return Cell.Sqrt((y - x * x) * z);

        }

    }


}
