using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;
using Pulse.TableExpressions;

namespace Pulse.ActionExpressions
{


    public sealed class ActionExpressionInsertSelect : ActionExpression
    {

        private RecordWriter _Writer;
        private TableExpression _Select;

        public ActionExpressionInsertSelect(Host Host, ActionExpression Parent, RecordWriter Writer, TableExpression Select)
            : base(Host, Parent)
        {
            this._Writer = Writer;
            this._Select = Select;
        }

        public override void Invoke(FieldResolver Variant)
        {

            using (RecordReader rr = this._Select.Evaluate().OpenReader())
            {
                this._Writer.Consume(rr);
            }

        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Writer.Close();
        }

    }


}
