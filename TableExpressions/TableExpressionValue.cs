using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Query;
using Pulse.ScalarExpressions;
using Pulse.Aggregates;
using Pulse.Query.Select;
using Pulse.Query.Aggregate;
using Pulse.Query.Join;
using Pulse.Query.Union;

namespace Pulse.TableExpressions
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
