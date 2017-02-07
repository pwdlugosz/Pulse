using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{


    /// <summary>
    /// Represents a scalar in a matrix in a heap
    /// </summary>
    public sealed class ExpressionMatrixRef : Expression
    {

        private int _HeapRef;
        private int _MatrixRef;

        public ExpressionMatrixRef(Expression Parent, int HeapRef, int MatrixRef)
            : base(Parent, ExpressionAffinity.Matrix)
        {
            this._HeapRef = HeapRef;
            this._MatrixRef = MatrixRef;
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
        public override Expression CloneOfMe()
        {
            return new ExpressionScalarRef(this._ParentNode, this._HeapRef, this._MatrixRef);
        }

        public override int ExpressionSize(FieldResolver Variants)
        {
            return Variants.GetScalar(this._HeapRef, this._MatrixRef).DataCost;
        }

        public override CellAffinity ReturnAffinity(FieldResolver Variants)
        {
            return Variants.GetMatrix(this._HeapRef, this._MatrixRef).Affinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            int row = this._ChildNodes.Count >= 1 ? (int)this._ChildNodes[0].Evaluate(Variants).INT : 0;
            int col = this._ChildNodes.Count >= 2 ? (int)this._ChildNodes[1].Evaluate(Variants).INT : 0;
            return Variants.GetMatrix(this._HeapRef, this._MatrixRef)[row, col];
        }

    }


}
