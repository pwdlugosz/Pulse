using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.ActionExpressions
{

    public abstract class ActionExpression
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

        // Virtuals and Abstracts //
        public virtual void BeginInvoke(FieldResolver Variant)
        {
        }

        public virtual void EndInvoke(FieldResolver Variant)
        {
        }

        public virtual void Invoke(FieldResolver Variant);

    }



}
