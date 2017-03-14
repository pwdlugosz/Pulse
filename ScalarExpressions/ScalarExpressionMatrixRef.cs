using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.ScalarExpressions
{


    /// <summary>
    /// Represents a scalar in a matrix in a heap
    /// </summary>
    public sealed class ScalarExpressionMatrixRef : ScalarExpression
    {

        private int _HeapRef;
        private int _MatrixRef;
        private int _FieldSize;
        private CellAffinity _FieldAffinity;

        public ScalarExpressionMatrixRef(ScalarExpression Parent, int HeapRef, int MatrixRef, CellAffinity FieldAffinity, int FieldSize)
            : base(Parent, ScalarExpressionAffinity.Matrix)
        {
            this._HeapRef = HeapRef;
            this._MatrixRef = MatrixRef;
            this._FieldAffinity = FieldAffinity;
            this._FieldSize = FieldSize;
        }

        public int HeapRef
        {
            get { return this._HeapRef; }
        }

        public int MatrixRef
        {
            get { return this._MatrixRef; }
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            string row = this._ChildNodes.Count >= 1 ? this._ChildNodes[0].Unparse(Variants) : "0";
            string col = this._ChildNodes.Count >= 2 ? this._ChildNodes[1].Unparse(Variants) : "0";
            return Variants.GetMatrixName(this._HeapRef, this._MatrixRef) + "[" + row + "," + col + "]";
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionMatrixRef(this._ParentNode, this._HeapRef, this._MatrixRef, this._FieldAffinity, this._FieldSize);
        }

        public override int ExpressionSize()
        {
            return this._FieldSize;
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._FieldAffinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            int row = this._ChildNodes.Count >= 1 ? (int)this._ChildNodes[0].Evaluate(Variants).INT : 0;
            int col = this._ChildNodes.Count >= 2 ? (int)this._ChildNodes[1].Evaluate(Variants).INT : 0;
            return Variants.GetMatrix(this._HeapRef, this._MatrixRef)[row, col];
        }

    }


}
