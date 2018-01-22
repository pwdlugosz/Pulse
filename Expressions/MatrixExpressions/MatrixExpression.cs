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

    public abstract class MatrixExpression
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

        // Statics //
        public static MatrixExpression Empty
        {
            get 
            {
                return null;
            }
        }

        public static MatrixExpression Null
        {
            get
            {
                return null;
            }
        }


    }

    public abstract class MatrixExpressionUnary : MatrixExpression
    {

        protected MatrixExpression _Value;
        protected string _Name;
        protected string _Token;

        public MatrixExpressionUnary(MatrixExpression Parent, string Name, string Token, MatrixExpression Value)
            : base(Parent)
        {
            this._Value = Value;
            this._Name = Name;
            this._Token = Token;
        }

        public override MatrixExpression CloneOfMe()
        {
            return null;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Value.ReturnAffinity();
        }

        public override int ReturnSize()
        {
            return this._Value.ReturnSize();
        }

        public sealed class MatrixExpressionTranspose : MatrixExpressionUnary
        {

            public MatrixExpressionTranspose(MatrixExpression Parent, MatrixExpression Value)
                : base(Parent, "TRANSPOSE", "~", Value)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._Value.Evaluate(Variant).Transposition;
            }

        }

        public sealed class MatrixExpressionInverse : MatrixExpressionUnary
        {

            public MatrixExpressionInverse(MatrixExpression Parent, MatrixExpression Value)
                : base(Parent, "INVERSE", "!!", Value)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._Value.Evaluate(Variant).Inverse;
            }

        }

    }

    public abstract class MatrixExpressionBinary : MatrixExpression
    {


        protected MatrixExpression _ValueA;
        protected MatrixExpression _ValueB;
        protected string _Name;
        protected string _Token;

        public MatrixExpressionBinary(MatrixExpression Parent, string Name, string Token, MatrixExpression ValueA, MatrixExpression ValueB)
            : base(Parent)
        {
            this._ValueA = ValueA;
            this._ValueB = ValueB;
            this._Name = Name;
            this._Token = Token;
        }

        public override MatrixExpression CloneOfMe()
        {
            return null;
        }

        public override CellAffinity ReturnAffinity()
        {
            return CellAffinityHelper.Highest(this._ValueA.ReturnAffinity(), this._ValueB.ReturnAffinity());
        }

        public override int ReturnSize()
        {
            CellAffinity a = this._ValueA.ReturnAffinity();
            CellAffinity b = this._ValueB.ReturnAffinity();
            if (a == b) return Math.Max(this._ValueA.ReturnSize(), this._ValueB.ReturnSize());
            if ((int)a > (int)b)
                return this._ValueA.ReturnSize();
            return this._ValueB.ReturnSize();
        }

        public sealed class MatrixExpressionBinaryAdd : MatrixExpressionBinary
        {

            public MatrixExpressionBinaryAdd(MatrixExpression Parent, MatrixExpression A, MatrixExpression B)
                : base(Parent, "ADD", "+", A, B)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._ValueA.Evaluate(Variant) + this._ValueA.Evaluate(Variant);
            }

        }

        public sealed class MatrixExpressionBinarySubtract : MatrixExpressionBinary
        {

            public MatrixExpressionBinarySubtract(MatrixExpression Parent, MatrixExpression A, MatrixExpression B)
                : base(Parent, "SUB", "-", A, B)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._ValueA.Evaluate(Variant) - this._ValueA.Evaluate(Variant);
            }

        }

        public sealed class MatrixExpressionBinaryMultiply : MatrixExpressionBinary
        {

            public MatrixExpressionBinaryMultiply(MatrixExpression Parent, MatrixExpression A, MatrixExpression B)
                : base(Parent, "MUL", "*", A, B)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._ValueA.Evaluate(Variant) * this._ValueA.Evaluate(Variant);
            }

        }

        public sealed class MatrixExpressionBinaryDivide : MatrixExpressionBinary
        {

            public MatrixExpressionBinaryDivide(MatrixExpression Parent, MatrixExpression A, MatrixExpression B)
                : base(Parent, "DIV", "/", A, B)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._ValueA.Evaluate(Variant) / this._ValueA.Evaluate(Variant);
            }

        }

        public sealed class MatrixExpressionBinaryCheckDivide : MatrixExpressionBinary
        {

            public MatrixExpressionBinaryCheckDivide(MatrixExpression Parent, MatrixExpression A, MatrixExpression B)
                : base(Parent, "CDIV", "/?", A, B)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return CellMatrix.CheckDivide(this._ValueA.Evaluate(Variant) , this._ValueA.Evaluate(Variant));
            }

        }

        public sealed class MatrixExpressionBinaryMMultiply : MatrixExpressionBinary
        {

            public MatrixExpressionBinaryMMultiply(MatrixExpression Parent, MatrixExpression A, MatrixExpression B)
                : base(Parent, "MMULT", "**", A, B)
            {
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {
                return this._ValueA.Evaluate(Variant) ^ this._ValueB.Evaluate(Variant);
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
            else if (this._Parameters.Count != this._ParamCount && this._ParamCount > 0)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }

        }

    }


    //public sealed class MatrixExpressionSubtractScalar : MatrixExpression
    //{

    //    private int _Association = 0; // 0 == left (A * B[]), 1 == right (B[] * A)
    //    private ScalarExpression _expression;

    //    public MatrixExpressionSubtractScalar(MatrixExpression Parent, ScalarExpression Expression, int Association)
    //        : base(Parent)
    //    {
    //        this._Association = Association;
    //        this._expression = Expression;
    //    }

    //    public override CellMatrix Evaluate(FieldResolver Variant)
    //    {
    //        if (this._Association == 0)
    //            return this._expression.Evaluate(Variant) - this[0].Evaluate(Variant);
    //        else
    //            return this[0].Evaluate(Variant) - this._expression.Evaluate(Variant);
    //    }

    //    public override CellAffinity ReturnAffinity()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnAffinity();
    //        else
    //            return this[0].ReturnAffinity();
    //    }

    //    public override int ReturnSize()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnSize();
    //        else
    //            return this[0].ReturnSize();
    //    }

    //    public override MatrixExpression CloneOfMe()
    //    {
    //        MatrixExpression node = new MatrixExpressionSubtractScalar(this.ParentNode, this._expression.CloneOfMe(), this._Association);
    //        foreach (MatrixExpression m in this._Children)
    //            node.AddChildNode(m.CloneOfMe());
    //        return node;
    //    }


    //}

    //public sealed class MatrixExpressionMultiplyScalar : MatrixExpression
    //{

    //    private int _Association = 0; // 0 == left (A * B[]), 1 == right (B[] * A)
    //    private ScalarExpression _expression;

    //    public MatrixExpressionMultiplyScalar(MatrixExpression Parent, ScalarExpression Expression, int Association)
    //        : base(Parent)
    //    {
    //        this._Association = Association;
    //        this._expression = Expression;
    //    }

    //    public override CellMatrix Evaluate(FieldResolver Variant)
    //    {
    //        if (this._Association == 0)
    //            return this._expression.Evaluate(Variant) * this[0].Evaluate(Variant);
    //        else
    //            return this[0].Evaluate(Variant) * this._expression.Evaluate(Variant);
    //    }

    //    public override CellAffinity ReturnAffinity()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnAffinity();
    //        else
    //            return this[0].ReturnAffinity();
    //    }

    //    public override int ReturnSize()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnSize();
    //        else
    //            return this[0].ReturnSize();
    //    }

    //    public override MatrixExpression CloneOfMe()
    //    {
    //        MatrixExpression node = new MatrixExpressionMultiplyScalar(this.ParentNode, this._expression.CloneOfMe(), this._Association);
    //        foreach (MatrixExpression m in this._Children)
    //            node.AddChildNode(m.CloneOfMe());
    //        return node;
    //    }

    //}

    //public sealed class MatrixExpressionCheckDivideScalar : MatrixExpression
    //{

    //    private int _Association = 0; // 0 == left (A * B[]), 1 == right (B[] * A)
    //    private ScalarExpression _expression;

    //    public MatrixExpressionCheckDivideScalar(MatrixExpression Parent, ScalarExpression Expression, int Association)
    //        : base(Parent)
    //    {
    //        this._Association = Association;
    //        this._expression = Expression;
    //    }

    //    public override CellMatrix Evaluate(FieldResolver Variant)
    //    {
    //        if (this._Association == 0)
    //            return CellMatrix.CheckDivide(this._expression.Evaluate(Variant), this[0].Evaluate(Variant));
    //        else
    //            return CellMatrix.CheckDivide(this[0].Evaluate(Variant), this._expression.Evaluate(Variant));
    //    }

    //    public override CellAffinity ReturnAffinity()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnAffinity();
    //        else
    //            return this[0].ReturnAffinity();
    //    }

    //    public override int ReturnSize()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnSize();
    //        else
    //            return this[0].ReturnSize();
    //    }

    //    public override MatrixExpression CloneOfMe()
    //    {
    //        MatrixExpression node = new MatrixExpressionCheckDivideScalar(this.ParentNode, this._expression.CloneOfMe(), this._Association);
    //        foreach (MatrixExpression m in this._Children)
    //            node.AddChildNode(m.CloneOfMe());
    //        return node;
    //    }

    //}

    //public sealed class MatrixExpressionDivideScalar : MatrixExpression
    //{

    //    private int _Association = 0; // 0 == left (A * B[]), 1 == right (B[] * A)
    //    private ScalarExpression _expression;

    //    public MatrixExpressionDivideScalar(MatrixExpression Parent, ScalarExpression Expression, int Association)
    //        : base(Parent)
    //    {
    //        this._Association = Association;
    //        this._expression = Expression;
    //    }

    //    public override CellMatrix Evaluate(FieldResolver Variant)
    //    {
    //        if (this._Association == 0)
    //            return this._expression.Evaluate(Variant) / this[0].Evaluate(Variant);
    //        else
    //            return this[0].Evaluate(Variant) / this._expression.Evaluate(Variant);
    //    }

    //    public override CellAffinity ReturnAffinity()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnAffinity();
    //        else
    //            return this[0].ReturnAffinity();
    //    }

    //    public override int ReturnSize()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnSize();
    //        else
    //            return this[0].ReturnSize();
    //    }

    //    public override MatrixExpression CloneOfMe()
    //    {
    //        MatrixExpression node = new MatrixExpressionDivideScalar(this.ParentNode, this._expression.CloneOfMe(), this._Association);
    //        foreach (MatrixExpression m in this._Children)
    //            node.AddChildNode(m.CloneOfMe());
    //        return node;
    //    }

    //}

    //public sealed class MatrixExpressionAddScalar : MatrixExpression
    //{

    //    private int _Association = 0; // 0 == left (A * B[]), 1 == right (B[] * A)
    //    private ScalarExpression _expression;

    //    public MatrixExpressionAddScalar(MatrixExpression Parent, ScalarExpression Expression, int Association)
    //        : base(Parent)
    //    {
    //        this._Association = Association;
    //        this._expression = Expression;
    //    }

    //    public override CellMatrix Evaluate(FieldResolver Variant)
    //    {
    //        if (this._Association == 0)
    //            return this._expression.Evaluate(Variant) + this[0].Evaluate(Variant);
    //        else
    //            return this[0].Evaluate(Variant) + this._expression.Evaluate(Variant);
    //    }

    //    public override CellAffinity ReturnAffinity()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnAffinity();
    //        else
    //            return this[0].ReturnAffinity();
    //    }

    //    public override int ReturnSize()
    //    {
    //        if (this._Association == 0)
    //            return this._expression.ReturnSize();
    //        else
    //            return this[0].ReturnSize();
    //    }

    //    public override MatrixExpression CloneOfMe()
    //    {
    //        MatrixExpression node = new MatrixExpressionAddScalar(this.ParentNode, this._expression.CloneOfMe(), this._Association);
    //        foreach (MatrixExpression m in this._Children)
    //            node.AddChildNode(m.CloneOfMe());
    //        return node;
    //    }

    //}




}
