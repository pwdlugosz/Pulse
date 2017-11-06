using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.MatrixExpressions;

namespace Pulse.Expressions.ScalarExpressions
{


    /// <summary>
    /// Represents a scalar in a matrix in a heap
    /// </summary>
    public sealed class ScalarExpressionMatrixRef : ScalarExpression
    {

        private MatrixExpression _Value;

        public ScalarExpressionMatrixRef(ScalarExpression Parent, MatrixExpression Value)
            : base(Parent, ScalarExpressionAffinity.Matrix)
        {
            this._Value = Value;
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            string row = this._ChildNodes.Count >= 1 ? this._ChildNodes[0].Unparse(Variants) : "0";
            string col = this._ChildNodes.Count >= 2 ? this._ChildNodes[1].Unparse(Variants) : "0";
            return "TODO: ScalarExpressionMatrixRef.Unparse";
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionMatrixRef(this._ParentNode, this._Value);
        }

        public override int ExpressionSize()
        {
            return this._Value.ReturnSize();
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._Value.ReturnAffinity();
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            int row = this._ChildNodes.Count >= 1 ? (int)this._ChildNodes[0].Evaluate(Variants).LONG : 0;
            int col = this._ChildNodes.Count >= 2 ? (int)this._ChildNodes[1].Evaluate(Variants).LONG : 0;
            return this._Value.Evaluate(Variants)[row, col];
        }

    }


}
