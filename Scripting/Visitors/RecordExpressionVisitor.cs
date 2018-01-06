using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Libraries;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;


namespace Pulse.Scripting
{

    public class RecordExpressionVisitor : PulseParserBaseVisitor<RecordExpression>
    {

        private Host _Host;
        private ScalarExpressionVisitor _sFactory;
        private RecordExpression _Master;

        public RecordExpressionVisitor(Host Host, ScalarExpressionVisitor Factory)
            : base()
        {
            this._Host = Host;
            this._sFactory = Factory;
        }

        public RecordExpressionVisitor(Host Host)
            : this(Host, new ScalarExpressionVisitor(Host))
        {
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public ScalarExpressionVisitor BaseFactory
        {
            get { return this._sFactory; }
        }

        public override RecordExpression VisitRecordExpressionLiteral(PulseParser.RecordExpressionLiteralContext context)
        {

            RecordExpressionLiteral rex = new RecordExpressionLiteral(this._Host, this._Master);
            int cnt = 0;

            foreach (PulseParser.NelementContext ctx in context.nframe().nelement())
            {
                string name = (ctx.IDENTIFIER() == null ? "F" + cnt.ToString() : ctx.IDENTIFIER().GetText());
                ScalarExpression sx = this._sFactory.Visit(ctx.scalar_expression());
                rex.Add(sx, name);
                cnt++;
            }

            return rex;
        }

        public override RecordExpression VisitRecordExpressionLookup(PulseParser.RecordExpressionLookupContext context)
        {
            string lib = ScriptingHelper.GetLibName(context.record_name());
            string name = ScriptingHelper.GetVarName(context.record_name());
            if (!this._sFactory.Map.StoreExists(lib))
                throw new Exception(string.Format("Object store '{0}' does not exist"));
            if (!this._sFactory.Map[lib].Records.Exists(name))
                throw new Exception(string.Format("Record '{0}' does not exist in library '{1}'", name, lib));
            return new RecordExpressionStoreRef(this._Host, this._Master, lib, name, this._sFactory.Map[lib].Records[name].Columns);
        }

        //public override RecordExpression VisitRecordExpressionUnion(PulseParser.RecordExpressionUnionContext context)
        //{
        //    RecordExpression A = this.Visit(context.record_expression()[0]);
        //    RecordExpression B = this.Visit(context.record_expression()[1]);
        //    return recorde
        //}

        public override RecordExpression VisitRecordExpressionParens(PulseParser.RecordExpressionParensContext context)
        {
            return this.Visit(context.record_expression());
        }

        public override RecordExpression VisitRecordExpressionFunction(PulseParser.RecordExpressionFunctionContext context)
        {

            string LibName = ScriptingHelper.GetLibName(context.record_name());
            string FuncName = ScriptingHelper.GetVarName(context.record_name());

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].ScalarFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            ObjectFactory of = new ObjectFactory(this._Host, this._sFactory);

            RecordExpressionFunction f = this._Host.Libraries[LibName].RecordFunctionLookup(FuncName);
            foreach (PulseParser.ParamContext ctx in context.param())
            {
                Parameter p = of.Render(ctx);
                f.AddParameter(p);
            }

            this._Master = f;

            return f;

        }

        public RecordExpression Render(PulseParser.Record_expressionContext context)
        {
            return this.Visit(context);
        }

    }

}
