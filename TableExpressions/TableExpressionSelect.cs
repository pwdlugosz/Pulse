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


    public sealed class TableExpressionSelect : TableExpression
    {

        private ScalarExpressionCollection _Fields;
        private Filter _Where;
        private Query.Select.SelectEngine _Engine;

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, Filter Where, Query.Select.SelectEngine Engine)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this._Engine = Engine;
        }

        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        public ScalarExpressionCollection Fields
        {
            get { return this._Fields; }
        }

        public Filter Where
        {
            get { return this._Where; }
        }

        public override void Evaluate(RecordWriter Writer)
        {
            this._Engine.Render(this._Host, Writer, this.Children[0].Evaluate(), Fields, Where);
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
