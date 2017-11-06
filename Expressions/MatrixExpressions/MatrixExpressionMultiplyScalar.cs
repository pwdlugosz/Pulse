using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionMultiplyScalar : MatrixExpression
    {

        private int _Association = 0; // 0 == left (A * B[]), 1 == right (B[] * A)
        private ScalarExpression _expression;

        public MatrixExpressionMultiplyScalar(MatrixExpression Parent, ScalarExpression Expression, int Association)
            : base(Parent)
        {
            this._Association = Association;
            this._expression = Expression;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            if (this._Association == 0)
                return this._expression.Evaluate(Variant) * this[0].Evaluate(Variant);
            else
                return this[0].Evaluate(Variant) * this._expression.Evaluate(Variant);
        }

        public override CellAffinity ReturnAffinity()
        {
            if (this._Association == 0)
                return this._expression.ExpressionReturnAffinity();
            else
                return this[0].ReturnAffinity();
        }

        public override int ReturnSize()
        {
            if (this._Association == 0)
                return this._expression.ExpressionSize();
            else
                return this[0].ReturnSize();
        }

        public override MatrixExpression CloneOfMe()
        {
            MatrixExpression node = new MatrixExpressionMultiplyScalar(this.ParentNode, this._expression.CloneOfMe(), this._Association);
            foreach (MatrixExpression m in this._Children)
                node.AddChildNode(m.CloneOfMe());
            return node;
        }

    }

}
