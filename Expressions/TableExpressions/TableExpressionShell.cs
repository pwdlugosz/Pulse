using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Tables;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.TableExpressions
{
    
    public class TableExpressionShell : TableExpression
    {

        Schema _Columns;

        public TableExpressionShell(Host Host, TableExpression Parent, Schema Columns)
            : base(Host, Parent)
        {
            this._Columns = Columns;
            this.Alias = "SHELL";
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants;
        }

        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {
            
        }

        public override long EstimatedCount
        {
            get
            {
                return 0;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return false;
        }

    }

    public sealed class TableExpressionLiteral : TableExpression
    {

        private Schema _Columns;
        private List<ScalarExpressionSet> _Records;

        public TableExpressionLiteral(Host Host, TableExpression Parent, List<ScalarExpressionSet> Values, Schema Columns)
            :base(Host, Parent)
        {
            this._Columns = Columns;
            this._Records = Values;
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return new FieldResolver(this._Host);
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return false;
        }

        public override long EstimatedCount
        {
            get
            {
                return this._Records.Count;
            }
        }

        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {

            foreach (ScalarExpressionSet r in this._Records)
            {

                Pulse.Elements.Record x = r.Evaluate(Variants);
                Writer.Insert(x);
            
            }

        }

    }

}
