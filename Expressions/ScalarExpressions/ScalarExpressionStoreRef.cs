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

        private string _VarName;
        private ObjectStore _Store;

        public ScalarExpressionStoreRef(ScalarExpression Parent, string VarName, ObjectStore Store)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._VarName = VarName;
            this._Store = Store;
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return this._VarName;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionStoreRef(this.ParentNode, this._name, this._Store);
        }

        public override int ExpressionSize()
        {
            return this._Store.Scalars[this._VarName].Length;
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._Store.Scalars[this._VarName].Affinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return this._Store.Scalars[this._VarName];
        }

    }


}
