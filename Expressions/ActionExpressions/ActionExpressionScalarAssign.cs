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

        private string _Name;
        private Assignment _Logic;
        private ObjectStore _Store;
        private ScalarExpression _Value;

        public ActionExpressionScalarAssign(Host Host, ActionExpression Parent, ObjectStore Store, string Name, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Logic = Logic;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {

            switch (this._Logic)
            {

                case Assignment.Equals:
                    this._Store.Scalars[this._Name] = this._Value.Evaluate(Variant);
                    break;
                case Assignment.PlusEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] + this._Value.Evaluate(Variant);
                    break;
                case Assignment.MinusEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] - this._Value.Evaluate(Variant);
                    break;
                case Assignment.ProductEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] * this._Value.Evaluate(Variant);
                    break;
                case Assignment.DivideEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] / this._Value.Evaluate(Variant);
                    break;
                case Assignment.CheckDivideEquals:
                    this._Store.Scalars[this._Name] = Cell.CheckDivide(this._Store.Scalars[this._Name], this._Value.Evaluate(Variant));
                    break;
                case Assignment.ModEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] % this._Value.Evaluate(Variant);
                    break;

            }

        }

    }


}
