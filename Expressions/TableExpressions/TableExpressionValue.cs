using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Expressions.TableExpressions
{


    public sealed class TableExpressionValue : TableExpression
    {

        private Table _t;

        public TableExpressionValue(Host Host, TableExpression Parent, Table Value)
            : base(Host, Parent)
        {
            this._t = Value;
            this.Alias = "VALUE";
        }

        public override Schema Columns
        {
            get { return this._t.Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants.CloneOfMeFull();
        }

        // Evaluates //
        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {
            Writer.Consume(this._t.OpenReader());
        }

        public override Table Select(FieldResolver Variants)
        {
            return this._t;
        }

        public override void Recycle()
        {
            // do nothing
        }

        public override long EstimatedCount
        {
            get
            {
                return this._t.RecordCount;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return this._t.IsIndexedBy(IndexColumns);
        }

    }


}
