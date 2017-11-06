using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.ActionExpressions
{

    public sealed class ActionExpressionInsert : ActionExpression
    {

        private RecordWriter _Writer;
        private RecordExpression _Fields;

        public ActionExpressionInsert(Host Host, ActionExpression Parent, RecordWriter Writer, RecordExpression Fields)
            : base(Host, Parent)
        {
            this._Writer = Writer;
            this._Fields = Fields;
        }

        public override void Invoke(FieldResolver Variant)
        {

            Record r = this._Fields.Evaluate(Variant);
            this._Writer.Insert(r);

        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Writer.Close();
        }

    }

}
