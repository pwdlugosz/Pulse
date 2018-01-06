using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Tables;

namespace Pulse.Expressions.ActionExpressions
{
    
    public class ActionExpressionDeclareScalar : ActionExpression
    {

        private ObjectStore _Store;
        private string _Name;
        private ScalarExpression _Value;

        public ActionExpressionDeclareScalar(Host Host, ActionExpression Parent, ObjectStore Store, string Name, ScalarExpression Value)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {
            this._Store.DeclareScalar(this._Name, this._Value.Evaluate(Variant));
        }

    }

    public class ActionExpressionDeclareMatrix : ActionExpression
    {

        private ObjectStore _Store;
        private string _Name;
        private MatrixExpression _Value;

        public ActionExpressionDeclareMatrix(Host Host, ActionExpression Parent, ObjectStore Store, string Name, MatrixExpression Value)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Value = Value;
            if (Value == null) throw new Exception();
        }

        public override void Invoke(FieldResolver Variant)
        {
            this._Store.DeclareMatrix(this._Name, this._Value.Evaluate(Variant));
        }

    }

    public class ActionExpressionDeclareRecord : ActionExpression
    {

        private ObjectStore _Store;
        private string _Name;
        private RecordExpression _Value;

        public ActionExpressionDeclareRecord(Host Host, ActionExpression Parent, ObjectStore Store, string Name, RecordExpression Value)
            : base(Host, Parent)
        {
            this._Store = Store;
            this._Name = Name;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {
            this._Store.DeclareRecord(this._Name, this._Value.EvaluateAssociative(Variant));
        }

    }

    public class ActionExpressionDeclareTable : ActionExpression
    {

        private string _DB;
        private string _Name;
        private TableExpression _Value;

        public ActionExpressionDeclareTable(Host Host, ActionExpression Parent, string Database, string Name, TableExpression Value)
            : base(Host, Parent)
        {
            this._DB = Database;
            this._Name = Name;
            this._Value = Value;
            if (Value == null) throw new Exception();
        }

        public override void Invoke(FieldResolver Variant)
        {
            Table q = this._Host.CreateTable(this._DB, this._Name, this._Value.Columns);
            using (RecordWriter w = q.OpenWriter())
            {
                this._Value.Evaluate(Variant, w);
            }
        }


    }


}
