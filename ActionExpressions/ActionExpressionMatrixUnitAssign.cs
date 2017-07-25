using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;

namespace Pulse.ActionExpressions
{

    public sealed class ActionExpressionMatrixUnitAssign : ActionExpression
    {

        private int _HeapRef = 0;
        private Assignment _Logic;
        private Heap<CellMatrix> _Store;
        private ScalarExpression _Value;
        private ScalarExpression _Row;
        private ScalarExpression _Col;

        public ActionExpressionMatrixUnitAssign(Host Host, ActionExpression Parent, Heap<CellMatrix> Store, int HeapRef, ScalarExpression Row, 
            ScalarExpression Col, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._HeapRef = HeapRef;
            this._Logic = Logic;
            this._Value = Value;
            this._Row = Row ?? ScalarExpression.ZeroINT;
            this._Col = Col ?? ScalarExpression.ZeroINT;
        }

        public override void Invoke(FieldResolver Variant)
        {

            int Row = (int)this._Row.Evaluate(Variant).valueINT;
            int Col = (int)this._Col.Evaluate(Variant).valueINT;

            switch (this._Logic)
            {

                case Assignment.Equals:
                    this._Store[this._HeapRef][Row, Col] = this._Value.Evaluate(Variant);
                    break;
                case Assignment.PlusEquals:
                    this._Store[this._HeapRef][Row, Col] = this._Store[this._HeapRef][Row, Col] + this._Value.Evaluate(Variant);
                    break;
                case Assignment.MinusEquals:
                    this._Store[this._HeapRef][Row, Col] = this._Store[this._HeapRef][Row, Col] - this._Value.Evaluate(Variant);
                    break;
                case Assignment.ProductEquals:
                    this._Store[this._HeapRef][Row, Col] = this._Store[this._HeapRef][Row, Col] * this._Value.Evaluate(Variant);
                    break;
                case Assignment.DivideEquals:
                    this._Store[this._HeapRef][Row, Col] = this._Store[this._HeapRef][Row, Col] / this._Value.Evaluate(Variant);
                    break;
                case Assignment.CheckDivideEquals:
                    this._Store[this._HeapRef][Row, Col] = Cell.CheckDivide(this._Store[this._HeapRef][Row, Col], this._Value.Evaluate(Variant));
                    break;
                case Assignment.ModEquals:
                    this._Store[this._HeapRef][Row, Col] = this._Store[this._HeapRef][Row, Col] % this._Value.Evaluate(Variant);
                    break;

            }

        }

    }


}
