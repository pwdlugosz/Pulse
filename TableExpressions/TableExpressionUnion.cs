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


    public sealed class TableExpressionUnion : TableExpression
    {

        public TableExpressionUnion(Host Host, TableExpression Parent)
            : base(Host, Parent)
        {
            this.Alias = "UNION";
        }

        public override Schema Columns
        {
            get { return this._Children.First().Columns; }
        }

        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            return Variants.CloneOfMeFull();
        }

        public override void AddChild(TableExpression Child)
        {

            if (this._Children.Count == 0)
            {
                base.AddChild(Child);
            }
            else if (Child.Columns.Count == this.Columns.Count)
            {
                base.AddChild(Child);
            }
            else
            {
                throw new Exception("Schema of the child node is not compatible with current node");
            }

        }

        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {

            // Loop through each table //
            foreach (Table t in this.RenderChildTables(Variants))
            {

                // Open a reader //
                RecordReader rs = t.OpenReader();

                // Write //
                Writer.Consume(rs);

                // Clean up //
                if (this._Host.IsSystemTemp(t))
                    this._Host.Store.DropTable(t.Key);


            }

        }

        public override long EstimatedCount
        {
            get
            {
                return this._Children.Sum((x) => { return x.EstimatedCount; });
            }
        }

    }


}
