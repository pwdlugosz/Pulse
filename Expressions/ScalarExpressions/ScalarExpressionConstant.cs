﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;

namespace Pulse.Expressions.ScalarExpressions
{

    /// <summary>
    /// Represents a constant value
    /// </summary>
    public sealed class ScalarExpressionConstant : ScalarExpression
    {

        private Cell _Value;

        public ScalarExpressionConstant(ScalarExpression Parent, Cell Value)
            : base(Parent, ScalarExpressionAffinity.Value)
        {
            this._Value = Value;
        }

        public Cell Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return this._Value.valueSTRING;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionConstant(this._ParentNode, this._Value);
        }

        public override int ReturnSize()
        {
            return CellSerializer.Length(this._Value);
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Value.Affinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return this._Value;
        }

    }

}
