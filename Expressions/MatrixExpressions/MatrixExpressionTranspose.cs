using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionTranspose : MatrixExpression
    {

        public MatrixExpressionTranspose(MatrixExpression Parent)
            : base(Parent)
        {
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            return this[0].Evaluate(Variant).Transposition;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this[0].ReturnAffinity();
        }

        public override int ReturnSize()
        {
            return this._Children[0].ReturnSize();
        }

        public override MatrixExpression CloneOfMe()
        {
            MatrixExpression node = new MatrixExpressionTranspose(this.ParentNode);
            foreach (MatrixExpression m in this._Children)
                node.AddChildNode(m.CloneOfMe());
            return node;
        }

    }

}
