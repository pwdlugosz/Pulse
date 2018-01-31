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

    public sealed class ActionExpressionScalarAssign : ActionExpression
    {

        private string _Name;
        private Assignment _Logic;
        private ObjectStore _Store;
        private ScalarExpression _Value;

        public ActionExpressionScalarAssign(Host Host, ActionExpression Parent, ObjectStore Store, string Name, ScalarExpression Value, Assignment Logic)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Logic = Logic;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {

            switch (this._Logic)
            {

                case Assignment.Equals:
                    this._Store.Scalars[this._Name] = this._Value.Evaluate(Variant);
                    break;
                case Assignment.PlusEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] + this._Value.Evaluate(Variant);
                    break;
                case Assignment.MinusEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] - this._Value.Evaluate(Variant);
                    break;
                case Assignment.ProductEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] * this._Value.Evaluate(Variant);
                    break;
                case Assignment.DivideEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] / this._Value.Evaluate(Variant);
                    break;
                case Assignment.CheckDivideEquals:
                    this._Store.Scalars[this._Name] = Cell.CheckDivide(this._Store.Scalars[this._Name], this._Value.Evaluate(Variant));
                    break;
                case Assignment.ModEquals:
                    this._Store.Scalars[this._Name] = this._Store.Scalars[this._Name] % this._Value.Evaluate(Variant);
                    break;

            }

        }

    }

    public sealed class ActionExpressionRecordMemberAssign : ActionExpression
    {

        private string _sName;
        private string _rName;
        private string _vName;
        private Assignment _Logic;
        private ScalarExpression _Value;

        public ActionExpressionRecordMemberAssign(Host Host, ActionExpression Parent, string StoreName, string RecordName, string ValueName, ScalarExpression Value, Assignment Logic)
            :base(Host, Parent)
        {
            this._sName = StoreName;
            this._rName = RecordName;
            this._vName = ValueName;
            this._Value = Value;
            this._Logic = Logic;
        }

        public override void Invoke(FieldResolver Variant)
        {
            
            Cell a = Variant.Stores[this._sName].Records[this._rName][this._vName];
            Cell b = this._Value.Evaluate(Variant);
            switch(this._Logic)
            {
                case Assignment.Equals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = b;
                    break;
                case Assignment.PlusEquals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = a + b;
                    break;
                case Assignment.MinusEquals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = a - b;
                    break;
                case Assignment.ProductEquals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = a * b;
                    break;
                case Assignment.DivideEquals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = a / b;
                    break;
                case Assignment.CheckDivideEquals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = Cell.CheckDivide(a, b);
                    break;
                case Assignment.ModEquals:
                    Variant.Stores[this._sName].Records[this._rName][this._vName] = a % b;
                    break;
            }

        }
    
    }



}
