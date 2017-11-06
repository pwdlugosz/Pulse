using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionHeap : MatrixExpression
    {

        private int _ref;
        private Heap<CellMatrix> _heap;

        public MatrixExpressionHeap(MatrixExpression Parent, Heap<CellMatrix> Mem, int Ref)
            : base(Parent)
        {
            this._ref = Ref;
            this._heap = Mem;
        }

        public Heap<CellMatrix> InnerHeap
        {
            get { return this._heap; }
            set { this._heap = value; }
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            return this._heap[this._ref];
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._heap[this._ref].Affinity;
        }

        public override int ReturnSize()
        {
            return this._heap[this._ref].Size;
        }
        
        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionHeap(this.ParentNode, this._heap, this._ref);
        }

    }

}
