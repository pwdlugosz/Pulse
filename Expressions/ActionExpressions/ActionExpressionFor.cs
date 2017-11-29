using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.ActionExpressions
{

    public sealed class ActionExpressionFor : ActionExpression
    {

        private ObjectStore _Store;
        private string _StoreRef;
        private ScalarExpression _Start;
        private ScalarExpression _Control;
        private ActionExpression _Increment;
        private bool _Escape = false;

        public ActionExpressionFor(Host Host, ActionExpression Parent, ObjectStore Store, string StoreRef,
            ScalarExpression Start, ScalarExpression Control, ActionExpression Increment)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._StoreRef = StoreRef;
            this._Start = Start;
            this._Control = Control;
            this._Increment = Increment;
        }

        public override void TriggerEscapeCurrent()
        {
            this._Escape = true;
        }

        public override void BeginInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => { x.BeginInvoke(Variant); });
            this._Increment.BeginInvoke(Variant);
        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => { x.EndInvoke(Variant); });
            this._Increment.EndInvoke(Variant);
        }

        public override void Invoke(FieldResolver Variant)
        {

            this._Escape = false;

            this._Store.SetScalar(this._StoreRef, this._Start.Evaluate(Variant));

            while (this._Control.Evaluate(Variant).valueBOOL)
            {

                foreach (ActionExpression ae in this._Children)
                {

                    ae.Invoke(Variant);
                    if (this._Escape)
                        break;

                }

                this._Increment.Invoke(Variant);

            }

            //for (this._Store[this._StoreRef] = this._Start.Evaluate(Variant); this._Control.Evaluate(Variant); this._Increment.Invoke(Variant))
            //{

            //    // Invoke all the children
            //    this._Children.ForEach((y) => { y.Invoke(Variant); });

            //}

        }

    }

    public class ActionExpressionEscape : ActionExpression
    {

        public ActionExpressionEscape(Host Host, ActionExpression Parent)
            : base(Host, Parent)
        {
        }

        public override void Invoke(FieldResolver Variant)
        {

            this.TriggerEscapeCurrent();

        }

    }

}
