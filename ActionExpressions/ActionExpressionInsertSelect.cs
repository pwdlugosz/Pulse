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

    /// <summary>
    /// Represents an action that appends records to a table 
    /// </summary>
    public sealed class ActionExpressionInsertSelect : ActionExpression
    {

        private RecordWriter _Writer;
        private TableExpression _Select;
        
        public ActionExpressionInsertSelect(Host Host, ActionExpression Parent, RecordWriter Writer, TableExpression Select)
            : base(Host, Parent)
        {

            // Need to check that the input and output table are compatible //
            if (Writer.Columns.Count != Select.Columns.Count)
                throw new Exception("Destination table and input expression have different field counts");

            this._Writer = Writer;
            this._Select = Select;
        }

        public override void Invoke(FieldResolver Variant)
        {

            // TODO: if the table doesnt have modifiers, then have it write directly to the stream rather than evaluating and then
            // writing to the stream

            // Get the source table //
            Table t = this._Select.Evaluate(Variant);

            // Write the data to a table //
            using (RecordReader rr = t.OpenReader())
            {
                this._Writer.Consume(rr);
            }

            // Drop the table; we do this here in case we're in a loop, the processor would accumulate too many tables and crash //
            this._Host.Store.DropTable(t.Key);
            
        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Writer.Close();
        }

        public override FieldResolver CreateResolver()
        {
            return new FieldResolver(this._Host);
        }

    }


}
