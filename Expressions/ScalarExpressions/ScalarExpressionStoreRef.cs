using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.ScalarExpressions
{

    /// <summary>
    /// Represents a scalar in a heap
    /// </summary>
    public sealed class ScalarExpressionStoreRef : ScalarExpression
    {

        private string _StoreName;
        private string _FieldName;
        private CellAffinity _Affinity;
        private int _Size;

        public ScalarExpressionStoreRef(ScalarExpression Parent, string StoreName, string FieldName, CellAffinity Affinity, int Size)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._StoreName = StoreName;
            this._FieldName = FieldName;
            this._Affinity = Affinity;
            this._Size = Size;
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return this._FieldName;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionStoreRef(this.ParentNode, this._StoreName, this._FieldName, this._Affinity, this._Size);
        }

        public override int ExpressionSize()
        {
            return this._Size;
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._Affinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return Variants[this._StoreName].GetScalar(this._FieldName);
        }

    }


}
