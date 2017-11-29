using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.ActionExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Libraries;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Scripting
{

    public class ActionExpressionVisitor : PulseParserBaseVisitor<ActionExpression>
    {

        private Host _Host;
        private Heap<RecordWriter> _OpenRecordStreams;
        private Heap<System.IO.StreamWriter> _OpenTextStreams;

        private ExpressionVisitor _Expr;
        private ActionExpression _Master;

        public ActionExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._Expr = new ExpressionVisitor(this._Host);
            this._OpenRecordStreams = new Heap<RecordWriter>();
            this._OpenTextStreams = new Heap<System.IO.StreamWriter>();
        }

        // Properties //
        /// <summary>
        /// Gets or sets the base scalar visitor
        /// </summary>
        //public ScalarExpressionVisitor BaseScalarExpressionVisitor
        //{
        //    get { return this._ScalarBuilder; }
        //    set { this._ScalarBuilder = value; }
        //}

        /// <summary>
        /// Gets or sets the base matrix visitor
        /// </summary>
        //public MatrixExpressionVisitor BaseMatrixExpressionVisitor
        //{
        //    get { return this._MatrixBuilder; }
        //    set { this._MatrixBuilder = value; }
        //}

        /// <summary>
        /// Base table visitor
        /// </summary>
        //public TableExpressionVisitor BaseTableExpressionVisitor
        //{
        //    get { return this._TableBuilder; }
        //    set { this._TableBuilder = value; }
        //}

        // Visitor //

        // Declares //
        
        public override ActionExpression VisitDeclareScalar(PulseParser.DeclareScalarContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            int Size = ScriptingHelper.GetTypeSize(context.type());

            IExpression ie = (context.expr() == null ? ScalarExpression.Value(t) : this._Expr.Render(context.expr()));
            if (ie.SuperAffinity != SuperExpressionAffinity.Scalar)
                throw new Exception("Expecting a scalar");

            return new ActionExpressionDeclareScalar(this._Host, this._Master, this._Host.Store, Name, ie.Scalar);

        }

        public override ActionExpression VisitDeclareMatrix(PulseParser.DeclareMatrixContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            IExpression ie = this._Expr.Render(context.expr());
            if (ie.SuperAffinity != SuperExpressionAffinity.Matrix)
                throw new Exception("Expecting a matrix");

            return new ActionExpressionDeclareMatrix(this._Host, this._Master, this._Host.Store, Name, ie.Matrix);
        }

        public override ActionExpression VisitDeclareRecord(PulseParser.DeclareRecordContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            IExpression ie = this._Expr.Render(context.expr());
            if (ie.SuperAffinity != SuperExpressionAffinity.Record)
                throw new Exception("Expecting a record");

            return new ActionExpressionDeclareRecord(this._Host, this._Master, this._Host.Store, Name, ie.Record);

        }

        public override ActionExpression VisitDeclareTable(PulseParser.DeclareTableContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());

            IExpression ie = this._Expr.Render(context.expr());
            if (ie.SuperAffinity != SuperExpressionAffinity.Table)
                throw new Exception("Expecting a table");

            return new ActionExpressionDeclareTable(this._Host, this._Master, this._Host.Store, Name, ie.Table);

        }

        //public override ActionExpression VisitCTORTable(PulseParser.CTORTableContext context)
        //{

        //    string db = ScriptingHelper.GetLibName(context.table_name().var_name());
        //    string name = ScriptingHelper.GetVarName(context.table_name().var_name());

        //    Schema columns = new Schema();
        //    for (int i = 0; i < context.type().Length; i++)
        //    {
        //        CellAffinity type = ScriptingHelper.GetTypeAffinity(context.type()[i]);
        //        string field = context.IDENTIFIER()[i].GetText();
        //        int len = ScriptingHelper.GetTypeSize(context.type()[i]);
        //        columns.Add(field, type, len);
        //    }

        //    return new ActionExpressionTableCTOR(this._Host, this._Master, db, name, columns);

        //}

        // Assign //
        public override ActionExpression VisitActionScalarAssign(PulseParser.ActionScalarAssignContext context)
        {

            // Figure out what we're assigning //
            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ObjectStore store = this._Host.Store;
            Assignment x = ScriptingHelper.GetAssignment(context.assignment());

            // Get the expression //
            IExpression ie = this._Expr.Render(context.expr());

            // Table += Table or Table += Record //
            if (store.Exists(Name, ObjectStore.ObjectAffinity.Table))
            {

                string key = store.GetTable(Name);
                RecordWriter rw = null;
                if (this._OpenRecordStreams.Exists(key))
                {
                    rw = this._OpenRecordStreams[key];
                }
                else
                {
                    rw = this._Host.OpenTable(store.GetTable(Name)).OpenWriter();
                    this._OpenRecordStreams.Allocate(key, rw);
                }

                // Check the expression //
                if (ie.SuperAffinity == SuperExpressionAffinity.Table)
                {
                    return new ActionExpressionInsertSelect(this._Host, null, rw, ie.Table); 
                }
                else if (ie.SuperAffinity == SuperExpressionAffinity.Record)
                {
                    return new ActionExpressionInsert(this._Host, null, rw, ie.Record);
                }

                throw new Exception("Expecting either record or table expression");

            }

            if (ie.SuperAffinity != SuperExpressionAffinity.Scalar)
                throw new Exception("Expecting scalar");
            return new ActionExpressionScalarAssign(this._Host, this._Master, store, Name, ie.Scalar, x);

        }

        public override ActionExpression VisitActionScalarIncrement(PulseParser.ActionScalarIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ObjectStore store = this._Host.Store;
            Cell one = CellValues.One(store.Scalars[Name].Affinity);
            Assignment x = Assignment.PlusEquals;
            if (context.increment().PLUS() != null) x = Assignment.PlusEquals;

            return new ActionExpressionScalarAssign(this._Host, this._Master, store, Name, new ScalarExpressionConstant(null, one), x);

        }

        public override ActionExpression VisitActionMatrixUnit1DAssign(PulseParser.ActionMatrixUnit1DAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._Expr.Render(context.expr()[0]).Scalar;
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = this._Expr.Render(context.expr()[1]).Scalar;
            
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit1DIncrement(PulseParser.ActionMatrixUnit1DIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._Expr.Render(context.expr()).Scalar;
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = new ScalarExpressionConstant(null, CellValues.One(this._Host.Store.GetMatrix(Name).Affinity));

            Assignment asg = (context.increment().PLUS() == null ? Assignment.MinusEquals : Assignment.PlusEquals);

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit2DAssign(PulseParser.ActionMatrixUnit2DAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._Expr.Render(context.expr()[0]).Scalar;
            ScalarExpression col = this._Expr.Render(context.expr()[1]).Scalar;
            ScalarExpression val = this._Expr.Render(context.expr()[2]).Scalar;

            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);


        }

        public override ActionExpression VisitActionMatrixUnit2DIncrement(PulseParser.ActionMatrixUnit2DIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.var_name());
            string Name = ScriptingHelper.GetVarName(context.var_name());
            ScalarExpression row = this._Expr.Render(context.expr()[0]).Scalar;
            ScalarExpression col = this._Expr.Render(context.expr()[1]).Scalar;
            ScalarExpression val = new ScalarExpressionConstant(null, CellValues.One(this._Host.Store.GetMatrix(Name).Affinity));

            Assignment asg = (context.increment().PLUS() == null ? Assignment.MinusEquals : Assignment.PlusEquals);

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);


        }

        //public override ActionExpression VisitActionMatrixUnitAllAssign(PulseParser.ActionMatrixUnitAllAssignContext context)
        //{

        //    string Lib = ScriptingHelper.GetLibName(context.matrix_name().var_name());
        //    string Name = ScriptingHelper.GetVarName(context.matrix_name().var_name());
        //    ScalarExpression val = this._ScalarBuilder.Render(context.scalar_expression());
        //    Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
        //    int ptr = mat.GetPointer(Name);
        //    Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

        //    return new ActionExpressionMatrixUnitAssignAll(this._Host, this._Master, mat, ptr, val, asg);

        //}

        //public override ActionExpression VisitActionMatrixUnitAllIncrement(PulseParser.ActionMatrixUnitAllIncrementContext context)
        //{

        //    string Lib = ScriptingHelper.GetLibName(context.matrix_name().var_name());
        //    string Name = ScriptingHelper.GetVarName(context.matrix_name().var_name());
        //    Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
        //    int ptr = mat.GetPointer(Name);
        //    ScalarExpression val = ScalarExpression.Value(CellValues.One(mat[ptr].Affinity));

        //    if (context.increment().PLUS() != null)
        //    {
        //        return new ActionExpressionMatrixUnitAssignAll(this._Host, this._Master, mat, ptr, val, Assignment.PlusEquals);
        //    }
        //    else
        //    {
        //        return new ActionExpressionMatrixUnitAssignAll(this._Host, this._Master, mat, ptr, val, Assignment.MinusEquals);
        //    }

        //}

        //public override ActionExpression VisitActionMatrixAssign(PulseParser.ActionMatrixAssignContext context)
        //{
        //    string Lib = ScriptingHelper.GetLibName(context.matrix_name().var_name());
        //    string Name = ScriptingHelper.GetVarName(context.matrix_name().var_name());
        //    MatrixExpression val = this._MatrixBuilder.Visit(context.matrix_expression());
        //    Heap<CellMatrix> mat = this._Host.Libraries[Lib].Matrixes;
        //    int ptr = mat.GetPointer(Name);
        //    Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

        //    return new ActionExpressionMatrixAssign(this._Host, this._Master, mat, ptr, val, asg);
        //}

        //public override ActionExpression VisitActionTableAssign(PulseParser.ActionTableAssignContext context)
        //{
            
        //    string Lib = ScriptingHelper.GetLibName(context.table_name().var_name());
        //    if (Lib == Host.GLOBAL) Lib = Host.TEMP;
        //    string Name = ScriptingHelper.GetVarName(context.table_name().var_name());
        //    TableExpression t = this._TableBuilder.Visit(context.table_expression());
        //    return new ActionExpressionTableAssign(this._Host, this._Master, Lib, Name, t);

        //}

        //public override ActionExpression VisitActionTableIncrement1(PulseParser.ActionTableIncrement1Context context)
        //{

        //    string Lib = ScriptingHelper.GetLibName(context.table_name().var_name());
        //    if (Lib == Host.GLOBAL) Lib = Host.TEMP;
        //    string Name = ScriptingHelper.GetVarName(context.table_name().var_name());
        //    TableExpression t = this._TableBuilder.Visit(context.table_expression());

        //    // Check for a writer //
        //    string key = TableHeader.DeriveV1Path(this._Host.Connections[Lib], Name);
        //    RecordWriter w = null;
        //    if (this._OpenRecordStreams.Exists(key))
        //    {
        //        w = this._OpenRecordStreams[key];
        //    }
        //    else
        //    {
        //        w = this._Host.OpenTable(Lib, Name).OpenWriter();
        //    }
            

        //    return new ActionExpressionInsertSelect(this._Host, this._Master, w, t);

        //}

        //public override ActionExpression VisitActionTableIncrement2(PulseParser.ActionTableIncrement2Context context)
        //{

        //    string Lib = ScriptingHelper.GetLibName(context.table_name().var_name());
        //    if (Lib == Host.GLOBAL) Lib = Host.TEMP;
        //    string Name = ScriptingHelper.GetVarName(context.table_name().var_name());
        //    ScalarExpressionSet select = RecordExpressionVisitor.Render(this._ScalarBuilder, context.record_expression());

        //    // Check for a writer //
        //    string key = TableHeader.DeriveV1Path(this._Host.Connections[Lib], Name);
        //    RecordWriter w = null;
        //    if (this._OpenRecordStreams.Exists(key))
        //    {
        //        w = this._OpenRecordStreams[key];
        //    }
        //    else
        //    {
        //        w = this._Host.OpenTable(Lib, Name).OpenWriter();
        //    }


        //    return new ActionExpressionInsert(this._Host, this._Master, w, select);

        //}

        // Prints //
        public override ActionExpression VisitActionPrint(PulseParser.ActionPrintContext context)
        {

            IExpression element = this._Expr.Render(context.expr()[0]);
            IExpression path = null;
            if (context.expr().Length == 2)
            {
                path = this._Expr.Render(context.expr()[1]);
                if (path.SuperAffinity != SuperExpressionAffinity.Scalar)
                    throw new Exception("Expecting a scalar");
            }
            
            if (element.SuperAffinity == SuperExpressionAffinity.Scalar)
            {
                if (path == null)
                    return new ActionExpressionPrintConsole(this._Host, this._Master, element.Scalar);
                else
                    return new ActionExpressionPrintFile(this._Host, this._Master, element.Scalar, this._OpenTextStreams, path.Scalar);
            }
            else if (element.SuperAffinity == SuperExpressionAffinity.Matrix)
            {
                if (path == null)
                    return new ActionExpressionPrintConsole(this._Host, this._Master, element.Matrix);
                else
                    return new ActionExpressionPrintFile(this._Host, this._Master, element.Matrix, this._OpenTextStreams, path.Scalar);
            }
            else if (element.SuperAffinity == SuperExpressionAffinity.Record)
            {
                if (path == null)
                    return new ActionExpressionPrintConsole(this._Host, this._Master, element.Record);
                else
                    return new ActionExpressionPrintFile(this._Host, this._Master, element.Record, this._OpenTextStreams, path.Scalar);
            }
            else if (element.SuperAffinity == SuperExpressionAffinity.Table)
            {
                if (path == null)
                    return new ActionExpressionPrintConsole(this._Host, this._Master, element.Table);
                else
                    return new ActionExpressionPrintFile(this._Host, this._Master, element.Table, this._OpenTextStreams, path.Scalar);
            }

            throw new Exception();

        }

        //public override ActionExpression VisitActionPrintMatrix(PulseParser.ActionPrintMatrixContext context)
        //{

        //    MatrixExpression t = this._MatrixBuilder.Visit(context.matrix_expression());
        //    if (context.K_TO() != null)
        //    {
        //        ScalarExpression path = this._ScalarBuilder.Render(context.scalar_expression());
        //        return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
        //    }
        //    return new ActionExpressionPrintConsole(this._Host, this._Master, t);

        //}

        //public override ActionExpression VisitActionPrintRecord(PulseParser.ActionPrintRecordContext context)
        //{

        //    ScalarExpressionSet t = RecordExpressionVisitor.Render(this._ScalarBuilder, context.record_expression());
        //    if (context.K_TO() != null)
        //    {
        //        ScalarExpression path = this._ScalarBuilder.Render(context.scalar_expression());
        //        return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
        //    }
        //    return new ActionExpressionPrintConsole(this._Host, this._Master, t);

        //}

        //public override ActionExpression VisitActionPrintTable(PulseParser.ActionPrintTableContext context)
        //{

        //    TableExpression t = this._TableBuilder.Visit(context.table_expression());
        //    if (context.K_TO() != null)
        //    {
        //        ScalarExpression path = this._ScalarBuilder.Render(context.scalar_expression());
        //        return new ActionExpressionPrintFile(this._Host, this._Master, t, this._OpenTextStreams, path);
        //    }
        //    return new ActionExpressionPrintConsole(this._Host, this._Master, t);

        //}


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
            if (!this._Expr.Map.StoreExists(Lib))
                throw new Exception(string.Format("Library '{0}' does not exist", Lib));

            // If the library exists, but the varible doesn't, add the variable //
            if (!this._Expr.Map[Lib].Exists(Name, ObjectStore.ObjectAffinity.Scalar))
            {
                CellAffinity q = ScriptingHelper.GetTypeAffinity(context.type());
                this._Expr.Map[Lib].DeclareScalar(Name, CellValues.Zero(q));
            }

            // Parse out the control expressions //
            ScalarExpression start = this._Expr.Render(context.expr()[0]).Scalar;
            if (start == null) 
                throw new Exception("Starting expression must be a scalar");
            ScalarExpression control = this._Expr.Render(context.expr()[1]).Scalar;
            if (control == null) 
                throw new Exception("Control expression must be a scalar");
            ActionExpression increment = this.Visit(context.action_expression()[0]);

            ActionExpressionFor a = new ActionExpressionFor(this._Host, this._Master, this._Expr.Map[Lib], Name, start, control, increment);

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

            ScalarExpression predicate = this._Expr.Render(context.expr()).Scalar;
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

            ScalarExpressionSet sec = new ScalarExpressionSet();
            foreach (PulseParser.ExprContext ctx in context.expr())
            {
                ScalarExpression se = this._Expr.Render(ctx).Scalar;
                if (se == null) throw new Exception("Expecting a scalar expression");
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

        // For Each //
        public override ActionExpression VisitActionForEach(PulseParser.ActionForEachContext context)
        {


            // Get the source table //
            TableExpression t = this._Expr.Render(context.expr()).Table;
            if (t == null) throw new Exception("Expecting a table");
            string alias = context.IDENTIFIER().GetText();

            // Add the table to the map //
            this._Expr.Map.Local.DeclareRecord(alias, new AssociativeRecord(t.Columns));

            // Now we can actually render the query! //
            ActionExpressionForEach aeq = new ActionExpressionForEach(this._Host, this._Master, t, alias);
            foreach (PulseParser.Action_expressionContext x in context.action_expression())
            {
                ActionExpression ae = this.Visit(x);
                aeq.AddChild(ae);
            }

            this._Master = aeq;

            // Step out of the context //
            this._Expr.Map.Local.RemoveRecord(alias);

            return aeq;

        }

        // Method calls //
        //public override ActionExpression VisitActionCallNamed(PulseParser.ActionCallNamedContext context)
        //{

        //    string lib = ScriptingHelper.GetLibName(context.var_name());
        //    string name = ScriptingHelper.GetVarName(context.var_name());

        //    ActionExpressionParameterized x = this._Host.Libraries[lib].ActionLookup(name);

        //    ScalarExpressionVisitor s = this._ScalarBuilder;
        //    this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();

        //    string[] names = new string[] { };
        //    ActionExpressionParameterized.Parameter[] p = new ActionExpressionParameterized.Parameter[] { };

        //    this.Render(context.parameter_name(), out p, out names);

        //    for (int i = 0; i < names.Length; i++)
        //    {
        //        x.AddParameter(names[i], p[i]);
        //    }
        //    this._ScalarBuilder = s;

        //    return x;

        //}

        //public override ActionExpression VisitActionCallSeq(PulseParser.ActionCallSeqContext context)
        //{

        //    string lib = ScriptingHelper.GetLibName(context.var_name());
        //    string name = ScriptingHelper.GetVarName(context.var_name());

        //    if (!this._Host.Libraries.Exists(lib))
        //        throw new Exception(string.Format("Library '{0}' does not exist", lib));
        //    if (!this._Host.Libraries[lib].ActionExists(name))
        //        throw new Exception(string.Format("Action '{0}' does not exist in '{1}'", name, lib));

        //    ActionExpressionParameterized x = this._Host.Libraries[lib].ActionLookup(name);

        //    ScalarExpressionVisitor s = this._ScalarBuilder;
        //    this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();

        //    foreach (ActionExpressionParameterized.Parameter p in this.Render(context.parameter()))
        //    {
        //        x.AddParameter(p);
        //    }
        //    this._ScalarBuilder = s;

        //    return x;

        //}

        //private bool IsTabular(PulseParser.ParameterContext context)
        //{
        //    if (context.var_name() != null)
        //    {
        //        string lib = ScriptingHelper.GetLibName(context.var_name());
        //        string name = ScriptingHelper.GetVarName(context.var_name());
        //        return this._Host.TableExists(lib, name);
        //    }
        //    else if (context.table_expression() != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //private ActionExpressionParameterized.Parameter Render(PulseParser.ParameterContext context)
        //{

        //    // Var Name //
        //    if (context.var_name() != null)
        //    {

        //        string lib = ScriptingHelper.GetLibName(context.var_name());
        //        string name = ScriptingHelper.GetVarName(context.var_name());

        //        if (this._Host.TableExists(lib, name))
        //        {

        //            TableExpression t = new TableExpressionValue(this._Host, null, this._Host.OpenTable(lib, name));
        //            ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(t);
        //            int idx = this._ScalarBuilder.ColumnCube.Count;
        //            this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();
        //            this._ScalarBuilder.AddSchema(t.Alias, t.Columns);
        //            this._TableBuilder.SeedVisitor = this._ScalarBuilder;
        //            this._MatrixBuilder.SeedVisitor = this._ScalarBuilder;
        //            p.HeapRef = idx;
        //            return p;

        //        }
        //        else if (this._Host.ScalarExists(lib, name))
        //        {

        //            int href = this._Host.Libraries.GetPointer(lib);
        //            int sref = this._Host.Libraries[href].Values.GetPointer(name);
        //            CellAffinity stype = this._Host.Libraries[href].Values[sref].Affinity;
        //            int ssize = CellSerializer.Length(this._Host.Libraries[href].Values[sref]);
        //            ScalarExpressionStoreRef x = new ScalarExpressionStoreRef(null, href, sref, stype, ssize);
        //            ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(x);
        //            return p;
        //        }
        //        else if (this._ScalarBuilder.ColumnCube.Exists(lib))
        //        {

        //            int href = this._ScalarBuilder.ColumnCube.GetPointer(lib);
        //            int sref = this._ScalarBuilder.ColumnCube[lib].ColumnIndex(name);
        //            if (sref == -1) throw new Exception(string.Format("Field '{0}' does not exist", name));
        //            CellAffinity stype = this._ScalarBuilder.ColumnCube[lib].ColumnAffinity(sref);
        //            int ssize = this._ScalarBuilder.ColumnCube[lib].ColumnSize(sref);
        //            ScalarExpressionFieldRef x = new ScalarExpressionFieldRef(null, href, sref, stype, ssize);
        //            ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(x);
        //            return p;

        //        }

        //        throw new Exception(string.Format("Table or value '{0}.{1}' does not exist", lib, name));

        //    }
        //    // Table expression //
        //    else if (context.table_expression() != null)
        //    {

        //        TableExpression t = this._TableBuilder.Visit(context.table_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(t);
        //        int idx = this._ScalarBuilder.ColumnCube.Count;
        //        this._ScalarBuilder = this._ScalarBuilder.CloneOfMe();
        //        this._ScalarBuilder.AddSchema(t.Alias, t.Columns);
        //        this._TableBuilder.SeedVisitor = this._ScalarBuilder;
        //        this._MatrixBuilder.SeedVisitor = this._ScalarBuilder;
        //        p.HeapRef = idx;
        //        return p;

        //    }
        //    // Matrix expression //
        //    else if (context.matrix_expression() != null)
        //    {

        //        MatrixExpression m = this._MatrixBuilder.Visit(context.matrix_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(m);
        //        return p;

        //    }
        //    // Record //
        //    else if (context.record_expression() != null)
        //    {

        //        ScalarExpressionSet r = RecordExpressionVisitor.Render(this._ScalarBuilder, context.record_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(r);
        //        return p;

        //    }
        //    else if (context.scalar_expression() != null)
        //    {
        //        ScalarExpression x = this._ScalarBuilder.Render(context.scalar_expression());
        //        ActionExpressionParameterized.Parameter p = new ActionExpressionParameterized.Parameter(x);
        //        return p;
        //    }

        //    throw new Exception(string.Format("Parameter '{0}' is invalid", context.GetText()));
            
        //}

        //private ActionExpressionParameterized.Parameter[] Render(PulseParser.ParameterContext[] context)
        //{

        //    if (context.Length == 0)
        //        return new ActionExpressionParameterized.Parameter[] { };

        //    ActionExpressionParameterized.Parameter[] s = new ActionExpressionParameterized.Parameter[context.Length];
        //    Queue<PulseParser.ParameterContext> p = new Queue<PulseParser.ParameterContext>();
        //    Queue<int> q = new Queue<int>();
        //    int i = 0;

        //    foreach (PulseParser.ParameterContext ctx in context)
        //    {

        //        if (IsTabular(ctx))
        //        {
        //            s[i] = this.Render(ctx);
        //        }
        //        else
        //        {
        //            p.Enqueue(ctx);
        //            q.Enqueue(i);
        //        }
        //        i++;

        //    }

        //    while (p.Count != 0)
        //    {
        //        s[q.Dequeue()] = this.Render(p.Dequeue());
        //    }

        //    return s;

        //}

        //private void Render(PulseParser.Parameter_nameContext[] context, out ActionExpressionParameterized.Parameter[] Parameters, out string[] Names)
        //{

        //    Names = context.Select((x) => { return x.lib_name().GetText(); }).ToArray();

        //    PulseParser.ParameterContext[] c = context.Select((x) => { return x.parameter(); }).ToArray();

        //    Parameters = this.Render(c);

        //}

    }

}
