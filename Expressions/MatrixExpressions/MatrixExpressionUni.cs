using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    /// <summary>
    /// Opperations for two matrixes
    /// </summary>
    public sealed class MatrixExpressionOperation : MatrixExpression
    {

        private Operation _Operation;

        public MatrixExpressionOperation(MatrixExpression Parent, Operation Op)
            : base(Parent)
        {
            this._Operation = Op;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {

            CellMatrix a = this._Children[0].Evaluate(Variant);
            CellMatrix b = (this._Children.Count >= 2 ? this._Children[1].Evaluate(Variant) : null);

            switch (this._Operation)
            {

                case Operation.Plus: return +a;
                case Operation.Minus: return -a;
                case Operation.Not: return !a;

                case Operation.Multiply: return a * b;
                case Operation.Divide: return a / b;
                case Operation.Divide2: return CellMatrix.CheckDivide(a, b);
                case Operation.Mod: return a % b;
                case Operation.Add: return a + b;
                case Operation.Subtract: return a - b;

            }

            throw new Exception(string.Format("Operation '{0}' not supported for matrixes", this._Operation));

        }

        public override CellAffinity ReturnAffinity()
        {
            return this[0].ReturnAffinity();
        }

        public override MatrixExpression CloneOfMe()
        {
            MatrixExpression node = new MatrixExpressionOperation(this.ParentNode, this._Operation);
            foreach (MatrixExpression m in this._Children)
                node.AddChildNode(m.CloneOfMe());
            return node;
        }

        public override int ReturnSize()
        {
            return Math.Max(this._Children[0].ReturnSize(), this._Children[1].ReturnSize());
        }

    }

    /// <summary>
    /// Operations for when the left argument (A[0]) is a matrix
    /// </summary>
    public sealed class MatrixExpressionOperationLeft : MatrixExpression
    {

        private Operation _Operation;
        private ScalarExpression _value;

        public MatrixExpressionOperationLeft(MatrixExpression Parent, Operation Op, ScalarExpression Value)
            : base(Parent)
        {
            this._Operation = Op;
            this._value = Value;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {

            CellMatrix a = this._Children[0].Evaluate(Variant);
            Cell b = this._value.Evaluate(Variant);

            switch (this._Operation)
            {

                case Operation.Multiply: return a * b;
                case Operation.Divide: return a / b;
                case Operation.Divide2: return CellMatrix.CheckDivide(a, b);
                case Operation.Mod: return a % b;
                case Operation.Add: return a + b;
                case Operation.Subtract: return a - b;

            }

            throw new Exception(string.Format("Operation '{0}' not supported for matrixes", this._Operation));

        }

        public override CellAffinity ReturnAffinity()
        {
            return this[0].ReturnAffinity();
        }

        public override MatrixExpression CloneOfMe()
        {
            MatrixExpression node = new MatrixExpressionOperationLeft(this.ParentNode, this._Operation, this._value);
            foreach (MatrixExpression m in this._Children)
                node.AddChildNode(m.CloneOfMe());
            return node;
        }

        public override int ReturnSize()
        {
            return Math.Max(this._Children[0].ReturnSize(), this._Children[1].ReturnSize());
        }

    }

    /// <summary>
    /// Operations for when the right argument (A[0]) is a matrix
    /// </summary>
    public sealed class MatrixExpressionOperationRight : MatrixExpression
    {

        private Operation _Operation;
        private ScalarExpression _value;

        public MatrixExpressionOperationRight(MatrixExpression Parent, Operation Op, ScalarExpression Value)
            : base(Parent)
        {
            this._Operation = Op;
            this._value = Value;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {

            Cell a = this._value.Evaluate(Variant);
            CellMatrix b = this._Children[0].Evaluate(Variant); 

            switch (this._Operation)
            {

                case Operation.Multiply: return a * b;
                case Operation.Divide: return a / b;
                case Operation.Divide2: return CellMatrix.CheckDivide(a, b);
                case Operation.Mod: return a % b;
                case Operation.Add: return a + b;
                case Operation.Subtract: return a - b;

            }

            throw new Exception(string.Format("Operation '{0}' not supported for matrixes", this._Operation));

        }

        public override CellAffinity ReturnAffinity()
        {
            return this[0].ReturnAffinity();
        }

        public override MatrixExpression CloneOfMe()
        {
            MatrixExpression node = new MatrixExpressionOperationLeft(this.ParentNode, this._Operation, this._value);
            foreach (MatrixExpression m in this._Children)
                node.AddChildNode(m.CloneOfMe());
            return node;
        }

        public override int ReturnSize()
        {
            return Math.Max(this._Children[0].ReturnSize(), this._Children[1].ReturnSize());
        }

    }



}
