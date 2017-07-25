using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.MatrixExpressions
{

    public abstract class MatrixExpression : IBindable
    {

        private MatrixExpression _ParentNode;
        protected List<MatrixExpression> _Children;

        public MatrixExpression(MatrixExpression Parent)
        {
            this._ParentNode = Parent;
            this._Children = new List<MatrixExpression>();
        }

        public MatrixExpression ParentNode
        {
            get { return _ParentNode; }
            set { this._ParentNode = value; }
        }

        public bool IsMaster
        {
            get { return _ParentNode == null; }
        }

        public bool IsTerminal
        {
            get { return this.Children.Count == 0; }
        }

        public bool IsQuasiTerminal
        {
            get
            {
                if (this.IsTerminal) return false;
                return this.Children.TrueForAll((n) => { return n.IsTerminal; });
            }
        }

        public MatrixExpression this[int IndexOf]
        {
            get { return this._Children[IndexOf]; }
        }

        // Methods //
        public void AddChildNode(MatrixExpression Node)
        {
            Node.ParentNode = this;
            this._Children.Add(Node);
        }

        public void AddChildren(params MatrixExpression[] Nodes)
        {
            foreach (MatrixExpression n in Nodes)
                this.AddChildNode(n);
        }

        public List<MatrixExpression> Children
        {
            get { return _Children; }
        }

        public CellMatrix[] EvaluateChildren(FieldResolver Variant)
        {
            List<CellMatrix> c = new List<CellMatrix>();
            foreach (MatrixExpression x in _Children)
                c.Add(x.Evaluate(Variant));
            return c.ToArray();
        }

        public CellAffinity[] ReturnAffinityChildren()
        {
            List<CellAffinity> c = new List<CellAffinity>();
            foreach (MatrixExpression x in _Children)
                c.Add(x.ReturnAffinity());
            return c.ToArray();
        }

        public void Deallocate()
        {
            if (this.IsMaster) return;
            this.ParentNode.Children.Remove(this);
        }

        // Abstracts //
        public abstract CellMatrix Evaluate(FieldResolver Variant);

        public abstract CellAffinity ReturnAffinity();

        public abstract MatrixExpression CloneOfMe();

        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._Children.ForEach((x) => { x.Bind(PointerRef, Value); });
        }


    }

}
