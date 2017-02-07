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

        public Expression ParentNode
        {
            get { return _ParentNode; }
            set { this._ParentNode = value; }
        }

        public List<Expression> ChildNodes
        {
            get { return _ChildNodes; }
        }

        public ExpressionAffinity Affinity
        {
            get { return _Affinity; }
        }

        public bool IsMaster
        {
            get { return _ParentNode == null; }
        }

        public bool IsTerminal
        {
            get { return this.ChildNodes.Count == 0; }
        }

        public bool IsQuasiTerminal
        {
            get
            {
                if (this.IsTerminal) return false;
                return this.ChildNodes.TrueForAll((n) => { return n.IsTerminal; });
            }
        }

        public Guid NodeID
        {
            get { return this._UID; }
        }

        public string Name
        {
            get { return this._name; }
            protected set { this._name = value; }
        }

        public Expression this[int IndexOf]
        {
            get { return this._ChildNodes[IndexOf]; }
        }

        // Methods //
        public void AddChildNode(Expression Node)
        {
            Node.ParentNode = this;
            this._ChildNodes.Add(Node);
        }

        public void AddChildren(params Expression[] Nodes)
        {
            foreach (Expression n in Nodes)
                this.AddChildNode(n);
        }

        public void Deallocate()
        {
            if (this.IsMaster) return;
            this.ParentNode.ChildNodes.Remove(this);
        }

        public void Deallocate(Expression Node)
        {
            if (this.IsTerminal) return;
            this._ChildNodes.Remove(Node);
        }

        // Size and Affinities //
        public CellAffinity[] ReturnAffinityChildren(FieldResolver Variants)
        {
            List<CellAffinity> c = new List<CellAffinity>();
            foreach (Expression x in _ChildNodes)
                c.Add(x.ReturnAffinity(Variants));
            return c.ToArray();
        }

        public int[] ReturnSizeChildren(FieldResolver Variants)
        {

            List<int> c = new List<int>();
            foreach (Expression x in _ChildNodes)
                c.Add(x.ExpressionSize(Variants));
            return c.ToArray();

        }

        // Abstracts //
        public abstract string Unparse(Schema S);

        public abstract Expression CloneOfMe();

        public abstract Cell Evaluate(FieldResolver Variants);

        public abstract CellAffinity ReturnAffinity(FieldResolver Variants);

        public abstract int ExpressionSize(FieldResolver Variants);

        // Virtuals //
        public virtual bool IsVolatile
        {
            get { return false; }
        }

        // Opperators //
        //public static Expression operator +(Expression Left, Expression Right)
        //{
        //    ExpressionResult t = new ExpressionResult(null, new CellBinPlus());
        //    t.AddChildren(Left, Right);
        //    return t;
        //}

        //public static Expression operator -(Expression Left, Expression Right)
        //{
        //    ExpressionResult t = new ExpressionResult(null, new CellBinMinus());
        //    t.AddChildren(Left, Right);
        //    return t;
        //}

        //public static Expression operator *(Expression Left, Expression Right)
        //{
        //    ExpressionResult t = new ExpressionResult(null, new CellBinMult());
        //    t.AddChildren(Left, Right);
        //    return t;
        //}

        //public static Expression operator /(Expression Left, Expression Right)
        //{
        //    ExpressionResult t = new ExpressionResult(null, new CellBinDiv());
        //    t.AddChildren(Left, Right);
        //    return t;
        //}

        //public static Expression operator %(Expression Left, Expression Right)
        //{
        //    ExpressionResult t = new ExpressionResult(null, new CellBinMod());
        //    t.AddChildren(Left, Right);
        //    return t;
        //}

        //public static Expression operator ^(Expression Left, Expression Right)
        //{
        //    ExpressionResult t = new ExpressionResult(null, new CellFuncFVPower());
        //    t.AddChildren(Left, Right);
        //    return t;
        //}

        //public static int DecompileToFieldRef(Expression E)
        //{
            
        //    if (E == null)
        //        return -1;
        //    if (E is ExpressionFieldRef)
        //        return (E as ExpressionFieldRef).Index;
        //    return -1;

        //}

    }

    public abstract class ExpressionFunction : Expression
    {

        private int _MaxParamterCount;
        private bool _DynamicReturn;
        private CellAffinity _ReturnAffinity;

        public ExpressionFunction(Expression Parent, string Name, int ParameterCount)
            : base(Parent, ExpressionAffinity.Function)
        {
            this.Name = Name;
            this._MaxParamterCount = ParameterCount;
            this._DynamicReturn = true;
        }

        public ExpressionFunction(Expression Parent, string Name, int ParameterCount, CellAffinity ReturnAffinity)
            : base(Parent, ExpressionAffinity.Function)
        {
            this.Name = Name;
            this._MaxParamterCount = ParameterCount;
            this._ReturnAffinity = ReturnAffinity;
            this._DynamicReturn = false;
        }

        /// <summary>
        /// Returns the cell affinity
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override CellAffinity ReturnAffinity(FieldResolver Variants)
        {
            if (this._DynamicReturn)
                return CellAffinityHelper.Highest(this.ReturnAffinityChildren(Variants));
            return this._ReturnAffinity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override int ExpressionSize(FieldResolver Variants)
        {
            return this.ReturnSizeChildren(Variants).Max();
        }

        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------
        
        // Unary Opperations //
        public const string NAME_NOT = "NOT";
        public const string NAME_PLUS = "PLUS";
        public const string NAME_MINUS = "MINUS";
        
        // Binary Opperations //
        public const string NAME_ADD = "ADD";
        public const string NAME_SUB = "SUB";
        public const string NAME_MULT = "MULT";
        public const string NAME_DIV = "DIV";
        public const string NAME_CDIV = "CDIV";
        public const string NAME_MOD = "MOD";
        public const string NAME_POWER = "POWER";
        public const string NAME_EQ = "EQ";
        public const string NAME_NEQ = "NEQ";
        public const string NAME_GT = "GT";
        public const string NAME_GTE = "GTE";
        public const string NAME_LT = "LT";
        public const string NAME_LTE = "LTE";
        public const string NAME_AND = "AND";
        public const string NAME_OR = "OR";
        public const string NAME_XOR = "XOR";

        // Special Opperations //
        public const string NAME_IF = "IF";
        public const string NAME_IFNULL = "IF_NULL";
        public const string NAME_CAST = "CAST";

        // Special Functions //
        public const string NAME_LIKE = "LIKE";
        public const string NAME_MATCH = "MATCH";
        public const string NAME_GUID = "GUID";
        public const string NAME_THREAD_ID = "THREAD_ID";
        public const string NAME_RAND_BOOL = "RAND_BOOL";
        public const string NAME_RAND_DATE = "RAND_DATE";
        public const string NAME_RAND_INT = "RAND_INT";
        public const string NAME_RAND_NUM = "RAND_NUM";
        public const string NAME_RAND_BLOB = "RAND_BLOB";
        public const string NAME_RAND_STRING = "RAND_STRING";
        
        // Date Functions //
        public const string NAME_DATE_BUILD = "DATE_BUILD";
        public const string NAME_NOW = "NOW";
        public const string NAME_YEAR = "YEAR";
        public const string NAME_MONTH = "MONTH";
        public const string NAME_DAY = "DAY";
        public const string NAME_HOUR = "HOUR";
        public const string NAME_MINUTE = "MINUTE";
        public const string NAME_SECOND = "SECOND";
        public const string NAME_MILISECOND = "MILISECOND";
        public const string NAME_TICKS = "TICKS";
        
        // String Functions //
        public const string NAME_SUBSTR = "SUBSTR";
        public const string NAME_REPLACE = "REPLACE";
        public const string NAME_POSITION = "POSITION";
        public const string NAME_LENGTH = "LENGTH";
        public const string NAME_TRIM = "TRIM";
        public const string NAME_TO_UTF16 = "TO_UTF16";
        public const string NAME_TO_UTF8 = "TO_UTF8";
        public const string NAME_TO_HEX = "TO_HEX";
        public const string NAME_FROM_UTF16 = "FROM_UTF16";
        public const string NAME_FROM_UTF8 = "FROM_UTF8";
        public const string NAME_FROM_HEX = "FROM_HEX";

        // Numerics //
        public const string NAME_LOG = "LOG";
        public const string NAME_EXP = "EXP";
        public const string NAME_SQRT = "SQRT";
        public const string NAME_SIN = "SIN";
        public const string NAME_COS = "COS";
        public const string NAME_TAN = "TAN";
        public const string NAME_SINH = "SINH";
        public const string NAME_COSH = "COSH";
        public const string NAME_TANH = "TANH";
        public const string NAME_LOGIT = "LOGIT";
        public const string NAME_NORMAL = "NORMAL";
        public const string NAME_ABS = "ABS";
        public const string NAME_SIGN = "SIGN";
        public const string NAME_ROUND = "ROUND";
        
        // Uninary Opperations //
        public class ExpressionNot : ExpressionFunction
        {

            public ExpressionNot()
                : base(null, NAME_NOT, 1)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionNot();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return !this._ChildNodes[0].Evaluate(Variants);

            }

        }

        public class ExpressionPlus : ExpressionFunction
        {

            public ExpressionPlus()
                : base(null, NAME_ADD, 1)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionPlus();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return +this._ChildNodes[0].Evaluate(Variants);

            }

        }

        public class ExpressionMinus : ExpressionFunction
        {

            public ExpressionMinus()
                : base(null, NAME_MINUS, 1)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionMinus();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return -this._ChildNodes[0].Evaluate(Variants);

            }

        }

        // Binary Opperations //
        public class ExpressionAdd : ExpressionFunction
        {

            public ExpressionAdd()
                : base(null, NAME_ADD , 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionAdd();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) + this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionSubtract : ExpressionFunction
        {

            public ExpressionSubtract()
                : base(null, NAME_SUB, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionSubtract();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) - this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionMultiply : ExpressionFunction
        {

            public ExpressionMultiply()
                : base(null, NAME_MULT, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionMultiply();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) * this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionDivide : ExpressionFunction
        {

            public ExpressionDivide()
                : base(null, NAME_DIV, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionDivide();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) / this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionCheckedDivide : ExpressionFunction
        {

            public ExpressionCheckedDivide()
                : base(null, NAME_CDIV, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionCheckedDivide();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.CheckDivide(this._ChildNodes[0].Evaluate(Variants), this._ChildNodes[1].Evaluate(Variants));

            }

        }

        public class ExpressionModulo : ExpressionFunction
        {

            public ExpressionModulo()
                : base(null, NAME_MOD, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionModulo();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) % this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionPower : ExpressionFunction
        {

            public ExpressionPower()
                : base(null, NAME_POWER, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionPower();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Power(this._ChildNodes[0].Evaluate(Variants) , this._ChildNodes[1].Evaluate(Variants));

            }

        }

        public class ExpressionEquals : ExpressionFunction
        {

            public ExpressionEquals()
                : base(null, NAME_EQ, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionEquals();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) == this._ChildNodes[1].Evaluate(Variants) ? Cell.TRUE : Cell.FALSE;

            }

        }

        public class ExpressionNotEquals : ExpressionFunction
        {

            public ExpressionNotEquals()
                : base(null, NAME_EQ, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionNotEquals();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) != this._ChildNodes[1].Evaluate(Variants) ? Cell.TRUE : Cell.FALSE;

            }

        }

        public class ExpressionLessThan : ExpressionFunction
        {

            public ExpressionLessThan()
                : base(null, NAME_LT, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionLessThan();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) < this._ChildNodes[1].Evaluate(Variants) ? Cell.TRUE : Cell.FALSE;

            }

        }

        public class ExpressionLessThanOrEqualTo : ExpressionFunction
        {

            public ExpressionLessThanOrEqualTo()
                : base(null, NAME_LTE, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionLessThanOrEqualTo();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) <= this._ChildNodes[1].Evaluate(Variants) ? Cell.TRUE : Cell.FALSE;

            }

        }

        public class ExpressionGreaterThan : ExpressionFunction
        {

            public ExpressionGreaterThan()
                : base(null, NAME_GT, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionGreaterThan();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) > this._ChildNodes[1].Evaluate(Variants) ? Cell.TRUE : Cell.FALSE;

            }

        }

        public class ExpressionGreaterThanOrEqualTo : ExpressionFunction
        {

            public ExpressionGreaterThanOrEqualTo()
                : base(null, NAME_GTE, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionGreaterThanOrEqualTo();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) >= this._ChildNodes[1].Evaluate(Variants) ? Cell.TRUE : Cell.FALSE;

            }

        }

        public class ExpressionAnd : ExpressionFunction
        {

            public ExpressionAnd()
                : base(null, NAME_AND, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionAnd();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) & this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionOr : ExpressionFunction
        {

            public ExpressionOr()
                : base(null, NAME_OR, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionOr();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) | this._ChildNodes[1].Evaluate(Variants);

            }

        }

        public class ExpressionXor : ExpressionFunction
        {

            public ExpressionXor()
                : base(null, NAME_XOR, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionXor();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return this._ChildNodes[0].Evaluate(Variants) ^ this._ChildNodes[1].Evaluate(Variants);

            }

        }

        // Special Opperations //
        public class ExpressionIf : ExpressionFunction
        {

            public ExpressionIf()
                : base(null, NAME_IF, 3)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionIf();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.If(this._ChildNodes[0].Evaluate(Variants), this._ChildNodes[1].Evaluate(Variants), this._ChildNodes[2].Evaluate(Variants));

            }

        }

        public class ExpressionIfNull : ExpressionFunction
        {

            public ExpressionIfNull()
                : base(null, NAME_IFNULL, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionIfNull();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell x = this._ChildNodes[0].Evaluate(Variants);
                return x.IsNull ? this._ChildNodes[1].Evaluate(Variants) : x;

            }

        }

        public class ExpressionCast : ExpressionFunction
        {

            public ExpressionCast()
                : base(null, NAME_CAST, 2)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionCast();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                CellAffinity t = (CellAffinity)this._ChildNodes[1].Evaluate(Variants).INT;
                return Cell.Cast(this._ChildNodes[0].Evaluate(Variants), t);

            }

        }

        // Special Functions //
        public class ExpressionLike : ExpressionFunction
        {

            public const char WILD_CARD = '*';

            public ExpressionLike()
                : base(null, NAME_LIKE, 2, CellAffinity.BOOL)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionLike();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell cText = this._ChildNodes[0].Evaluate(Variants);
                Cell cPatern = this._ChildNodes[1].Evaluate(Variants);
                string Text = cText.valueSTRING;
                string Patern = cPatern.valueSTRING;

                if (cText.IsNull || cPatern.IsNull)
                    return Cell.FALSE;

                bool x = false, y = false, z = false;

                if (Patern.First() == WILD_CARD)
                {
                    Patern = Patern.Remove(0, 1);
                    x = true;
                }

                if (Patern.Last() == WILD_CARD)
                {
                    Patern = Patern.Remove(Patern.Length - 1, 1);
                    y = true;
                }

                if (x && y) // '*Hello World*' //
                {
                    z = Text.ToUpper().Contains(Patern.ToUpper());
                }
                else if (x && !y) // '*Hello World' //
                {
                    z = Text.EndsWith(Patern, StringComparison.OrdinalIgnoreCase);
                }
                else if (!x && y) // 'Hello World*' //
                {
                    z = Text.StartsWith(Patern, StringComparison.OrdinalIgnoreCase);
                }
                else // !OriginalNode && !NewNode // 'Hello World' //
                {
                    z = string.Equals(Text, Patern, StringComparison.OrdinalIgnoreCase);
                }

                return new Cell(z);

            }

        }

        public class ExpressionMatch : ExpressionFunction
        {

            public ExpressionMatch()
                : base(null, NAME_MATCH, -128, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionMatch();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count <= 1)
                    return Cell.NULL_INT;

                Cell x = this._ChildNodes[0].Evaluate(Variants);
                for (int i = 1; i < this._ChildNodes.Count; i++)
                {

                    Cell y = this._ChildNodes[i].Evaluate(Variants);
                    if (x == y)
                        return new Cell(i - 1);

                }

                return Cell.NULL_INT;

            }

        }

        public class ExpressionGUID : ExpressionFunction
        {

            public ExpressionGUID()
                : base(null, NAME_GUID, 0, CellAffinity.BLOB)
            {   
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionGUID();
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(Guid.NewGuid().ToByteArray());
            }

        }

        public class ExpressionThreadID : ExpressionFunction
        {

            public ExpressionThreadID()
                : base(null, NAME_THREAD_ID, 0, CellAffinity.INT)
            {
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionThreadID();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(System.Threading.Thread.CurrentThread.ManagedThreadId);
            }

        }

        public class ExpressionRandomBool : ExpressionFunction
        {

            private RandomCell _Generator;

            public ExpressionRandomBool(RandomCell Generator)
                : base(null, NAME_RAND_BOOL, -128, CellAffinity.BOOL)
            {
                this._Generator = Generator;
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionRandomBool(this._Generator);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                    return this._Generator.NextBool();

                double p = this._ChildNodes[0].Evaluate(Variants).valueDOUBLE;
                return this._Generator.NextBool(p);

            }

        }

        public class ExpressionRandomDate : ExpressionFunction
        {

            private RandomCell _Generator;

            public ExpressionRandomDate(RandomCell Generator)
                : base(null, NAME_RAND_DATE, -128, CellAffinity.DATE_TIME)
            {
                this._Generator = Generator;
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionRandomDate(this._Generator);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                    return this._Generator.NextDate();

                DateTime min = (this._ChildNodes.Count >= 1 ? this._ChildNodes[0].Evaluate(Variants).valueDATE_TIME : DateTime.MinValue);
                DateTime max = (this._ChildNodes.Count >= 2 ? this._ChildNodes[1].Evaluate(Variants).valueDATE_TIME : DateTime.MaxValue);

                return this._Generator.NextDate(min, max);

            }

        }

        public class ExpressionRandomInt : ExpressionFunction
        {

            private RandomCell _Generator;

            public ExpressionRandomInt(RandomCell Generator)
                : base(null, NAME_RAND_INT, -128, CellAffinity.INT)
            {
                this._Generator = Generator;
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionRandomInt(this._Generator);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                    return this._Generator.NextLong();

                long min = (this._ChildNodes.Count >= 1 ? this._ChildNodes[0].Evaluate(Variants).valueINT : 0);
                long max = (this._ChildNodes.Count >= 2 ? this._ChildNodes[1].Evaluate(Variants).valueINT : long.MaxValue);

                return this._Generator.NextLong(min, max);

            }

        }

        public class ExpressionRandomNum : ExpressionFunction
        {

            private RandomCell _Generator;

            public ExpressionRandomNum(RandomCell Generator)
                : base(null, NAME_RAND_NUM, -128, CellAffinity.DOUBLE)
            {
                this._Generator = Generator;
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionRandomNum(this._Generator);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                    return this._Generator.NextDouble();

                double min = (this._ChildNodes.Count >= 1 ? this._ChildNodes[0].Evaluate(Variants).valueDOUBLE : 0D);
                double max = (this._ChildNodes.Count >= 2 ? this._ChildNodes[1].Evaluate(Variants).valueDOUBLE : 1D);

                if (min == max && min == 0)
                    return this._Generator.NextDoubleGauss();

                return this._Generator.NextDouble(min, max);

            }

        }

        public class ExpressionRandomBLOB : ExpressionFunction
        {

            private RandomCell _Generator;

            public ExpressionRandomBLOB(RandomCell Generator)
                : base(null, NAME_RAND_NUM, -128, CellAffinity.BLOB)
            {
                this._Generator = Generator;
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionRandomBLOB(this._Generator);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                    return this._Generator.NextBLOB(32);

                int Length = (int)this._ChildNodes[0].Evaluate(Variants).valueINT;

                return this._Generator.NextBLOB(Length);

            }

        }

        public class ExpressionRandomString : ExpressionFunction
        {

            private RandomCell _Generator;

            public ExpressionRandomString(RandomCell Generator)
                : base(null, NAME_RAND_NUM, -128, CellAffinity.STRING)
            {
                this._Generator = Generator;
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionRandomString(this._Generator);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                {
                    return this._Generator.NextStringASCIIPrintable(16);
                }
                else if (this._ChildNodes.Count == 1)
                {
                    int Length = (int)this._ChildNodes[0].Evaluate(Variants).valueINT;
                    return this._Generator.NextStringASCIIPrintable(Length);
                }
                else
                {
                    int Length = (int)this._ChildNodes[0].Evaluate(Variants).valueINT;
                    string Corpus = this._ChildNodes[1].Evaluate(Variants).valueSTRING;
                    return this._Generator.NextString(Length, Corpus);
                }


            }

        }

        // Date Functions //
        public class ExpressionDateBuild : ExpressionFunction
        {

            public ExpressionDateBuild()
                : base(null, NAME_DATE_BUILD, -7, CellAffinity.DATE_TIME)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionDateBuild();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count == 0)
                    return Cell.NULL_DATE;

                int[] xdate = new int[7];

                for (int i = 0; i < 7; i++)
                {

                    if (this._ChildNodes.Count >= i)
                    {
                        xdate[i] = (int)this._ChildNodes[i].Evaluate(Variants).valueINT;
                    }

                }

                DateTime x = new DateTime(xdate[0], xdate[1], xdate[2], xdate[3], xdate[4], xdate[5], xdate[6]);

                return new Cell(x);

            }

        }

        public class ExpressionNow : ExpressionFunction
        {

            public ExpressionNow()
                : base(null, NAME_NOW, 0, CellAffinity.DATE_TIME)
            {
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionDateBuild();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(DateTime.Now);
            }

        }

        public class ExpressionTicks : ExpressionFunction
        {

            public ExpressionTicks()
                : base(null, NAME_TICKS, 0, CellAffinity.INT)
            {
            }

            public override bool IsVolatile
            {
                get { return true; }
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionTicks();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(DateTime.Now.Ticks);
            }

        }

        public class ExpressionYear : ExpressionFunction
        {

            public ExpressionYear()
                : base(null, NAME_YEAR, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionYear();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Year(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        public class ExpressionMonth : ExpressionFunction
        {

            public ExpressionMonth()
                : base(null, NAME_MONTH, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionMonth();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Month(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        public class ExpressionDay : ExpressionFunction
        {

            public ExpressionDay()
                : base(null, NAME_DAY, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionDay();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Day(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        public class ExpressionHour : ExpressionFunction
        {

            public ExpressionHour()
                : base(null, NAME_HOUR, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionHour();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Hour(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        public class ExpressionMinute : ExpressionFunction
        {

            public ExpressionMinute()
                : base(null, NAME_MINUTE, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionMinute();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Minute(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        public class ExpressionSecond : ExpressionFunction
        {

            public ExpressionSecond()
                : base(null, NAME_SECOND, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionSecond();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Second(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        public class ExpressionMillisecond : ExpressionFunction
        {

            public ExpressionMillisecond()
                : base(null, NAME_MILISECOND, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionMillisecond();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                return Cell.Millisecond(this._ChildNodes[0].Evaluate(Variants));

            }

        }

        // Strings //
        /*
            public const string NAME_SUBSTR = "SUBSTR";
            public const string NAME_REPLACE = "REPLACE";
            public const string NAME_POSITION = "POSITION";
            public const string NAME_LENGTH = "LENGTH";
            public const string NAME_TRIM = "TRIM";
            public const string NAME_TO_UTF16 = "TO_UTF16";
            public const string NAME_TO_UTF8 = "TO_UTF8";
            public const string NAME_TO_HEX = "TO_HEX";
            public const string NAME_FROM_UTF16 = "FROM_UTF16";
            public const string NAME_FROM_UTF8 = "FROM_UTF8";
            public const string NAME_FROM_HEX = "FROM_HEX";
         */
        public class ExpressionSubstring : ExpressionFunction
        {

            public ExpressionSubstring()
                : base(null, NAME_SUBSTR, 3)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionSubstring();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell value = this._ChildNodes[0].Evaluate(Variants);
                Cell start = this._ChildNodes[1].Evaluate(Variants);
                Cell length = this._ChildNodes[2].Evaluate(Variants);

                return Cell.Substring(value, start.valueINT, length.valueINT);

            }

        }

        public class ExpressionReplace : ExpressionFunction
        {

            public ExpressionReplace()
                : base(null, NAME_REPLACE, 3)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionReplace();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell value = this._ChildNodes[0].Evaluate(Variants);
                Cell pattern = this._ChildNodes[1].Evaluate(Variants);
                Cell new_pattern = this._ChildNodes[2].Evaluate(Variants);

                return Cell.Replace(value, pattern, new_pattern);

            }

        }

        public class ExpressionPosition : ExpressionFunction
        {

            public ExpressionPosition()
                : base(null, NAME_POSITION, -2, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionPosition();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count < Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell value = this._ChildNodes[0].Evaluate(Variants);
                Cell pattern = this._ChildNodes[1].Evaluate(Variants);
                int start = (this._ChildNodes.Count >= 3 ? (int)this._ChildNodes[2].Evaluate(Variants).valueINT : 0);

                return Cell.Position(value, pattern, start);

            }

        }

        public class ExpressionLength : ExpressionFunction
        {

            public ExpressionLength()
                : base(null, NAME_LENGTH, 1, CellAffinity.INT)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionLength();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell x = this._ChildNodes[0].Evaluate(Variants);
                if (x.IsNull)
                    return Cell.NULL_INT;

                if (x.AFFINITY == CellAffinity.BLOB)
                {
                    return new Cell(x.BLOB.Length);
                }
                else
                {
                    return new Cell(x.valueSTRING.Length);
                }

            }

        }

        public class ExpressionTrim : ExpressionFunction
        {

            public ExpressionTrim()
                : base(null, NAME_TRIM, 1)
            {
            }

            public override Expression CloneOfMe()
            {
                return new ExpressionTrim();
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._ChildNodes.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                Cell x = this._ChildNodes[0].Evaluate(Variants);
                if (x.IsNull)
                    return new Cell(this.ReturnAffinity(Variants));

                return Cell.Trim(x);

            }

        }



    }



}
