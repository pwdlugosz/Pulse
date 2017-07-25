using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;

namespace Pulse.Scripting
{
    
    public class MatrixExpressionVisitor : PulseParserBaseVisitor<MatrixExpression>
    {

        private Host _Host;
        private MatrixExpression _Master;

        public MatrixExpressionVisitor(Host Host, ScalarExpressionVisitor Visitor)
            : base()
        {
            this._Host = Host;
            this.SeedVisitor = Visitor;
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
            get;
            set;
        }

        public override MatrixExpression VisitMatrixInvert(PulseParser.MatrixInvertContext context)
        {
            MatrixExpression m = new MatrixExpressionInverse(this._Master);
            m.AddChildNode(this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixTranspose(PulseParser.MatrixTransposeContext context)
        {
            MatrixExpression m = new MatrixExpressionTranspose(this._Master);
            m.AddChildNode(this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixTrueMul(PulseParser.MatrixTrueMulContext context)
        {
            MatrixExpression m = new MatrixExpressionMatrixMultiply(this._Master);
            m.AddChildNode(this.Visit(context.matrix_expression()[0]));
            m.AddChildNode(this.Visit(context.matrix_expression()[1]));
            this._Master = m;
            return m;
        }

        public override MatrixExpression VisitMatrixMulDiv(PulseParser.MatrixMulDivContext context)
        {

            MatrixExpression m;
            if (context.op.Type == PulseParser.MUL)
                m = new MatrixExpressionMultiply(this._Master);
            else if (context.op.Type == PulseParser.DIV)
                m = new MatrixExpressionDivide(this._Master);
            else
                m = new MatrixExpressionCheckDivide(this._Master);

            m.AddChildNode(this.Visit(context.matrix_expression()[0]));
            m.AddChildNode(this.Visit(context.matrix_expression()[1]));
            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixMulDivLeft(PulseParser.MatrixMulDivLeftContext context)
        {

            ScalarExpression node = this.SeedVisitor.Render(context.expression());
            MatrixExpression m;
            // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
            if (context.op.Type == PulseParser.MUL)
                m = new MatrixExpressionMultiplyScalar(this._Master, node, 0);
            else if (context.op.Type == PulseParser.DIV)
                m = new MatrixExpressionDivideScalar(this._Master, node, 0);
            else
                m = new MatrixExpressionCheckDivideScalar(this._Master, node, 0);

            m.AddChildNode(this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixMulDivRight(PulseParser.MatrixMulDivRightContext context)
        {

            ScalarExpression node = this.SeedVisitor.Render(context.expression());
            MatrixExpression m;
            // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
            if (context.op.Type == PulseParser.MUL)
                m = new MatrixExpressionMultiplyScalar(this._Master, node, 1);
            else if (context.op.Type == PulseParser.DIV)
                m = new MatrixExpressionDivideScalar(this._Master, node, 1);
            else
                m = new MatrixExpressionCheckDivideScalar(this._Master, node, 1);

            m.AddChildNode(this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixAddSub(PulseParser.MatrixAddSubContext context)
        {

            MatrixExpression m;
            if (context.op.Type == PulseParser.PLUS)
                m = new MatrixExpressionAdd(this._Master);
            else
                m = new MatrixExpressionSubtract(this._Master);

            m.AddChildNode(this.Visit(context.matrix_expression()[0]));
            m.AddChildNode(this.Visit(context.matrix_expression()[1]));
            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixAddSubLeft(PulseParser.MatrixAddSubLeftContext context)
        {

            ScalarExpression node = this.SeedVisitor.Render(context.expression());
            MatrixExpression m;
            // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
            if (context.op.Type == PulseParser.PLUS)
                m = new MatrixExpressionAddScalar(this._Master, node, 0);
            else
                m = new MatrixExpressionSubtractScalar(this._Master, node, 0);

            m.AddChildNode(this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixAddSubRight(PulseParser.MatrixAddSubRightContext context)
        {

            ScalarExpression node = this.SeedVisitor.Render(context.expression());
            MatrixExpression m;
            // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
            if (context.op.Type == PulseParser.PLUS)
                m = new MatrixExpressionAddScalar(this._Master, node, 1);
            else
                m = new MatrixExpressionSubtractScalar(this._Master, node, 1);

            m.AddChildNode(this.Visit(context.matrix_expression()));
            this._Master = m;
            return m;

        }

        public override MatrixExpression VisitMatrixLookup(PulseParser.MatrixLookupContext context)
        {

            string Alias = context.IDENTIFIER()[0].GetText();
            string Name = context.IDENTIFIER()[1].GetText();

            // Check if the alias exists //
            if (!this._Host.Libraries.Exists(Alias))
                throw new Exception(string.Format("Library '{0}' does not exist", Alias));

            // Get the library //
            Libraries.Library x = this._Host.Libraries[Alias];
            if (!x.Matrixes.Exists(Name))
                throw new Exception(string.Format("Matrix '{0}' does not exist in library '{1}'", Name, Alias));

            return new MatrixExpressionHeap(this._Master, x.Matrixes, x.Matrixes.GetPointer(Name));

        }

        public override MatrixExpression VisitMatrixNakedLookup(PulseParser.MatrixNakedLookupContext context)
        {

            string Name = context.IDENTIFIER().GetText();

            // Get the library //
            Libraries.Library x = this._Host.BaseLibrary;
            if (!x.Matrixes.Exists(Name))
                throw new Exception(string.Format("Matrix '{0}' does not exist in the base library", Name));

            return new MatrixExpressionHeap(this._Master, x.Matrixes, x.Matrixes.GetPointer(Name));

        }

        public override MatrixExpression VisitMatrixLiteral(PulseParser.MatrixLiteralContext context)
        {

            int Cols = context.matrix_literal().vector_literal().Length;
            int Rows = context.matrix_literal().vector_literal()[0].expression().Length;
            CellAffinity affinity = this.SeedVisitor.Render(context.matrix_literal().vector_literal()[0].expression()[0]).ExpressionReturnAffinity();
            int size = this.SeedVisitor.Render(context.matrix_literal().vector_literal()[0].expression()[0]).ExpressionSize();
            CellMatrix matrix = new CellMatrix(Rows, Cols, affinity, size);
            FieldResolver fr = this.SeedVisitor.ImpliedResolver;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    matrix[i, j] = this.SeedVisitor.Render(context.matrix_literal().vector_literal()[j].expression()[i]).Evaluate(fr);
                }
            }

            return new MatrixExpressionLiteral(this._Master, matrix);

        }

        public override MatrixExpression VisitMatrixIdent(PulseParser.MatrixIdentContext context)
        {

            int rank = (int)this.SeedVisitor.Render(context.expression()).Evaluate(this.SeedVisitor.ImpliedResolver).INT;
            CellAffinity type = ScriptingHelper.GetTypeAffinity(context.type());
            int size = ScriptingHelper.GetTypeSize(context.type());

            return new MatrixExpressionIdentity(this._Master, rank, type);

        }

        public override MatrixExpression VisitMatrixParen(PulseParser.MatrixParenContext context)
        {
            return this.Visit(context.matrix_expression());
        }

        public MatrixExpression ToMatrix(PulseParser.Matrix_expressionContext context)
        {
            return this.Visit(context);
        }


    }

}
