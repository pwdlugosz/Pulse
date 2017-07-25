using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;
using Pulse.TableExpressions;
using Pulse.ActionExpressions;
using Antlr4;
using Antlr4.Runtime;

namespace Pulse.Scripting
{

    public class ScriptProcessor
    {

        private Host _Host;

        public ScriptProcessor(Host Host)
        {
            this._Host = Host;
        }

        public void Execute(string Script)
        {

            //// Create a token stream and do lexal analysis //
            //AntlrInputStream TextStream = new AntlrInputStream(Script);
            //PulseLexer HorseLexer = new PulseLexer(TextStream);

            //// Parse the script //
            //CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            //PulseParser HeartBeat = new PulseParser(RyeTokenStream);

            //// Handle the error listener //
            //HeartBeat.RemoveErrorListeners();
            
            //// Create an executer object //
            //CommandVisitor processor = new CommandVisitor(this.Enviro);

            //// Create the call stack and the error catch stack //
            //List<RyeParser.CommandContext> CallStack = new List<RyeParser.CommandContext>();
            //List<string> Errors = new List<string>();

            //// Load the call stack and/or parse the errors
            //try
            //{

            //    foreach (RyeParser.CommandContext ctx in rye.compile_unit().command_set().command())
            //    {
            //        CallStack.Add(ctx);
            //    }

            //}
            //catch (RyeParseException e1)
            //{

            //    Errors.Add("\tParsing Error Detected 1: ");
            //    Errors.Add("\t" + "\t" + e1.Message);

            //}
            //catch (RyeCompileException e2)
            //{

            //    Errors.Add("\tCompile Error Detected 2: ");
            //    Errors.Add("\t" + "\t" + e2.Message);

            //}
            
            //// Check to see if we found any errors //
            //if (Errors.Count != 0)
            //{

            //    this.Enviro.IO.WriteLine("Rye encountered one or more critical errors; no statements executed:");
            //    foreach (string s in Errors)
            //    {
            //        this.Enviro.IO.WriteLine(s);
            //    }

            //}

            //// Execute each element in the call stack //
            //int Runs = 0;
            //foreach (RyeParser.CommandContext ctx in CallStack)
            //{

            //    Runs += processor.Visit(ctx);

            //}

        }

        public ScalarExpression ToExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer HorseLexer = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            PulseParser HeartBeat = new PulseParser(RyeTokenStream);

            // Create an executer object //
            ScalarExpressionVisitor processor = new ScalarExpressionVisitor(this._Host);

            PulseParser.ExpressionContext context = HeartBeat.compileUnit().expression();

            // Handle no expressions //
            if (context == null)
                return ScalarExpression.NullInt;

            ScalarExpression x = processor.Visit(context);

            return x;

        }

        public MatrixExpression ToMatrixExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer HorseLexer = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            PulseParser HeartBeat = new PulseParser(RyeTokenStream);

            // Create an executer object //
            MatrixExpressionVisitor processor = new MatrixExpressionVisitor(this._Host);

            // Handle no expressions //
            if (HeartBeat.compileUnit().matrix_expression() == null)
                return new MatrixExpressionLiteral(null, new CellMatrix(1,1, Cell.NULL_INT));

            MatrixExpression x = processor.Visit(HeartBeat.compileUnit().matrix_expression());

            return x;

        }

        public TableExpression ToTableExpression(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            PulseLexer HorseLexer = new PulseLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(HorseLexer);
            PulseParser HeartBeat = new PulseParser(RyeTokenStream);

            // Create an executer object //
            TableExpressionVisitor processor = new TableExpressionVisitor(this._Host);

            // Handle no expressions //
            if (HeartBeat.compileUnit().table_expression() == null)
                return null;

            TableExpression x = processor.Visit(HeartBeat.compileUnit().table_expression());

            return x;

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
            return this.ToExpression(Script).Evaluate(new FieldResolver(this._Host));
        }

        public CellMatrix RenderMatrix(string Script)
        {
            return this.ToMatrixExpression(Script).Evaluate(new FieldResolver(this._Host));
        }

        public Table RenderTable(string Script)
        {
            return this.ToTableExpression(Script).Evaluate();
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
