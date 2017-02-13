using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    /// <summary>
    /// Represents a constant value
    /// </summary>
    public sealed class ExpressionConstant : Expression
    {

        private Cell _Value;

        public ExpressionConstant(Expression Parent, Cell Value)
            : base(Parent, ExpressionAffinity.Value)
        {
            this._Value = Value;
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return this._Value.valueSTRING;
        }

        public override Expression CloneOfMe()
        {
            return new ExpressionConstant(this._ParentNode, this._Value);
        }

        public override int ExpressionSize()
        {
            return this._Value.DataCost;
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._Value.Affinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return this._Value;
        }

    }

}
