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

    public sealed class TableExpressionJoin : TableExpression
    {

        private ScalarExpressionCollection _Fields;
        private RecordMatcher _Predicate;
        private Filter _Where;
        private JoinEngine _Engine;
        private JoinType _Type;

        public TableExpressionJoin(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, RecordMatcher Predicate, Filter Where, JoinEngine Engine, JoinType Type)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Predicate = Predicate;
            this._Where = Where;
            this._Engine = Engine;
            this._Type = Type;
        }

        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        public override void Evaluate(RecordWriter Writer)
        {
            this._Engine.Render(this._Host, Writer, this._Children[0].Evaluate(), this._Children[1].Evaluate(), this._Predicate, this._Fields, this._Where, JoinType.INNER);
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
