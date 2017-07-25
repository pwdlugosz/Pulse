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

        private UnionEngine _Engine;

        public TableExpressionUnion(Host Host, TableExpression Parent, UnionEngine Engine)
            : base(Host, Parent)
        {
            this._Engine = Engine;
        }

        public override Schema Columns
        {
            get { return this._Children.First().Columns; }
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

        public override void Evaluate(RecordWriter Writer)
        {
            this._Engine.Render(this._Host, Writer, this.ChildTables);
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
