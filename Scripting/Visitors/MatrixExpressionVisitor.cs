﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions;

namespace Pulse.Scripting
{

    public class MatrixExpressionVisitor : PulseParserBaseVisitor<MatrixExpression>
    {

        private Host _Host;
        private MatrixExpression _Master;
        private ScalarExpressionVisitor _sFactory;

        public MatrixExpressionVisitor(Host Host, ScalarExpressionVisitor Visitor)
            : base()
        {
            this._Host = Host;
            this._sFactory = Visitor;
        }

        public MatrixExpressionVisitor(Host Host)
            : this(Host, new ScalarExpressionVisitor(Host))
        {
        }

        // Properties //
        /// <summary>
        /// This is used as a seed visitor for each matrix render
        /// </summary>
        public ScalarExpressionVisitor SeedVisitor
        {
            get { return this._sFactory; }
            set { this._sFactory = value; }
        }

        public override MatrixExpression VisitMatrixInvert(PulseParser.MatrixInvertContext context)
        {
            MatrixExpression m = new MatrixExpressionUnary.MatrixExpressionInverse(this._Master, this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixTranspose(PulseParser.MatrixTransposeContext context)
        {
            MatrixExpression m = new MatrixExpressionUnary.MatrixExpressionTranspose(this._Master, this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixTrueMul(PulseParser.MatrixTrueMulContext context)
        {
            MatrixExpression m = new MatrixExpressionBinary.MatrixExpressionBinaryMMultiply(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixMulDiv(PulseParser.MatrixMulDivContext context)
        {

            MatrixExpression m;
            if (context.op.Type == PulseParser.MUL)
                m = new MatrixExpressionBinary.MatrixExpressionBinaryMultiply(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            else if (context.op.Type == PulseParser.DIV)
                m = new MatrixExpressionBinary.MatrixExpressionBinaryDivide(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            else
                m = new MatrixExpressionBinary.MatrixExpressionBinaryCheckDivide(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));

            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixAddSub(PulseParser.MatrixAddSubContext context)
        {

            MatrixExpression m;
            if (context.op.Type == PulseParser.PLUS)
                m = new MatrixExpressionBinary.MatrixExpressionBinaryAdd(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));
            else
                m = new MatrixExpressionBinary.MatrixExpressionBinarySubtract(this._Master, this.Visit(context.matrix_expression()[0]), this.Visit(context.matrix_expression()[1]));

            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixLookup(PulseParser.MatrixLookupContext context)
        {

            string Alias = ScriptingHelper.GetLibName(context.matrix_name());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());

            // Check if the alias exists //
            if (!this.SeedVisitor.Map.StoreExists(Alias))
                throw new Exception(string.Format("Library '{0}' does not exist", Alias));

            // Get the library //
            ObjectStore os = this.SeedVisitor.Map[Alias];
            if (!os.Matrixes.Exists(Name))
                throw new Exception(string.Format("Matrix '{0}' does not exist in library '{1}'", Name, Alias));

            return new MatrixExpressionStoreRef(this._Master, Alias, Name, os.Matrixes[Name].Affinity, os.Matrixes[Name].Size);

        }

        public override MatrixExpression VisitMatrixLiteral(PulseParser.MatrixLiteralContext context)
        {

            FieldResolver fr = this.SeedVisitor.Map;

            // Get the record expressions //
            List<ScalarExpressionSet> x = new List<ScalarExpressionSet>();
            foreach (PulseParser.NframeContext ctx in context.nframe())
            {
                x.Add(this.SeedVisitor.Render(ctx));
            }

            // Get the highest affinity and record legnth //
            return new MatrixExpressionLiteral(this._Master, x);

        }

        public override MatrixExpression VisitMatrixExpressionFunction(PulseParser.MatrixExpressionFunctionContext context)
        {

            string LibName = ScriptingHelper.GetLibName(context.matrix_name());
            string FuncName = ScriptingHelper.GetVarName(context.matrix_name());

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].MatrixFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            ObjectFactory of = new ObjectFactory(this._Host, this._sFactory);

            MatrixExpressionFunction f = this._Host.Libraries[LibName].MatrixFunctionLookup(FuncName);
            foreach (PulseParser.ParamContext ctx in context.param())
            {
                Parameter p = of.Render(ctx);
                f.AddParameter(p);
            }

            this._Master = f;

            return f;

        }

        public override MatrixExpression VisitMatrixParen(PulseParser.MatrixParenContext context)
        {
            return this.Visit(context.matrix_expression());
        }

        public MatrixExpression Render(PulseParser.Matrix_expressionContext context)
        {
            return this.Visit(context);
        }


    }

}
