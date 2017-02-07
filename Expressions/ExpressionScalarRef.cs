using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    /// <summary>
    /// Represents a scalar in a heap
    /// </summary>
    public sealed class ExpressionScalarRef : Expression
    {

        private int _HeapRef;
        private int _ScalarRef;

        public ExpressionScalarRef(Expression Parent, int HeapRef, int ScalarRef)
            : base(Parent, ExpressionAffinity.Heap)
        {
            this._HeapRef = HeapRef;
            this._ScalarRef = ScalarRef;
        }

        public int HeapRef
        {
            get { return this._HeapRef; }
        }

        public int ScalarRef
        {
            get { return this._ScalarRef; }
        }

        // Overrides //
        public override Expression CloneOfMe()
        {
            return new ExpressionScalarRef(this._ParentNode, this._HeapRef, this._ScalarRef);
        }

        public override int ExpressionSize(FieldResolver Variants)
        {
            return Variants.GetScalar(this._HeapRef, this._ScalarRef).DataCost;
        }

        public override CellAffinity ReturnAffinity(FieldResolver Variants)
        {
            return Variants.GetScalar(this._HeapRef, this._ScalarRef).Affinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return Variants.GetScalar(this._HeapRef, this._ScalarRef);
        }

    }

}
