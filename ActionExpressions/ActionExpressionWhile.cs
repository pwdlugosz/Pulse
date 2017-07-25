using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.ActionExpressions
{
    
    public class ActionExpressionWhile : ActionExpression
    {

        private ScalarExpression _Predicate;
        private bool _Escape = false;

        public ActionExpressionWhile(Host Host, ActionExpression Parent, ScalarExpression Predicate)
            : base(Host, Parent)
        {
            this._Predicate = Predicate;
        }

        public override void TriggerEscapeCurrent()
        {
            this._Escape = true;
        }

        public override void BeginInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => { x.BeginInvoke(Variant); });
        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => { x.EndInvoke(Variant); });
        }

        public override void Invoke(FieldResolver Variant)
        {

            while (this._Predicate.Evaluate(Variant).valueBOOL == true)
            {

                foreach (ActionExpression ae in this._Children)
                {

                    ae.Invoke(Variant);
                    if (this._Escape)
                        break;

                }

            }

        }
    
    }

}
