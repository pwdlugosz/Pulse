using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.ActionExpressions
{

    public abstract class ActionExpression : IBindable
    {

        protected Host _Host;
        protected ActionExpression _Parent;
        protected List<ActionExpression> _Children;
        
        public ActionExpression(Host Host, ActionExpression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<ActionExpression>();
        }

        public ActionExpression Parent
        {
            get { return this._Parent; }
        }

        public List<ActionExpression> Children
        {
            get { return this._Children; }
        }

        public void AddChild(ActionExpression Node)
        {
            Node._Parent = this;
            this._Children.Add(Node);
        }

        public void AddChilren(List<ActionExpression> Nodes)
        {
            foreach(ActionExpression ae in Nodes)
            {
                this.AddChild(ae);
            }
        }

        // Escapes //
        public virtual void TriggerEscapeCurrent()
        {
            if (this._Parent != null)
                this._Parent.TriggerEscapeCurrent();
        }

        // Virtuals and Abstracts //
        public virtual void BeginInvoke(FieldResolver Variant)
        {
        }

        public virtual void EndInvoke(FieldResolver Variant)
        {
        }

        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._Children.ForEach((x) => { x.Bind(PointerRef, Value); });
        }

        public virtual FieldResolver CreateResolver()
        {

            FieldResolver f = new FieldResolver(this._Host);

            foreach (ActionExpression x in this._Children)
            {
                f = FieldResolver.Union(f, x.CreateResolver());
            }

            return f;

        }

        public abstract void Invoke(FieldResolver Variant);

    }

}
