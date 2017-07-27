﻿using System;
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

            foreach (Table t in this.ChildTables)
            {

                RecordReader rs = t.OpenReader();
                Writer.Consume(rs);

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
