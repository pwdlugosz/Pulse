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
        private int _FieldSize;
        private CellAffinity _FieldAffinity;

        public ExpressionFieldRef(Expression Parent, int HeapRef, int ColumnRef, CellAffinity FieldAffinity, int FieldSize)
            : base(Parent, ExpressionAffinity.Field)
        {
            this._HeapRef = HeapRef;
            this._ColumnRef = ColumnRef;
            this._FieldAffinity = FieldAffinity;
            this._FieldSize = FieldSize;
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
        public override string Unparse(FieldResolver Variants)
        {
            return Variants.GetFieldName(this._HeapRef, this._ColumnRef);
        }

        public override Expression CloneOfMe()
        {
            return new ExpressionFieldRef(this._ParentNode, this._HeapRef, this._ColumnRef, this._FieldAffinity, this._FieldSize);
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
            return Variants.GetField(this._HeapRef, this._ColumnRef);
        }

    }

}
