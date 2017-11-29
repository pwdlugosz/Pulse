using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.Aggregates;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;

namespace Pulse.Expressions
{

    public enum ExpressionAffinity
    {
        Scalar,
        Aggregate,
        Matrix,
        Record,
        Table,
        Void
    }

    public abstract class ExpressionResolver
    {

        protected Host _Host;
        protected List<ExpressionResolver> _Children;
        protected ExpressionResolver _Parent;

        public ExpressionResolver(Host Host, ExpressionResolver Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<ExpressionResolver>();
        }

        public ExpressionResolver Parent
        {
            get { return this._Parent; }
        }

        public List<ExpressionResolver> Children
        {
            get { return this._Children; }
        }

        public ExpressionResolver this[int Index]
        {
            get { return this._Children[Index]; }
        }

        /*
         * SizeOf
         * LengthOf
         * TypeOf
         * SuperTypeOf
         * NameOf
         * ScriptOf
         * SchemaOf
         * 
         */

        //public abstract long HashOf(FieldResolver Variants);

        //public abstract int LengthOf(FieldResolver Variants);

        //public abstract string NameOf(FieldResolver Variants);
        
        //public abstract Schema SchemaOf(FieldResolver Varinats);
        
        //public abstract string ScriptOf(FieldResolver Variants);

        //public abstract int SizeOf(FieldResolver Variants);

        //public abstract CellAffinity TypeOf(FieldResolver Variants);

        public abstract ExpressionAffinity ReturnAffinity();

        public abstract ScalarExpression RenderScalar();

        public abstract MatrixExpression RenderMatrix();

        public abstract RecordExpression RenderRecord();

        public abstract TableExpression RenderTable();

    }

    public sealed class ExpressionResolverGlobalVariable : ExpressionResolver
    {

        private string _Name;

        public ExpressionResolverGlobalVariable(Host Host, ExpressionResolverGlobalVariable Parent, string Name)
            : base(Host, Parent)
        {
            this._Name = Name;
        }

        public override ExpressionAffinity ReturnAffinity()
        {

            if (this._Host.Store.ExistsScalar(this._Name))
                return ExpressionAffinity.Scalar;
            if (this._Host.Store.ExistsMatrix(this._Name))
                return ExpressionAffinity.Matrix;
            if (this._Host.Store.ExistsRecord(this._Name))
                return ExpressionAffinity.Record;
            if (this._Host.Store.ExistsTable(this._Name))
                return ExpressionAffinity.Table ;
            return ExpressionAffinity.Void;

        }

        public override ScalarExpression RenderScalar()
        {
            if (!this._Host.Store.ExistsScalar(this._Name))
                throw new ObjectStore.ObjectDoesNotExistException(this._Name);
            return new ScalarExpressionStoreRef(null, this._Name, this._Host.Store);
        }

        public override MatrixExpression RenderMatrix()
        {
            if (!this._Host.Store.ExistsMatrix(this._Name))
                throw new ObjectStore.ObjectDoesNotExistException(this._Name);
            return new MatrixExpressionStoreRef(null, this._Name, this._Host.Store);
        }

        public override RecordExpression RenderRecord()
        {
            if (!this._Host.Store.ExistsRecord(this._Name))
                throw new ObjectStore.ObjectDoesNotExistException(this._Name);
            return new RecordExpressionStoreRef(this._Host, null, this._Name, this._Host.Store);
        }

        public override TableExpression RenderTable()
        {
            return new TableExpressionStoreRef(this._Host, null, this._Name, this._Host.Store);
        }

    }



}
