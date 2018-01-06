using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public abstract class MatrixExpression : IBindable, IExpression
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
        public abstract int ReturnSize();

        public abstract CellMatrix Evaluate(FieldResolver Variant);

        public abstract CellAffinity ReturnAffinity();

        public abstract MatrixExpression CloneOfMe();

        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._Children.ForEach((x) => { x.Bind(PointerRef, Value); });
        }

        public SuperExpressionAffinity SuperAffinity { get { return SuperExpressionAffinity.Matrix; } }

        public ScalarExpression Scalar { get { return null; } }

        public MatrixExpression Matrix { get { return this; } }

        public RecordExpression Record { get { return null; } }

        public TableExpression Table { get { return null; } }


        // Statics //
        public static MatrixExpression Empty
        {
            get 
            {
                CellMatrix m = new CellMatrix(0, 0, CellValues.NullLONG);
                return new MatrixExpressionLiteral(null, m);
            }
        }

        public static MatrixExpression Null
        {
            get
            {
                CellMatrix m = new CellMatrix(1, 1, CellValues.NullLONG);
                return new MatrixExpressionLiteral(null, m);
            }
        }


    }

    public abstract class MatrixExpressionFunction : MatrixExpression
    {

        protected string _Name;
        protected int _ParamCount = -1;
        protected List<Parameter> _Parameters;
        protected CellAffinity _FunctionReturnAffinity;

        public MatrixExpressionFunction(MatrixExpression Parent, string Name, int Parameters, CellAffinity FunctionReturnAffinity)
            : base(Parent)
        {
            this._Name = Name;
            this._ParamCount = Parameters;
            this._Parameters = new List<Parameter>();
            this._FunctionReturnAffinity = FunctionReturnAffinity;
        }

        public virtual bool IsVolatile
        {
            get { return true; }
        }

        public string FunctionName
        {
            get { return this._Name; }
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._FunctionReturnAffinity;
        }

        public int ParameterCount
        {
            get { return this._ParamCount; }
        }

        public void AddParameter(Parameter Value)
        {
            this._Parameters.Add(Value);
        }

        public void CheckParameters()
        {
            
            if (this._ParamCount < 0 && this._Parameters.Count > (-this._ParamCount))
            {
                throw new Exception(string.Format("Function '{0}' can have at most '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }
            else if (this._Parameters.Count != this._ParamCount)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }

        }

    }

}
