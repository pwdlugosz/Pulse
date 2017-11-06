using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;

namespace Pulse.Expressions.ActionExpressions
{

    public sealed class ActionExpressionScalarAssign : ActionExpression
    {

        private int _HeapRef = 0;
        private Assignment _Logic;
        private Heap<Cell> _Store;
        private ScalarExpression _Value;

        public ActionExpressionScalarAssign(Host Host, ActionExpression Parent,Heap<Cell> Store, int HeapRef, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._HeapRef = HeapRef;
            this._Logic = Logic;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {

            //if (this._Store[this._HeapRef].LONG == 1)
            //    throw new Exception();

            switch (this._Logic)
            {

                case Assignment.Equals:
                    this._Store[this._HeapRef] = this._Value.Evaluate(Variant);
                    break;
                case Assignment.PlusEquals:
                    this._Store[this._HeapRef] = this._Store[this._HeapRef] + this._Value.Evaluate(Variant);
                    break;
                case Assignment.MinusEquals:
                    this._Store[this._HeapRef] = this._Store[this._HeapRef] - this._Value.Evaluate(Variant);
                    break;
                case Assignment.ProductEquals:
                    this._Store[this._HeapRef] = this._Store[this._HeapRef] * this._Value.Evaluate(Variant);
                    break;
                case Assignment.DivideEquals:
                    this._Store[this._HeapRef] = this._Store[this._HeapRef] / this._Value.Evaluate(Variant);
                    break;
                case Assignment.CheckDivideEquals:
                    this._Store[this._HeapRef] = Cell.CheckDivide(this._Store[this._HeapRef] , this._Value.Evaluate(Variant));
                    break;
                case Assignment.ModEquals:
                    this._Store[this._HeapRef] = this._Store[this._HeapRef] % this._Value.Evaluate(Variant);
                    break;

            }

        }

    }


}
