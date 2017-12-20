using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.ActionExpressions;
using Antlr4;
using Antlr4.Runtime;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Scripting
{

    public class ScriptProcessor
    {

        private Host _Host;

        public ScriptProcessor(Host Host)
        {
            this._Host = Host;
        }

        //private IExpression ToExpression(string Script)
        //{

        //    // Create a token stream and do lexal analysis //
        //    AntlrInputStream TextStream = new AntlrInputStream(Script);
        //    PulseLexer Lex = new PulseLexer(TextStream);

        //    // Parse the script //
        //    CommonTokenStream TokenStream = new CommonTokenStream(Lex);
        //    PulseParser HeartBeat = new PulseParser(TokenStream);

        //    // Create an executer object //
        //    ExpressionVisitor processor = new ExpressionVisitor(this._Host);

        //    PulseParser.ExprContext context = HeartBeat.compileUnit().expr();

        //    // Handle no expressions //
        //    if (context == null)
        //        return null;

        //    return processor.Render(context);

        //}

        public ScalarExpression ToScalar(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer Lex = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            PulseParser HeartBeat = new PulseParser(TokenStream);

            // Create an executer object //
            ScalarExpressionVisitor processor = new ScalarExpressionVisitor(this._Host);

            PulseParser.Scalar_expressionContext context = HeartBeat.compileUnit().scalar_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public MatrixExpression ToMatrixExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer Lex = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            PulseParser HeartBeat = new PulseParser(TokenStream);

            // Create an executer object //
            MatrixExpressionVisitor processor = new MatrixExpressionVisitor(this._Host);

            PulseParser.Matrix_expressionContext context = HeartBeat.compileUnit().matrix_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public RecordExpression ToRecordExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer Lex = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            PulseParser HeartBeat = new PulseParser(TokenStream);

            // Create an executer object //
            RecordExpressionVisitor processor = new RecordExpressionVisitor(this._Host);

            PulseParser.Record_expressionContext context = HeartBeat.compileUnit().record_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public TableExpression ToTableExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer Lex = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream TokenStream = new CommonTokenStream(Lex);
            PulseParser HeartBeat = new PulseParser(TokenStream);

            // Create an executer object //
            TableExpressionVisitor processor = new TableExpressionVisitor(this._Host);

            PulseParser.Table_expressionContext context = HeartBeat.compileUnit().table_expression();

            // Handle no expressions //
            if (context == null)
                return null;

            return processor.Render(context);

        }

        public ActionExpression ToActionExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer HorseLexer = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            PulseParser HeartBeat = new PulseParser(RyeTokenStream);

            // Create an executer object //
            ActionExpressionVisitor processor = new ActionExpressionVisitor(this._Host);
            
            PulseParser.Action_expressionContext[] actions = HeartBeat.compileUnit().action_expression();

            // Handle no expressions //
            if (actions == null)
                return null;
            
            ActionExpressionDo x = new ActionExpressionDo(this._Host, null);
            foreach (PulseParser.Action_expressionContext ctx in actions)
            {
                x.AddChild(processor.Visit(ctx));
            }
            return x;

        }

        public Cell RenderScalar(string Script)
        {
            return this.ToScalar(Script).Evaluate(new FieldResolver(this._Host));
        }

        public CellMatrix RenderMatrix(string Script)
        {
            return this.ToMatrixExpression(Script).Evaluate(new FieldResolver(this._Host));
        }

        public Table RenderTable(string Script)
        {
            return this.ToTableExpression(Script).Select(new FieldResolver(this._Host));
        }

        public void RenderAction(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer HorseLexer = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            PulseParser HeartBeat = new PulseParser(RyeTokenStream);

            // Create an executer object //
            ActionExpressionVisitor processor = new ActionExpressionVisitor(this._Host);

            PulseParser.Action_expressionContext[] actions = HeartBeat.compileUnit().action_expression();

            // Handle no expressions //
            if (actions == null)
                return;

            foreach (PulseParser.Action_expressionContext ctx in actions)
            {

                ActionExpression x = processor.Visit(ctx);
                FieldResolver fr = x.CreateResolver();
                x.BeginInvoke(fr);
                x.Invoke(fr);
                x.EndInvoke(fr);

            }


        }

    }

}
