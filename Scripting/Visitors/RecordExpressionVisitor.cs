﻿using System;
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
        private ScalarExpressionVisitor _scalars;

        public RecordExpressionVisitor(ScalarExpressionVisitor Factory)
            : base()
        {
            this._Host = Factory.Host;
            this._scalars = Factory;
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public ScalarExpressionVisitor BaseFactory
        {
            get { return this._scalars; }
        }

        public override RecordExpression VisitRecordExpressionLiteral(PulseParser.RecordExpressionLiteralContext context)
        {

            RecordExpression rex = new RecordExpression();
            int cnt = 0;

            foreach (PulseParser.Scalar_expression_aliasContext ctx in context.naked_record().scalar_expression_alias())
            {

                string name = (ctx.IDENTIFIER() == null ? "F" + cnt.ToString() : ctx.IDENTIFIER().GetText());
                ScalarExpression sx = this._scalars.Visit(ctx.scalar_expression());
                rex.Add(name, sx);
                cnt++;

            }

            return rex;
        }

        public override RecordExpression VisitRecordExpressionLookup(PulseParser.RecordExpressionLookupContext context)
        {

            string lib = ScriptingHelper.GetLibName(context.record_name().var_name());
            string name = ScriptingHelper.GetVarName(context.record_name().var_name());
            if (!this._Host.Libraries.Exists(lib))
                throw new Exception(string.Format("Library '{0}' does not exist"));
            if (!this._Host.Libraries[lib].Records.Exists(name))
                throw new Exception(string.Format("Record '{0}' does not exist in library '{1}'", name, lib));
            return new RecordExpression(this._Host.Libraries[lib].Records[name]);
            
        }

        public override RecordExpression VisitRecordExpressionUnion(PulseParser.RecordExpressionUnionContext context)
        {
            RecordExpression A = this.Visit(context.record_expression()[0]);
            RecordExpression B = this.Visit(context.record_expression()[1]);
            return A + B;
        }

        public override RecordExpression VisitRecordExpressionParens(PulseParser.RecordExpressionParensContext context)
        {
            return this.Visit(context.record_expression());
        }

        public RecordExpression Render(PulseParser.Record_expressionContext context)
        {
            return this.Visit(context);
        }

        public List<RecordExpression> Render(PulseParser.Record_expressionContext[] context)
        {

            List<RecordExpression> rexs = new List<RecordExpression>();
            foreach (PulseParser.Record_expressionContext ctx in context)
            {
                RecordExpression rex = this.Render(ctx);
                rexs.Add(rex);
            }

            return rexs;

        }

        public static RecordExpression Render(ScalarExpressionVisitor ScalarFactory, PulseParser.Record_expressionContext context)
        {
            return (new RecordExpressionVisitor(ScalarFactory).Render(context));
        }

        public static List<RecordExpression> Render(ScalarExpressionVisitor ScalarFactory, PulseParser.Record_expressionContext[] context)
        {
            return new RecordExpressionVisitor(ScalarFactory).Render(context);
        }

        public static List<Record> Render(List<RecordExpression> Rex, FieldResolver Variants)
        {

            List<Record> x = new List<Record>();
            foreach (RecordExpression r in Rex)
            {
                Record y = r.Evaluate(Variants);
                x.Add(y);
            }
            return x;

        }

        public static List<AssociativeRecord> RenderAssociative(List<RecordExpression> Rex, FieldResolver Variants)
        {

            List<AssociativeRecord> x = new List<AssociativeRecord>();
            foreach (RecordExpression r in Rex)
            {
                AssociativeRecord y = r.EvaluateAssociative(Variants);
                x.Add(y);
            }
            return x;

        }

    }

}
