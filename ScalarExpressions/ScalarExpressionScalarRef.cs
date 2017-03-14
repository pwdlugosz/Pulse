﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.ScalarExpressions
{

    /// <summary>
    /// Represents a scalar in a heap
    /// </summary>
    public sealed class ScalarExpressionScalarRef : ScalarExpression
    {

        private int _HeapRef;
        private int _ScalarRef;
        private int _FieldSize;
        private CellAffinity _FieldAffinity;

        public ScalarExpressionScalarRef(ScalarExpression Parent, int HeapRef, int ScalarRef, CellAffinity FieldAffinity, int FieldSize)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._HeapRef = HeapRef;
            this._ScalarRef = ScalarRef;
            this._FieldAffinity = FieldAffinity;
            this._FieldSize = FieldSize;
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
        public override string Unparse(FieldResolver Variants)
        {
            return Variants.GetScalarName(this._HeapRef, this._ScalarRef);
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionScalarRef(this._ParentNode, this._HeapRef, this._ScalarRef, this._FieldAffinity, this._FieldSize);
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
            return Variants.GetScalar(this._HeapRef, this._ScalarRef);
        }

    }

}