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


    public sealed class TableExpressionFold : TableExpression
    {

        private ScalarExpressionCollection _Keys;
        private AggregateCollection _Values;
        private ScalarExpressionCollection _Select;
        private Filter _Where;
        private AggregateEngine _Engine;

        public TableExpressionFold(Host Host, TableExpression Parent, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where,
            ScalarExpressionCollection Select, AggregateEngine Engine)
            : base(Host, Parent)
        {
            this._Keys = Keys;
            this._Values = Values;
            this._Where = Where;
            this._Engine = Engine;
            this._Select = Select;
        }

        public override Schema Columns
        {
            get { return this._Engine.GetOutputSchema(this._Keys, this._Values); }
        }

        public override void Evaluate(RecordWriter Writer)
        {
            
            Table t = this._Children[0].Evaluate();
            AggregateMetaData amd = new AggregateMetaData();
            this._Engine.Render(this._Host, Writer, t, this._Keys, this._Values, this._Where, this._Select, amd);

        }

        public override long EstimatedCount
        {
            get
            {
                return this._Children.Max((x) => { return x.EstimatedCount; });
            }
        }

    }


}
