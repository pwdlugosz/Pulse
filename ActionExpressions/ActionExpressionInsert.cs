using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;

namespace Pulse.ActionExpressions
{

    public sealed class ActionExpressionInsert : ActionExpression
    {

        private WriteStream _Writer;
        private ScalarExpressionCollection _Fields;

        public ActionExpressionInsert(Host Host, ActionExpression Parent, WriteStream Writer, ScalarExpressionCollection Fields)
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
