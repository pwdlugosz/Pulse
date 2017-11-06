using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionLiteral : MatrixExpression
    {

        private CellMatrix _value;

        public MatrixExpressionLiteral(MatrixExpression Parent, CellMatrix Value)
            : base(Parent)
        {
            this._value = Value;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            return this._value;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._value.Affinity;
        }

        public override int ReturnSize()
        {
            return this._value.Size;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionLiteral(this.ParentNode, new CellMatrix(this._value));
        }

    }

    



}
