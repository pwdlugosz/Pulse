using System;
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
    
    //public class MatrixExpressionVisitor : PulseParserBaseVisitor<MatrixExpression>
    //{

    //    private Host _Host;
    //    private MatrixExpression _Master;

    //    public MatrixExpressionVisitor(Host Host, ScalarExpressionVisitor Visitor)
    //        : base()
    //    {
    //        this._Host = Host;
    //        this.SeedVisitor = Visitor;
    //    }

    //    public MatrixExpressionVisitor(Host Host)
    //        : this(Host, new ScalarExpressionVisitor(Host))
    //    {
    //    }

    //    // Properties //
    //    /// <summary>
    //    /// This is used as a seed visitor for each matrix render
    //    /// </summary>
    //    public ScalarExpressionVisitor SeedVisitor
    //    {
    //        get;
    //        set;
    //    }

    //    public override MatrixExpression VisitMatrixInvert(PulseParser.MatrixInvertContext context)
    //    {
    //        MatrixExpression m = new MatrixExpressionInverse(this._Master);
    //        m.AddChildNode(this.Visit(context.matrix_expression()));
    //        this._Master = m;
    //        return m;
    //    }

    //    public override MatrixExpression VisitMatrixTranspose(PulseParser.MatrixTransposeContext context)
    //    {
    //        MatrixExpression m = new MatrixExpressionTranspose(this._Master);
    //        m.AddChildNode(this.Visit(context.matrix_expression()));
    //        this._Master = m;
    //        return m;
    //    }

    //    public override MatrixExpression VisitMatrixTrueMul(PulseParser.MatrixTrueMulContext context)
    //    {
    //        MatrixExpression m = new MatrixExpressionMatrixMultiply(this._Master);
    //        m.AddChildNode(this.Visit(context.matrix_expression()[0]));
    //        m.AddChildNode(this.Visit(context.matrix_expression()[1]));
    //        this._Master = m;
    //        return m;
    //    }

    //    public override MatrixExpression VisitMatrixMulDiv(PulseParser.MatrixMulDivContext context)
    //    {

    //        MatrixExpression m;
    //        if (context.op.Type == PulseParser.MUL)
    //            m = new MatrixExpressionMultiply(this._Master);
    //        else if (context.op.Type == PulseParser.DIV)
    //            m = new MatrixExpressionDivide(this._Master);
    //        else
    //            m = new MatrixExpressionCheckDivide(this._Master);

    //        m.AddChildNode(this.Visit(context.matrix_expression()[0]));
    //        m.AddChildNode(this.Visit(context.matrix_expression()[1]));
    //        this._Master = m;
    //        return m;

    //    }

    //    //public override MatrixExpression VisitMatrixMulDivLeft(PulseParser.MatrixMulDivLeftContext context)
    //    //{

    //    //    ScalarExpression node = this.SeedVisitor.Render(context.scalar_expression());
    //    //    MatrixExpression m;
    //    //    // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
    //    //    if (context.op.Type == PulseParser.MUL)
    //    //        m = new MatrixExpressionMultiplyScalar(this._Master, node, 0);
    //    //    else if (context.op.Type == PulseParser.DIV)
    //    //        m = new MatrixExpressionDivideScalar(this._Master, node, 0);
    //    //    else
    //    //        m = new MatrixExpressionCheckDivideScalar(this._Master, node, 0);

    //    //    m.AddChildNode(this.Visit(context.matrix_expression()));
    //    //    this._Master = m;
    //    //    return m;

    //    //}

    //    //public override MatrixExpression VisitMatrixMulDivRight(PulseParser.MatrixMulDivRightContext context)
    //    //{

    //    //    ScalarExpression node = this.SeedVisitor.Render(context.scalar_expression());
    //    //    MatrixExpression m;
    //    //    // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
    //    //    if (context.op.Type == PulseParser.MUL)
    //    //        m = new MatrixExpressionMultiplyScalar(this._Master, node, 1);
    //    //    else if (context.op.Type == PulseParser.DIV)
    //    //        m = new MatrixExpressionDivideScalar(this._Master, node, 1);
    //    //    else
    //    //        m = new MatrixExpressionCheckDivideScalar(this._Master, node, 1);

    //    //    m.AddChildNode(this.Visit(context.matrix_expression()));
    //    //    this._Master = m;
    //    //    return m;

    //    //}

    //    public override MatrixExpression VisitMatrixAddSub(PulseParser.MatrixAddSubContext context)
    //    {

    //        MatrixExpression m;
    //        if (context.op.Type == PulseParser.PLUS)
    //            m = new MatrixExpressionAdd(this._Master);
    //        else
    //            m = new MatrixExpressionSubtract(this._Master);

    //        m.AddChildNode(this.Visit(context.matrix_expression()[0]));
    //        m.AddChildNode(this.Visit(context.matrix_expression()[1]));
    //        this._Master = m;
    //        return m;

    //    }

    //    //public override MatrixExpression VisitMatrixAddSubLeft(PulseParser.MatrixAddSubLeftContext context)
    //    //{

    //    //    ScalarExpression node = this.SeedVisitor.Render(context.scalar_expression());
    //    //    MatrixExpression m;
    //    //    // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
    //    //    if (context.op.Type == PulseParser.PLUS)
    //    //        m = new MatrixExpressionAddScalar(this._Master, node, 0);
    //    //    else
    //    //        m = new MatrixExpressionSubtractScalar(this._Master, node, 0);

    //    //    m.AddChildNode(this.Visit(context.matrix_expression()));
    //    //    this._Master = m;
    //    //    return m;

    //    //}

    //    //public override MatrixExpression VisitMatrixAddSubRight(PulseParser.MatrixAddSubRightContext context)
    //    //{

    //    //    ScalarExpression node = this.SeedVisitor.Render(context.scalar_expression());
    //    //    MatrixExpression m;
    //    //    // Third parameter here: 0 == scalar is on left side (A + B[]), 1 == scalar is on right side (A[] + B)
    //    //    if (context.op.Type == PulseParser.PLUS)
    //    //        m = new MatrixExpressionAddScalar(this._Master, node, 1);
    //    //    else
    //    //        m = new MatrixExpressionSubtractScalar(this._Master, node, 1);

    //    //    m.AddChildNode(this.Visit(context.matrix_expression()));
    //    //    this._Master = m;
    //    //    return m;

    //    //}

    //    public override MatrixExpression VisitMatrixLookup(PulseParser.MatrixLookupContext context)
    //    {

    //        string Alias = ScriptingHelper.GetLibName(context.matrix_name().var_name());
    //        string Name = ScriptingHelper.GetVarName(context.matrix_name().var_name());

    //        // Check if the alias exists //
    //        if (!this._Host.Libraries.Exists(Alias))
    //            throw new Exception(string.Format("Library '{0}' does not exist", Alias));

    //        // Get the library //
    //        Libraries.Library x = this._Host.Libraries[Alias];
    //        if (!x.Matrixes.Exists(Name))
    //            throw new Exception(string.Format("Matrix '{0}' does not exist in library '{1}'", Name, Alias));

    //        return new ScalarExpressionStoreRef(this._Master, x.Matrixes, x.Matrixes.GetPointer(Name));

    //    }

    //    public override MatrixExpression VisitMatrixLiteral(PulseParser.MatrixLiteralContext context)
    //    {

    //        FieldResolver fr = this.SeedVisitor.ImpliedResolver;

    //        // Get the record expressions //
    //        List<ScalarExpressionSet> v = RecordExpressionVisitor.Render(this.SeedVisitor, context.record_chain().record_expression());
    //        List<Record> x = RecordExpressionVisitor.Render(v, fr);

    //        // Get the highest affinity and record legnth //
    //        CellAffinity maxaffinity = CellAffinityHelper.LOWEST_AFFINITY;
    //        int colcount = 0;
    //        int[] size = new int[CellAffinityHelper.AFFINITY_COUNT];
    //        foreach (Record r in x)
    //        {
    //            colcount = Math.Max(r.Count, colcount);
    //            foreach(Cell c in r.BaseArray)
    //            {
    //                size[(int)c.Affinity] = Math.Max(size[(int)c.Affinity], c.Length); 
    //                maxaffinity = CellAffinityHelper.Highest(maxaffinity, c.Affinity);
    //            }
    //        }
    //        int fsize = size[(int)maxaffinity];

    //        CellMatrix matrix = new CellMatrix(x.Count, colcount, maxaffinity, fsize);
    //        Cell q = CellValues.Null(maxaffinity);
            
    //        for (int i = 0; i < matrix.RowCount; i++)
    //        {
    //            for (int j = 0; j < matrix.ColumnCount; j++)
    //            {
    //                matrix[i, j] = (j >= x[i].Count ? q : x[i][j]);
    //            }
    //        }

    //        return new MatrixExpressionLiteral(this._Master, matrix);

    //    }

    //    public override MatrixExpression VisitMatrixParen(PulseParser.MatrixParenContext context)
    //    {
    //        return this.Visit(context.matrix_expression());
    //    }

    //    public MatrixExpression ToMatrix(PulseParser.Matrix_expressionContext context)
    //    {
    //        return this.Visit(context);
    //    }


    //}

}
