using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    /// <summary>
    /// Represents a field from a table
    /// </summary>
    public sealed class ExpressionFieldRef : Expression
    {

        private int _HeapRef;
        private int _ColumnRef;

        public ExpressionFieldRef(Expression Parent, int HeapRef, int ColumnRef)
            : base(Parent, ExpressionAffinity.Field)
        {
            this._HeapRef = HeapRef;
            this._ColumnRef = ColumnRef;
        }

        public int HeapRef
        {
            get { return this._HeapRef; }
        }

        public int ColumnRef
        {
            get { return this._ColumnRef; }
        }

        // Overrides //
        public override Expression CloneOfMe()
        {
            return new ExpressionFieldRef(this._ParentNode, this._HeapRef, this._ColumnRef);
        }

        public override int ExpressionSize(FieldResolver Variants)
        {
            return Variants.GetFieldSize(this._HeapRef, this._ColumnRef);
        }

        public override CellAffinity ReturnAffinity(FieldResolver Variants)
        {
            return Variants.GetFieldAffinity(this._HeapRef, this._ColumnRef);
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return Variants.GetField(this._HeapRef, this._ColumnRef);
        }

    }

}
