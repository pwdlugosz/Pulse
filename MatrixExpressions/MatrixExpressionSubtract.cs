﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.MatrixExpressions
{

    public sealed class MatrixExpressionSubtract : MatrixExpression
    {

        public MatrixExpressionSubtract(MatrixExpression Parent)
            : base(Parent)
        {
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            return this[0].Evaluate(Variant) - this[1].Evaluate(Variant);
        }

        public override CellAffinity ReturnAffinity()
        {
            return this[0].ReturnAffinity();
        }

        public override MatrixExpression CloneOfMe()
        {
            MatrixExpression node = new MatrixExpressionSubtract(this.ParentNode);
            foreach (MatrixExpression m in this._Cache)
                node.AddChildNode(m.CloneOfMe());
            return node;
        }

    }

}
