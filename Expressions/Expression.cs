using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    /// <summary>
    /// Represents the base class for all expressions
    /// </summary>
    public abstract class Expression
    {

        protected Expression _ParentNode;
        protected List<Expression> _ChildNodes;
        protected ExpressionAffinity _Affinity;
        protected Guid _UID;
        protected string _name;

        public Expression(Expression Parent, ExpressionAffinity Affinity)
        {
            this._ParentNode = Parent;
            this._Affinity = Affinity;
            this._ChildNodes = new List<Expression>();
            this._UID = Guid.NewGuid();
            this._name = Affinity.ToString();
        }

        /// <summary>
        /// The parent node in the expression tree
        /// </summary>
        public Expression ParentNode
        {
            get { return _ParentNode; }
            set { this._ParentNode = value; }
        }

        /// <summary>
        /// All child nodes
        /// </summary>
        public List<Expression> ChildNodes
        {
            get { return _ChildNodes; }
        }

        /// <summary>
        /// Gets the return affinity
        /// </summary>
        public ExpressionAffinity Affinity
        {
            get { return _Affinity; }
        }

        /// <summary>
        /// True if this node has no parent (top of the chain)
        /// </summary>
        public bool IsMaster
        {
            get { return _ParentNode == null; }
        }

        /// <summary>
        /// True if this node is has no children (bottom of the chain)
        /// </summary>
        public bool IsTerminal
        {
            get { return this.ChildNodes.Count == 0; }
        }

        /// <summary>
        /// True if the node's children are all terminal
        /// </summary>
        public bool IsQuasiTerminal
        {
            get
            {
                if (this.IsTerminal) return false;
                return this.ChildNodes.TrueForAll((n) => { return n.IsTerminal; });
            }
        }

        /// <summary>
        /// Gets a unique node ID for debugging
        /// </summary>
        public Guid NodeID
        {
            get { return this._UID; }
        }

        /// <summary>
        /// Gets an optional name parameter for debugging
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Gets or sets the nth child node
        /// </summary>
        /// <param name="IndexOf"></param>
        /// <returns></returns>
        public Expression this[int IndexOf]
        {
            get { return this._ChildNodes[IndexOf]; }
        }

        // Methods //
        /// <summary>
        /// Adds a child node
        /// </summary>
        /// <param name="Node"></param>
        public void AddChildNode(Expression Node)
        {
            Node.ParentNode = this;
            this._ChildNodes.Add(Node);
        }

        /// <summary>
        /// Adds many child nodes
        /// </summary>
        /// <param name="Nodes"></param>
        public void AddChildren(params Expression[] Nodes)
        {
            foreach (Expression n in Nodes)
                this.AddChildNode(n);
        }

        /// <summary>
        /// Removes all child nodes
        /// </summary>
        public void Deallocate()
        {
            if (this.IsMaster) return;
            this.ParentNode.ChildNodes.Remove(this);
        }

        /// <summary>
        /// Removes a single child node
        /// </summary>
        /// <param name="Node"></param>
        public void Deallocate(Expression Node)
        {
            if (this.IsTerminal) return;
            this._ChildNodes.Remove(Node);
        }

        // Size and Affinities //
        /// <summary>
        /// Gets all the child return affinitys
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public CellAffinity[] ReturnAffinityChildren()
        {
            List<CellAffinity> c = new List<CellAffinity>();
            foreach (Expression x in _ChildNodes)
                c.Add(x.ExpressionReturnAffinity());
            return c.ToArray();
        }

        /// <summary>
        /// Gets all the child data sizes
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public int[] ReturnSizeChildren()
        {

            List<int> c = new List<int>();
            foreach (Expression x in _ChildNodes)
                c.Add(x.ExpressionSize());
            return c.ToArray();

        }

        // Abstracts //
        /// <summary>
        /// Decompiles the expression
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public abstract string Unparse(FieldResolver Variants);

        /// <summary>
        /// Gets a clone of the expression
        /// </summary>
        /// <returns></returns>
        public abstract Expression CloneOfMe();

        /// <summary>
        /// Evaluates the expression
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract Cell Evaluate(FieldResolver Variants);

        /// <summary>
        /// Gets the return affinity
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract CellAffinity ExpressionReturnAffinity();

        /// <summary>
        /// Gets the return size
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract int ExpressionSize();

        // Virtuals //
        /// <summary>
        /// True if the expression returns different values with two different calls, all else the same
        /// </summary>
        public virtual bool IsVolatile
        {
            get { return false; }
        }

        // Opperators //
        public static Expression operator +(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionAdd();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression operator -(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionSubtract();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression operator *(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionMultiply();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression operator /(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionDivide();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression operator %(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionModulo();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression CDIV(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionCheckedDivide();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression EQ(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionEquals();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression NEQ(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionNotEquals();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression GT(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionGreaterThan();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression GTE(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionGreaterThanOrEqualTo();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression LT(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionLessThan();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression LTE(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionLessThanOrEqualTo();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression AND(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionAnd();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression OR(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionOr();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression XOR(Expression A, Expression B)
        {
            Expression x = new ExpressionFunction.ExpressionXor();
            x.AddChildren(A, B);
            return x;
        }

        public static Expression Value(bool Value)
        {
            return new ExpressionConstant(null, new Cell(Value));
        }

        public static Expression Value(DateTime Value)
        {
            return new ExpressionConstant(null, new Cell(Value));
        }

        public static Expression Value(long Value)
        {
            return new ExpressionConstant(null, new Cell(Value));
        }

        public static Expression Value(double Value)
        {
            return new ExpressionConstant(null, new Cell(Value));
        }

        public static Expression Value(byte[] Value)
        {
            return new ExpressionConstant(null, new Cell(Value));
        }

        public static Expression Value(string Value)
        {
            return new ExpressionConstant(null, new Cell(Value));
        }

        public static Expression Value(Cell Value)
        {
            return new ExpressionConstant(null, Value);
        }

        public static Expression Field(IColumns Schema, string Name, int ResolverOffset)
        {
            int FieldOffset = Schema.Columns.ColumnIndex(Name);
            CellAffinity Affinity = Schema.Columns.ColumnAffinity(FieldOffset);
            int Size = Schema.Columns.ColumnSize(FieldOffset);
            return new ExpressionFieldRef(null, ResolverOffset, FieldOffset, Affinity, Size);
        }

        // Constants //
        public static Expression True
        {
            get { return new ExpressionConstant(null, new Cell(true)); }
        }

        public static Expression False
        {
            get { return new ExpressionConstant(null, new Cell(false)); }
        }

        public static Expression ZeroNUM
        {
            get { return new ExpressionConstant(null, new Cell(0D)); }
        }

        public static Expression OneNUM
        {
            get { return new ExpressionConstant(null, new Cell(1D)); }
        }

        public static Expression ZeroINT
        {
            get { return new ExpressionConstant(null, new Cell(0)); }
        }

        public static Expression OneINT
        {
            get { return new ExpressionConstant(null, new Cell(1)); }
        }

        public static Expression Now
        {
            get { return new ExpressionConstant(null, new Cell(DateTime.Now)); }
        }


    }

}
