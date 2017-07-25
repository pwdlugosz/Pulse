using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.ActionExpressions;
using Pulse.TableExpressions;
using Pulse.MatrixExpressions;
using Pulse.Libraries;


namespace Pulse.Scripting
{

    public class ActionExpressionVisitor : PulseParserBaseVisitor<ActionExpression>
    {

        private Host _Host;
        private Heap<RecordWriter> _OpenRecordStreams;
        private Heap<System.IO.StreamWriter> _OpenTextStreams;

        private ScalarExpressionVisitor _ScalarBuilder;
        private MatrixExpressionVisitor _MatrixBuilder;
        private TableExpressionVisitor _TableBuilder;
        private ActionExpression _Master;

        public ActionExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._ScalarBuilder = new ScalarExpressionVisitor(this._Host);
            this._MatrixBuilder = new MatrixExpressionVisitor(this._Host);
            this._TableBuilder = new TableExpressionVisitor(this._Host);
            this._OpenRecordStreams = new Heap<RecordWriter>();
            this._OpenTextStreams = new Heap<System.IO.StreamWriter>();
        }

        // Properties //
        /// <summary>
        /// Gets or sets the base scalar visitor
        /// </summary>
        public ScalarExpressionVisitor BaseScalarExpressionVisitor
        {
            get { return this._ScalarBuilder; }
            set { this._ScalarBuilder = value; }
        }

        /// <summary>
        /// Gets or sets the base matrix visitor
        /// </summary>
        public MatrixExpressionVisitor BaseMatrixExpressionVisitor
        {
            get { return this._MatrixBuilder; }
            set { this._MatrixBuilder = value; }
        }

        /// <summary>
        /// Base table visitor
        /// </summary>
        public TableExpressionVisitor BaseTableExpressionVisitor
        {
            get { return this._TableBuilder; }
            set { this._TableBuilder = value; }
        }

        // Visitor //

        // Declares //
        public override ActionExpression VisitDeclareScalar(PulseParser.DeclareScalarContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            int Size = ScriptingHelper.GetTypeSize(context.type());

            ScalarExpression se = (context.expression() == null ? ScalarExpression.Value(t) : this._ScalarBuilder.Render(context.expression()));

            return new ActionExpressionDeclareScalar(this._Host, this._Master, Lib, Name, t, se);

        }

        public override ActionExpression VisitDeclareMatrix1(PulseParser.DeclareMatrix1Context context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            int Size = ScriptingHelper.GetTypeSize(context.type());

            // Expressions //
            ScalarExpression row = this._ScalarBuilder.Render(context.expression()[0]);
            ScalarExpression col = (context.expression().Length == 1 ? ScalarExpression.OneINT : this._ScalarBuilder.Render(context.expression()[1]));


            return new ActionExpressionDeclareMatrix(this._Host, this._Master, Lib, Name, t, new MatrixExpressionNullLiteral(null, row, col, t));

        }

        public override ActionExpression VisitDeclareMatrix2(PulseParser.DeclareMatrix2Context context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            int Size = ScriptingHelper.GetTypeSize(context.type());

            // Expressions //
            MatrixExpression m = this._MatrixBuilder.Visit(context.matrix_expression());

            return new ActionExpressionDeclareMatrix(this._Host, this._Master, Lib, Name, t, m);

        }

        public override ActionExpression VisitDeclareTable(PulseParser.DeclareTableContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());

            TableExpression te = this._TableBuilder.Visit(context.table_expression());

            return new ActionExpressionTableAssign(this._Host, this._Master, Lib, Name, te);

        }

        // Assign //
        public override ActionExpression VisitActionAssignVariable(PulseParser.ActionAssignVariableContext context)
        {

            string LibTo = ScriptingHelper.GetLibName(context.var_name()[0]);
            string NameTo = ScriptingHelper.GetVarName(context.var_name()[0]);
            string LibFrom = ScriptingHelper.GetLibName(context.var_name()[1]);
            string NameFrom = ScriptingHelper.GetVarName(context.var_name()[1]);
            
            // Check for the table //
            if (this._Host.Connections.Exists(LibFrom) && this._Host.Connections.Exists(LibTo))
            {
                return new ActionExpressionTableAssign(this._Host, this._Master, LibTo, NameTo, new TableExpressionValue(this._Host, null, this._Host.OpenTable(LibFrom, NameFrom)));
            }

            // We need to make sure first that both libraries exist //
            if (!this._Host.Libraries.Exists(LibTo))
                throw new Exception(string.Format("Library '{0}' does not exist", LibTo));
            if (!this._Host.Libraries.Exists(LibFrom))
                throw new Exception(string.Format("Library '{0}' does not exist", LibFrom));

            // Get libraries //
            Library ToLibrary = this._Host.Libraries[LibTo];
            Library FromLibrary = this._Host.Libraries[LibFrom];

            // Find the scalar //
            if (ToLibrary.Values.Exists(NameTo) && FromLibrary.Values.Exists(NameFrom))
            {
                ScalarExpression x = new ScalarExpressionScalarRef(null, this._Host.Libraries.GetPointer(LibFrom), FromLibrary.Values.GetPointer(NameFrom), FromLibrary.Values[NameFrom].Affinity, FromLibrary.Values[NameFrom].DataCost);
                return new ActionExpressionScalarAssign(this._Host, this._Master, ToLibrary.Values, ToLibrary.Values.GetPointer(NameTo), x, Assignment.Equals); 
            }

            throw new Exception(string.Format("'{0}.{1}' or '{2}.{3}' is invalid", LibTo, NameTo, LibFrom, NameFrom));
            
        }

        public override ActionExpression VisitActionIncermentVariable(PulseParser.ActionIncermentVariableContext context)
        {

            string LibTo = ScriptingHelper.GetLibName(context.var_name()[0]);
            string NameTo = ScriptingHelper.GetVarName(context.var_name()[0]);
            string LibFrom = ScriptingHelper.GetLibName(context.var_name()[1]);
            string NameFrom = ScriptingHelper.GetVarName(context.var_name()[1]);

            // Check for the table //
            if (this._Host.Connections.Exists(LibFrom) && this._Host.Connections.Exists(LibTo))
            {
                using (RecordWriter w = this._Host.OpenTable(LibTo, NameTo).OpenWriter())
                {
                    return new ActionExpressionInsertSelect(this._Host, this._Master, w, new TableExpressionValue(this._Host, null, this._Host.OpenTable(LibFrom, NameFrom)));
                }
            }

            // We need to make sure first that both libraries exist //
            if (!this._Host.Libraries.Exists(LibTo))
                throw new Exception(string.Format("Library '{0}' does not exist", LibTo));
            if (!this._Host.Libraries.Exists(LibFrom))
                throw new Exception(string.Format("Library '{0}' does not exist", LibFrom));

            // Get libraries //
            Library ToLibrary = this._Host.Libraries[LibTo];
            Library FromLibrary = this._Host.Libraries[LibFrom];

            // Find the scalar //
            if (ToLibrary.Values.Exists(NameTo) && FromLibrary.Values.Exists(NameFrom))
            {
                ScalarExpression x = new ScalarExpressionScalarRef(null, this._Host.Libraries.GetPointer(LibFrom), FromLibrary.Values.GetPointer(NameFrom), FromLibrary.Values[NameFrom].Affinity, FromLibrary.Values[NameFrom].DataCost);
                return new ActionExpressionScalarAssign(this._Host, this._Master, ToLibrary.Values, ToLibrary.Values.GetPointer(NameTo), x, Assignment.PlusEquals);
            }

            throw new Exception(string.Format("'{0}.{1}' or '{2}.{3}' is invalid", LibTo, NameTo, LibFrom, NameFrom));
            

        }

        public override ActionExpression VisitActionScalarAssign(PulseParser.ActionScalarAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            Heap<Cell> scalars = this._Host.Libraries[Lib].Values;
            int ptr = scalars.GetPointer(Name);
            Assignment x = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionScalarAssign(this._Host, this._Master, scalars, ptr, this._ScalarBuilder.Render(context.expression()), x);

        }

        public override ActionExpression VisitActionScalarIncrement(PulseParser.ActionScalarIncrementContext context)
        {
            
            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            Heap<Cell> scalars = this._Host.Libraries[Lib].Values;
            int ptr = scalars.GetPointer(Name);

            Cell x = Cell.OneValue(scalars[Name].Affinity);
            if (context.increment().PLUS() != null)
            {
                return new ActionExpressionScalarAssign(this._Host, this._Master, scalars, ptr, ScalarExpression.Value(x), Assignment.PlusEquals);
            }
            else
            {
                return new ActionExpressionScalarAssign(this._Host, this._Master, scalars, ptr, ScalarExpression.Value(x), Assignment.MinusEquals);
            }
            

        }

        public override ActionExpression VisitActionMatrixUnit1DAssign(PulseParser.ActionMatrixUnit1DAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._ScalarBuilder.Render(context.expression()[0]);
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = this._ScalarBuilder.Render(context.expression()[1]);
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, mat, ptr, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit1DIncrement(PulseParser.ActionMatrixUnit1DIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._ScalarBuilder.Render(context.expression());
            ScalarExpression col = ScalarExpression.ZeroINT;
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            ScalarExpression val = ScalarExpression.Value(Cell.OneValue(mat[ptr].Affinity));

            if (context.increment().PLUS() != null)
            {
                return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, mat, ptr, row, col, val, Assignment.PlusEquals);
            }
            else
            {
                return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, mat, ptr, row, col, val, Assignment.MinusEquals);
            }

        }

        public override ActionExpression VisitActionMatrixUnit2DAssign(PulseParser.ActionMatrixUnit2DAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._ScalarBuilder.Render(context.expression()[0]);
            ScalarExpression col = this._ScalarBuilder.Render(context.expression()[1]);
            ScalarExpression val = this._ScalarBuilder.Render(context.expression()[2]);
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, mat, ptr, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit2DIncrement(PulseParser.ActionMatrixUnit2DIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._ScalarBuilder.Render(context.expression()[0]);
            ScalarExpression col = this._ScalarBuilder.Render(context.expression()[1]);
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            ScalarExpression val = ScalarExpression.Value(Cell.OneValue(mat[ptr].Affinity));

            if (context.increment().PLUS() != null)
            {
                return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, mat, ptr, row, col, val, Assignment.PlusEquals);
            }
            else
            {
                return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, mat, ptr, row, col, val, Assignment.MinusEquals);
            }

        }

        public override ActionExpression VisitActionMatrixUnitAllAssign(PulseParser.ActionMatrixUnitAllAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression val = this._ScalarBuilder.Render(context.expression());
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssignAll(this._Host, this._Master, mat, ptr, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnitAllIncrement(PulseParser.ActionMatrixUnitAllIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            ScalarExpression val = ScalarExpression.Value(Cell.OneValue(mat[ptr].Affinity));

            if (context.increment().PLUS() != null)
            {
                return new ActionExpressionMatrixUnitAssignAll(this._Host, this._Master, mat, ptr, val, Assignment.PlusEquals);
            }
            else
            {
                return new ActionExpressionMatrixUnitAssignAll(this._Host, this._Master, mat, ptr, val, Assignment.MinusEquals);
            }

        }

        public override ActionExpression VisitActionMatrixAssign(PulseParser.ActionMatrixAssignContext context)
        {
            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            MatrixExpression val = this._MatrixBuilder.Visit(context.matrix_expression());
            Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
            int ptr = mat.GetPointer(Name);
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixAssign(this._Host, this._Master, mat, ptr, val, asg);
        }

        public override ActionExpression VisitActionTableAssign(PulseParser.ActionTableAssignContext context)
        {
            
            string Lib = ScriptingHelper.GetLibName(context.var_name());
            if (Lib == Host.GLOBAL) Lib = Host.TEMP;
            string Name = ScriptingHelper.GetVarName(context.var_name());
            TableExpression t = this._TableBuilder.Visit(context.table_expression());
            return new ActionExpressionTableAssign(this._Host, this._Master, Lib, Name, t);

        }

        public override ActionExpression VisitActionTableIncrement1(PulseParser.ActionTableIncrement1Context context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            if (Lib == Host.GLOBAL) Lib = Host.TEMP;
            string Name = ScriptingHelper.GetVarName(context.var_name());
            TableExpression t = this._TableBuilder.Visit(context.table_expression());

            // Check for a writer //
            string key = TableHeader.DeriveV1Path(this._Host.Connections[Lib], Name);
            RecordWriter w = null;
            if (this._OpenRecordStreams.Exists(key))
            {
                w = this._OpenRecordStreams[key];
            }
            else
            {
                w = this._Host.OpenTable(Lib, Name).OpenWriter();
            }
            

            return new ActionExpressionInsertSelect(this._Host, this._Master, w, t);

        }

        public override ActionExpression VisitActionTableIncrement2(PulseParser.ActionTableIncrement2Context context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            if (Lib == Host.GLOBAL) Lib = Host.TEMP;
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpressionCollection select = this._ScalarBuilder.Render(context.expression_or_wildcard_set());

            // Check for a writer //
            string key = TableHeader.DeriveV1Path(this._Host.Connections[Lib], Name);
            RecordWriter w = null;
            if (this._OpenRecordStreams.Exists(key))
            {
                w = this._OpenRecordStreams[key];
            }
            else
            {
                w = this._Host.OpenTable(Lib, Name).OpenWriter();
            }


            return new ActionExpressionInsert(this._Host, this._Master, w, select);

        }

        // Prints //
        public override ActionExpression VisitActionPrintVariable(PulseParser.ActionPrintVariableContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            
            // Check if this is a table variable //
            if (this._ScalarBuilder.ColumnCube.Exists(Lib))
            {

                if (this._ScalarBuilder.ColumnCube[Lib].ColumnIndex(Name) > -1)
                {
                    int sidx = this._ScalarBuilder.ColumnCube.GetPointer(Lib);
                    int cidx = this._ScalarBuilder.ColumnCube[sidx].ColumnIndex(Name);
                    CellAffinity t = this._ScalarBuilder.ColumnCube[sidx].ColumnAffinity(cidx);
                    int s = this._ScalarBuilder.ColumnCube[sidx].ColumnSize(cidx);
                    ScalarExpressionFieldRef sef = new ScalarExpressionFieldRef(null, sidx, cidx, t, s);
                    ScalarExpressionCollection c = new ScalarExpressionCollection();
                    c.Add(Name, sef);
                    if (context.K_TO() != null)
                    {
                        ScalarExpression path = this._ScalarBuilder.Render(context.expression());
                        return new ActionExpressionPrintFile(this._Host, this._Master, c, this._OpenTextStreams, path);
                    }
                    return new ActionExpressionPrintConsole(this._Host, this._Master, c);

                }

            }

            // Check if this is a scalar //
            if (this._Host.Libraries.Exists(Lib))
            {
                
                ScalarExpressionCollection t = new ScalarExpressionCollection();
                ScalarExpression se = new ScalarExpressionScalarRef(null, this._Host.Libraries.GetPointer(Lib), this._Host.Libraries[Lib].Values.GetPointer(Name), this._Host.Libraries[Lib].Values[Name].Affinity, this._Host.Libraries[Lib].Values[Name].DataCost);
                t.Add(Name, se);
                if (context.K_TO() != null)
                {
                    ScalarExpression path = this._ScalarBuilder.Render(context.expression());
                    return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
                }
                return new ActionExpressionPrintConsole(this._Host, this._Master, t);

            }

            // Check if this is a table //
            if (string.Compare(Lib, Host.GLOBAL, true) == 0) 
                Lib = Host.TEMP;
            if (this._Host.TableExists(Lib, Name))
            {
                Table t = this._Host.OpenTable(Lib, Name);
                TableExpression te = new TableExpressionValue(this._Host, null, t);
                if (context.K_TO() != null)
                {
                    ScalarExpression path = this._ScalarBuilder.Render(context.expression());
                    return new ActionExpressionPrintFile(this._Host, this._Master, te, this._OpenTextStreams, path);
                }
                return new ActionExpressionPrintConsole(this._Host, this._Master, te);
            }

            // Otherwise throw an exception //
            throw new Exception(string.Format("Variable '{0}.{1}' does not exist", Lib, Name));

        }

        public override ActionExpression VisitActionPrintExpression(PulseParser.ActionPrintExpressionContext context)
        {

            ScalarExpressionCollection t = this._ScalarBuilder.Render(context.expression_or_wildcard_set());
            if (context.K_TO() != null)
            {
                ScalarExpression path = this._ScalarBuilder.Render(context.expression());
                return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
            }
            return new ActionExpressionPrintConsole(this._Host, this._Master, t);

        }

        public override ActionExpression VisitActionPrintMatrix(PulseParser.ActionPrintMatrixContext context)
        {

            MatrixExpression t = this._MatrixBuilder.Visit(context.matrix_expression());
            if (context.K_TO() != null)
            {
                ScalarExpression path = this._ScalarBuilder.Render(context.expression());
                return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
            }
            return new ActionExpressionPrintConsole(this._Host, this._Master, t);

        }

        public override ActionExpression VisitActionPrintTable(PulseParser.ActionPrintTableContext context)
        {

            TableExpression t = this._TableBuilder.Visit(context.table_expression());
            if (context.K_TO() != null)
            {
                ScalarExpression path = this._ScalarBuilder.Render(context.expression());
                return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
            }
            return new ActionExpressionPrintConsole(this._Host, this._Master, t);

        }

        // Chained -- do, for, while, if //
        public override ActionExpression VisitActionSet(PulseParser.ActionSetContext context)
        {
            ActionExpressionDo a = new ActionExpressionDo(this._Host, this._Master);

            foreach (PulseParser.Action_expressionContext ctx in context.action_expression())
            {
                a.AddChild(this.Visit(ctx));
            }

            this._Master = a;

            return a;
        }

        public override ActionExpression VisitActionDo(PulseParser.ActionDoContext context)
        {

            ActionExpressionDo a = new ActionExpressionDo(this._Host, this._Master);

            foreach (PulseParser.Action_expressionContext ctx in context.action_expression())
            {
                a.AddChild(this.Visit(ctx));
            }

            this._Master = a;

            return a;

        }

        public override ActionExpression VisitActionFor(PulseParser.ActionForContext context)
        {
            
            // Get the control var name //
            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());

            // If the library doesnt exist, then error out
            if (!this._Host.Libraries.Exists(Lib))
                throw new Exception(string.Format("Library '{0}' does note exist", Lib));

            // If the library exists, but the varible doesn't, add the variable //
            Heap<Cell> store = this._Host.Libraries[Lib].Values;
            if (context.type() != null)
            {
                CellAffinity q = ScriptingHelper.GetTypeAffinity(context.type());
                store.Allocate(Name, Cell.ZeroValue(q));
            }

            // Parse out the control expressions //
            ScalarExpression start = this._ScalarBuilder.Render(context.expression()[0]);
            ScalarExpression control = this._ScalarBuilder.Render(context.expression()[1]);
            ActionExpression increment = this.Visit(context.action_expression()[0]);

            ActionExpressionFor a = new ActionExpressionFor(this._Host, this._Master, store, store.GetPointer(Name), start, control, increment);

            if (context.action_expression().Length == 1)
                throw new Exception("For loops must contain at least one statement");

            for (int i = 1; i < context.action_expression().Length; i++)
            {
                ActionExpression ae = this.Visit(context.action_expression()[i]);
                a.AddChild(ae);
            }

            this._Master = a;

            return a;

        }

        public override ActionExpression VisitActionWhile(PulseParser.ActionWhileContext context)
        {

            ScalarExpression predicate = this._ScalarBuilder.Render(context.expression());
            ActionExpressionWhile act = new ActionExpressionWhile(this._Host, this._Master, predicate);

            // Load the children //
            foreach (PulseParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression ae = this.Visit(ctx);
                act.AddChild(ae);
            }

            this._Master = act;

            return act;

        }

        public override ActionExpression VisitActionIF(PulseParser.ActionIFContext context)
        {
            
            ScalarExpressionCollection sec = new ScalarExpressionCollection();
            foreach(PulseParser.ExpressionContext ctx in context.expression())
            {
                ScalarExpression se = this._ScalarBuilder.Render(ctx);
                sec.Add("X" + sec.Count.ToString(), se);
            }

            ActionExpression aei = new ActionExpressionIf(this._Host, this._Master, sec);

            foreach (PulseParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression ae = this.Visit(ctx);
                aei.AddChild(ae);
            }

            this._Master = aei;

            return aei;

        }

        /* Query
         *      Query is special because it involves possible layered contexes. Consider the following:
         *      
         *      QUERY TEMP.TEST1 AS A
         *      {
         *          
         *          // HERE, ONLY 'A' EXISTS //
         *          
         *          QUERY TEMP.TEST2 AS B
         *          {
         *          
         *              // BOTH 'A' AND 'B' EXIST HERE
         *              
         *              IF (A.KEY == B.KEY) THEN TEMP.TEST3 += { A.KEY, B.VALUE };
         *              
         *          }
         * 
         *          // WE'RE BACK TO ONLY 'A' EXISTING
         * 
         *      }
         * 
         *      
         *      TO SATISFY THIS CONSTRAINT, THE SCALAR AND MATRIX VISITORS NEED TO BE 
         * 
         */
        public override ActionExpression VisitActionQuery(PulseParser.ActionQueryContext context)
        {

            // Render the table expression //
            TableExpression t = this._TableBuilder.Visit(context.table_expression());
            string alias = context.K_AS() == null ? "T" : context.IDENTIFIER().GetText();

            // Save the scalar and matrix visitors //
            ScalarExpressionVisitor lsb = this._ScalarBuilder;
            MatrixExpressionVisitor lmb = this._MatrixBuilder;

            // Build the current context variables //
            this._ScalarBuilder = lsb.CloneOfMe();
            this._ScalarBuilder.AddSchema(alias, t.Columns);
            this._MatrixBuilder = new MatrixExpressionVisitor(this._Host, this._ScalarBuilder);
            this._TableBuilder.SeedVisitor = this._ScalarBuilder;

            // Now we can actually render the query! //
            ActionExpressionQuery aeq = new ActionExpressionQuery(this._Host, this._Master, t, alias, this._ScalarBuilder.ColumnCube.Count - 1);
            foreach (PulseParser.Action_expressionContext x in context.action_expression())
            {
                ActionExpression ae = this.Visit(x);
                aeq.AddChild(ae);
            }

            this._Master = aeq;

            // Step out of the context //
            this._ScalarBuilder = lsb;
            this._MatrixBuilder = lmb;
            this._TableBuilder.SeedVisitor = lsb;

            return aeq;

        }


    }

}
