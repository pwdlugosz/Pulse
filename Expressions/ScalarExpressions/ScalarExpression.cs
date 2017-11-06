using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Expressions.ScalarExpressions
{

    /// <summary>
    /// Represents the base class for all expressions
    /// </summary>
    public abstract class ScalarExpression : IBindable
    {

        protected ScalarExpressionAffinity _Affinity;
        protected ScalarExpression _ParentNode;
        protected List<ScalarExpression> _ChildNodes;
        protected Guid _UID;
        protected string _name;

        public ScalarExpression(ScalarExpression Parent, ScalarExpressionAffinity Affinity)
        {
            this._Affinity = Affinity;
            this._name = Affinity.ToString();
            this._ParentNode = Parent;
            this._ChildNodes = new List<ScalarExpression>();
            this._UID = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the return affinity
        /// </summary>
        public ScalarExpressionAffinity Affinity
        {
            get { return _Affinity; }
        }

        /// <summary>
        /// The parent node in the expression tree
        /// </summary>
        public ScalarExpression ParentNode
        {
            get { return _ParentNode; }
            set { this._ParentNode = value; }
        }

        /// <summary>
        /// All child nodes
        /// </summary>
        public List<ScalarExpression> ChildNodes
        {
            get { return _ChildNodes; }
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
        public ScalarExpression this[int IndexOf]
        {
            get { return this._ChildNodes[IndexOf]; }
        }

        // Methods //
        /// <summary>
        /// Adds a child node
        /// </summary>
        /// <param name="OriginalNode"></param>
        public void AddChildNode(ScalarExpression Node)
        {
            Node.ParentNode = this;
            this._ChildNodes.Add(Node);
        }

        /// <summary>
        /// Adds many child nodes
        /// </summary>
        /// <param name="Nodes"></param>
        public void AddChildren(params ScalarExpression[] Nodes)
        {
            foreach (ScalarExpression n in Nodes)
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
        /// <param name="OriginalNode"></param>
        public void Deallocate(ScalarExpression Node)
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
            foreach (ScalarExpression x in _ChildNodes)
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
            foreach (ScalarExpression x in _ChildNodes)
                c.Add(x.ExpressionSize());
            return c.ToArray();

        }

        // Abstracts //
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract ScalarExpression CloneOfMe();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public abstract Cell Evaluate(FieldResolver Variants);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string Unparse(FieldResolver Variants);

        /// <summary>
        /// Binds an expression to the tree
        /// </summary>
        /// <param name="PointerRef"></param>
        /// <param name="Value"></param>
        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._ChildNodes.ForEach((x) => { x.Bind(PointerRef, Value); });
        }

        // Virtuals //
        /// <summary>
        /// True if the expression returns different values with two different calls, all else the same
        /// </summary>
        public virtual bool IsVolatile
        {
            get { return false; }
        }

        // Opperators //
        public static ScalarExpression operator +(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionAdd();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression operator -(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionSubtract();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression operator *(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionMultiply();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression operator /(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionDivide();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression operator %(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionModulo();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression CDIV(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionCheckedDivide();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression EQ(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionEquals();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression NEQ(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionNotEquals();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression GT(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionGreaterThan();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression GTE(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionGreaterThanOrEqualTo();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression LT(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionLessThan();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression LTE(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionLessThanOrEqualTo();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression AND(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionAnd();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression OR(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionOr();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression XOR(ScalarExpression A, ScalarExpression B)
        {
            ScalarExpression x = new ScalarExpressionFunction.ExpressionXor();
            x.AddChildren(A, B);
            return x;
        }

        public static ScalarExpression Value(bool Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(DateTime Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(long Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(double Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(byte[] Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(string Value)
        {
            return new ScalarExpressionConstant(null, new Cell(Value));
        }

        public static ScalarExpression Value(Cell Value)
        {
            return new ScalarExpressionConstant(null, Value);
        }

        public static ScalarExpression Value(CellAffinity Affinity)
        {
            return new ScalarExpressionConstant(null, new Cell(Affinity));
        }

        public static ScalarExpression Field(IColumns Schema, string Name, int ResolverOffset)
        {
            int FieldOffset = Schema.Columns.ColumnIndex(Name);
            CellAffinity Affinity = Schema.Columns.ColumnAffinity(FieldOffset);
            int Size = Schema.Columns.ColumnSize(FieldOffset);
            return new ScalarExpressionFieldRef(null, ResolverOffset, FieldOffset, Affinity, Size);
        }

        public static ScalarExpression HeapRef(Host Host, string LibName, string ValName)
        {
            
            if (!Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));
            if (!Host.Libraries[LibName].Values.Exists(ValName))
                throw new Exception(string.Format("Variable '{0}' does not exist in '{1}'", ValName, LibName));

            int HeapRef = Host.Libraries.GetPointer(LibName);
            int ValRef = Host.Libraries[LibName].Values.GetPointer(ValName);

            return new ScalarExpressionScalarRef(null, HeapRef, ValRef, Host.Libraries[HeapRef].Values[ValRef].Affinity, CellSerializer.Length(Host.Libraries[HeapRef].Values[ValRef]));

        }

        // Constants //
        public static ScalarExpression True
        {
            get { return new ScalarExpressionConstant(null, new Cell(true)); }
        }

        public static ScalarExpression False
        {
            get { return new ScalarExpressionConstant(null, new Cell(false)); }
        }

        public static ScalarExpression ZeroNUM
        {
            get { return new ScalarExpressionConstant(null, new Cell(0D)); }
        }

        public static ScalarExpression OneNUM
        {
            get { return new ScalarExpressionConstant(null, new Cell(1D)); }
        }

        public static ScalarExpression ZeroINT
        {
            get { return new ScalarExpressionConstant(null, new Cell(0)); }
        }

        public static ScalarExpression OneINT
        {
            get { return new ScalarExpressionConstant(null, new Cell(1)); }
        }

        public static ScalarExpression EmptyString
        {
            get { return new ScalarExpressionConstant(null, new Cell("")); }
        }

        public static ScalarExpression Now
        {
            get { return new ScalarExpressionConstant(null, new Cell(DateTime.Now)); }
        }

        public static ScalarExpression NullBool
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullBOOL); }
        }

        public static ScalarExpression NullInt
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullLONG); }
        }

        public static ScalarExpression NullNum
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullDOUBLE); }
        }

        public static ScalarExpression NullDate
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullDATE); }
        }

        public static ScalarExpression NullString
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullSTRING); }
        }

        public static ScalarExpression NullBLOB
        {
            get { return new ScalarExpressionConstant(null, CellValues.NullBLOB); }
        }
    }

}
