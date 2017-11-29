using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;

namespace Pulse.Expressions.ActionExpressions
{

    public sealed class ActionExpressionMatrixUnitAssign : ActionExpression
    {

        private string _Name;
        private Assignment _Logic;
        private ObjectStore _Store;
        private ScalarExpression _Value;
        private ScalarExpression _Row;
        private ScalarExpression _Col;

        public ActionExpressionMatrixUnitAssign(Host Host, ActionExpression Parent, ObjectStore Store, string Name, ScalarExpression Row, 
            ScalarExpression Col, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Logic = Logic;
            this._Value = Value;
            this._Row = Row ?? ScalarExpression.ZeroINT;
            this._Col = Col ?? ScalarExpression.ZeroINT;
        }

        public override void Invoke(FieldResolver Variant)
        {

            int Row = (int)this._Row.Evaluate(Variant).valueLONG;
            int Col = (int)this._Col.Evaluate(Variant).valueLONG;

            switch (this._Logic)
            {

                case Assignment.Equals:
                    this._Store.Matrixes[this._Name][Row, Col] = this._Value.Evaluate(Variant);
                    break;
                case Assignment.PlusEquals:
                    this._Store.Matrixes[this._Name][Row, Col] = this._Store.Matrixes[this._Name][Row, Col] + this._Value.Evaluate(Variant);
                    break;
                case Assignment.MinusEquals:
                    this._Store.Matrixes[this._Name][Row, Col] = this._Store.Matrixes[this._Name][Row, Col] - this._Value.Evaluate(Variant);
                    break;
                case Assignment.ProductEquals:
                    this._Store.Matrixes[this._Name][Row, Col] = this._Store.Matrixes[this._Name][Row, Col] * this._Value.Evaluate(Variant);
                    break;
                case Assignment.DivideEquals:
                    this._Store.Matrixes[this._Name][Row, Col] = this._Store.Matrixes[this._Name][Row, Col] / this._Value.Evaluate(Variant);
                    break;
                case Assignment.CheckDivideEquals:
                    this._Store.Matrixes[this._Name][Row, Col] = Cell.CheckDivide(this._Store.Matrixes[this._Name][Row, Col], this._Value.Evaluate(Variant));
                    break;
                case Assignment.ModEquals:
                    this._Store.Matrixes[this._Name][Row, Col] = this._Store.Matrixes[this._Name][Row, Col] % this._Value.Evaluate(Variant);
                    break;

            }

        }

    }


}
