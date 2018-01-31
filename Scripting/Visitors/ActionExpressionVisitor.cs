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

        private ScalarExpressionVisitor _sFactory;
        private MatrixExpressionVisitor _mFactory;
        private RecordExpressionVisitor _rFactory;
        private TableExpressionVisitor _tFactory;
        private ActionExpression _Master; 

        public ActionExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._OpenRecordStreams = new Heap<RecordWriter>();
            this._OpenTextStreams = new Heap<System.IO.StreamWriter>();
            this._sFactory = new ScalarExpressionVisitor(this._Host);
            this._mFactory = new MatrixExpressionVisitor(this._Host, this._sFactory);
            this._rFactory = new RecordExpressionVisitor(this._Host, this._sFactory);
            this._tFactory = new TableExpressionVisitor(this._Host, this._sFactory);
        }

        // Properties //
        public Host Host
        {
            get { return this._Host; }
        }

        public Heap<RecordWriter> OpenRecordStreams
        {
            get { return this._OpenRecordStreams; }
        }

        public Heap<System.IO.StreamWriter> OpenTextWriters
        {
            get { return this._OpenTextStreams; }
        }

        public override ActionExpression VisitDeclareScalar(PulseParser.DeclareScalarContext context)
        {
            string Lib = ScriptingHelper.GetLibName(context.scalar_name());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            ScalarExpression s = this._sFactory.Render(context.scalar_expression());
            return new ActionExpressionDeclareScalar(this._Host, this._Master, this._sFactory.Map[Lib], Name, s);
        }

        public override ActionExpression VisitDeclareMatrix(PulseParser.DeclareMatrixContext context)
        {
            string Lib = ScriptingHelper.GetLibName(context.matrix_name());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            MatrixExpression m = this._mFactory.Render(context.matrix_expression());
            return new ActionExpressionDeclareMatrix(this._Host, this._Master, this._sFactory.Map[Lib], Name, m);
        }

        public override ActionExpression VisitDeclareRecord(PulseParser.DeclareRecordContext context)
        {
            string Lib = ScriptingHelper.GetLibName(context.record_name());
            string Name = ScriptingHelper.GetVarName(context.record_name());
            RecordExpression x = this._rFactory.Render(context.record_expression());
            return new ActionExpressionDeclareRecord(this._Host, this._Master, this._sFactory.Map[Lib], Name, x);
        }

        public override ActionExpression VisitDeclareTable1(PulseParser.DeclareTable1Context context)
        {
            
            string Lib = ScriptingHelper.GetLibName(context.table_name());
            string Name = ScriptingHelper.GetVarName(context.table_name());
            Schema cols = new Schema();
            for (int i = 0; i < context.IDENTIFIER().Length; i++)
            {
                cols.Add(context.IDENTIFIER()[i].GetText(), ScriptingHelper.GetTypeAffinity(context.type()[i]), ScriptingHelper.GetTypeSize(context.type()[i]));
            }

            TableExpressionCTOR x = new TableExpressionCTOR(this._Host, null, cols, Lib, Name, new Key());

            ActionExpressionDeclareTable y = new ActionExpressionDeclareTable(this._Host, this._Master, Lib, Name, x);
            return y;

        }

        public override ActionExpression VisitDeclareTable2(PulseParser.DeclareTable2Context context)
        {
            string Lib = ScriptingHelper.GetLibName(context.table_name());
            string Name = ScriptingHelper.GetVarName(context.table_name());
            TableExpression x = this._tFactory.Render(context.table_expression());
            return new ActionExpressionDeclareTable(this._Host, this._Master, Lib, Name, x);
        }

        public override ActionExpression VisitActionScalarAssign(PulseParser.ActionScalarAssignContext context)
        {

            // Figure out what we're assigning //
            string Lib = ScriptingHelper.GetLibName(context.scalar_name());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            ObjectStore store = this._sFactory.Map[Lib];
            Assignment x = ScriptingHelper.GetAssignment(context.assignment());

            // Get the expression //
            ScalarExpression s = this._sFactory.Render(context.scalar_expression());
            return new ActionExpressionScalarAssign(this._Host, this._Master, store, Name, s, x);

        }

        public override ActionExpression VisitActionScalarIncrement(PulseParser.ActionScalarIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.scalar_name());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            ObjectStore store = this._Host.Store;
            Cell one = CellValues.One(store.Scalars[Name].Affinity);
            Assignment x = Assignment.PlusEquals;
            if (context.increment().PLUS() != null) x = Assignment.PlusEquals;

            return new ActionExpressionScalarAssign(this._Host, this._Master, store, Name, new ScalarExpressionConstant(null, one), x);

        }

        public override ActionExpression VisitActionMatrixUnit1DAssign(PulseParser.ActionMatrixUnit1DAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.matrix_name());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._sFactory.Render(context.scalar_expression()[0]);
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = this._sFactory.Render(context.scalar_expression()[1]);
            
            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit1DIncrement(PulseParser.ActionMatrixUnit1DIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.matrix_name());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._sFactory.Render(context.scalar_expression());
            ScalarExpression col = ScalarExpression.ZeroINT;
            ScalarExpression val = new ScalarExpressionConstant(null, CellValues.One(this._Host.Store.GetMatrix(Name).Affinity));

            Assignment asg = (context.increment().PLUS() == null ? Assignment.MinusEquals : Assignment.PlusEquals);

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);

        }

        public override ActionExpression VisitActionMatrixUnit2DAssign(PulseParser.ActionMatrixUnit2DAssignContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.matrix_name());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._sFactory.Render(context.scalar_expression()[0]);
            ScalarExpression col = this._sFactory.Render(context.scalar_expression()[1]);
            ScalarExpression val = this._sFactory.Render(context.scalar_expression()[2]);

            Assignment asg = ScriptingHelper.GetAssignment(context.assignment());

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);


        }

        public override ActionExpression VisitActionMatrixUnit2DIncrement(PulseParser.ActionMatrixUnit2DIncrementContext context)
        {

            string Lib = ScriptingHelper.GetLibName(context.matrix_name());
            string Name = ScriptingHelper.GetVarName(context.matrix_name());
            ScalarExpression row = this._sFactory.Render(context.scalar_expression()[0]);
            ScalarExpression col = this._sFactory.Render(context.scalar_expression()[1]);
            ScalarExpression val = new ScalarExpressionConstant(null, CellValues.One(this._Host.Store.GetMatrix(Name).Affinity));

            Assignment asg = (context.increment().PLUS() == null ? Assignment.MinusEquals : Assignment.PlusEquals);

            return new ActionExpressionMatrixUnitAssign(this._Host, this._Master, this._Host.Store, Name, row, col, val, asg);


        }

        public override ActionExpression VisitActionRecordUnitAssign(PulseParser.ActionRecordUnitAssignContext context)
        {

            string sName = ScriptingHelper.GetLibName(context.record_name());
            string rName = ScriptingHelper.GetVarName(context.record_name());
            string vName = context.IDENTIFIER().GetText();
            Assignment logic = ScriptingHelper.GetAssignment(context.assignment());
            ScalarExpression value = this._sFactory.Render(context.scalar_expression());

            ActionExpression a = new ActionExpressionRecordMemberAssign(this._Host, this._Master, sName, rName, vName, value, logic);
            this._Master = a;

            return a;

        }

        public override ActionExpression VisitActionRecordUnitIncrement(PulseParser.ActionRecordUnitIncrementContext context)
        {
            string sName = ScriptingHelper.GetLibName(context.record_name());
            string rName = ScriptingHelper.GetVarName(context.record_name());
            string vName = context.IDENTIFIER().GetText();
            CellAffinity q = this._sFactory.Map.Stores[sName].Records[rName].Columns.ColumnAffinity(vName);
            ScalarExpression value = new ScalarExpressionConstant(null, CellValues.One(q));
            Assignment logic = Assignment.PlusEquals;
            if (context.increment().PLUS() == null)
                logic = Assignment.MinusEquals;

            ActionExpression a = new ActionExpressionRecordMemberAssign(this._Host, this._Master, sName, rName, vName, value, logic);
            this._Master = a;

            return a;
        }

        public override ActionExpression VisitActionPrintScalar(PulseParser.ActionPrintScalarContext context)
        {
            ScalarExpression element = this._sFactory.Render(context.scalar_expression()[0]);
            ScalarExpression path = (context.scalar_expression().Length == 2) ? this._sFactory.Render(context.scalar_expression()[1]) : null;
            
            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionPrintMatrix(PulseParser.ActionPrintMatrixContext context)
        {
            MatrixExpression element = this._mFactory.Render(context.matrix_expression());
            ScalarExpression path = (context.scalar_expression() != null) ? this._sFactory.Render(context.scalar_expression()) : null;

            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionPrintRecord(PulseParser.ActionPrintRecordContext context)
        {
            RecordExpression element = this._rFactory.Render(context.record_expression());
            ScalarExpression path = (context.scalar_expression() != null) ? this._sFactory.Render(context.scalar_expression()) : null;

            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionPrintTable(PulseParser.ActionPrintTableContext context)
        {
            TableExpression element = this._tFactory.Render(context.table_expression());
            ScalarExpression path = (context.scalar_expression() != null) ? this._sFactory.Render(context.scalar_expression()) : null;

            if (path == null)
                return new ActionExpressionPrintConsole(this._Host, this._Master, element);
            else
                return new ActionExpressionPrintFile(this._Host, this._Master, element, this._OpenTextStreams, path);
        }

        public override ActionExpression VisitActionTableInsertRecord(PulseParser.ActionTableInsertRecordContext context)
        {

            Table t = this._tFactory.GetTable(context.table_name());

            ActionExpressionInsert x = new ActionExpressionInsert(this._Host, this._Master, t.OpenWriter(), this._rFactory.Render(context.record_expression()));
            return x;

        }

        public override ActionExpression VisitActionTableInsertTable(PulseParser.ActionTableInsertTableContext context)
        {

            Table t = this._tFactory.GetTable(context.table_name());
            TableExpression te = this._tFactory.Render(context.table_expression());
            using (RecordWriter w = t.OpenWriter())
            {
                return new ActionExpressionInsertSelect(this._Host, this._Master, w, te);
            }
        }

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
            string Lib = ScriptingHelper.GetLibName(context.scalar_name());
            string Name = ScriptingHelper.GetVarName(context.scalar_name());
            
            // If the library doesnt exist, then error out
            if (!this._sFactory.Map.StoreExists(Lib))
                throw new Exception(string.Format("Library '{0}' does not exist", Lib));

            // If the store exists, but the varible doesn't, add the variable //
            ScalarExpression start = this._sFactory.Render(context.scalar_expression()[0]);
            if (!this._sFactory.Map[Lib].Exists(Name, ObjectStore.ObjectAffinity.Scalar))
            {
                CellAffinity q = (context.type() == null) ? (start.ReturnAffinity()) : (ScriptingHelper.GetTypeAffinity(context.type()));
                this._sFactory.Map[Lib].DeclareScalar(Name, CellValues.Zero(q));
            }

            // Parse out the control expressions //
            ScalarExpression control = this._sFactory.Render(context.scalar_expression()[1]);
            ActionExpression increment = this.Visit(context.action_expression()[0]);

            ActionExpressionFor a = new ActionExpressionFor(this._Host, this._Master, this._sFactory.Map[Lib], Name, start, control, increment);

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

            ScalarExpression predicate = this._sFactory.Render(context.scalar_expression());
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

            ScalarExpressionSet sec = new ScalarExpressionSet(this._Host);
            foreach (PulseParser.Scalar_expressionContext ctx in context.scalar_expression())
            {
                ScalarExpression se = this._sFactory.Render(ctx);
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

        public override ActionExpression VisitActionCallSeq(PulseParser.ActionCallSeqContext context)
        {
            
            // Pull the action //
            string LibName = ScriptingHelper.GetLibName(context.scalar_name());
            string MethodName = ScriptingHelper.GetVarName(context.scalar_name());
            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library '{0}' does not exist", LibName));
            ActionExpressionParameterized x = this._Host.Libraries[LibName].ActionLookup(MethodName);
            ObjectFactory of = new ObjectFactory(this._Host, this._sFactory, this._mFactory, this._rFactory, this._tFactory);
            foreach (PulseParser.ParamContext ctx in context.param())
            {
                Parameter p = of.Render(ctx);
                x.AddParameter(p);
            }

            this._Master = x;
            return x;

        }

        // For Each //
        public override ActionExpression VisitActionForEachRecord(PulseParser.ActionForEachRecordContext context)
        {


            // Get the source table //
            TableExpression t = this._tFactory.Render(context.table_expression());
            string alias = context.record_name().IDENTIFIER().GetText();

            // Add the table to the map //
            this._sFactory.Map.Local.DeclareRecord(alias, new AssociativeRecord(t.Columns));

            // Now we can actually render the query! //
            ActionExpressionForEachTable aeq = new ActionExpressionForEachTable(this._Host, this._Master, t, alias);
            foreach (PulseParser.Action_expressionContext x in context.action_expression())
            {
                ActionExpression ae = this.Visit(x);
                aeq.AddChild(ae);
            }

            this._Master = aeq;

            // Step out of the context //
            this._sFactory.Map.Local.RemoveRecord(alias);

            return aeq;

        }

        public override ActionExpression VisitActionForEachMatrix(PulseParser.ActionForEachMatrixContext context)
        {

            string mLib = ScriptingHelper.GetLibName(context.matrix_name());
            string mName = ScriptingHelper.GetVarName(context.matrix_name());
            string sLib = ScriptingHelper.GetLibName(context.scalar_name());
            string sName = ScriptingHelper.GetVarName(context.scalar_name());

            Cell c = CellValues.Null(this._sFactory.Map.GetMatrix(mLib, mName).Affinity);
            this._sFactory.Map.DeclareScalar(sLib, sName, c);

            ActionExpression y = new ActionExpressionForEachMatrix(this._Host, this._Master, mLib, mName, sLib, sName);
            foreach (PulseParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression x = this.Visit(ctx);
                y.AddChild(x);
            }

            return y;

        }

        public override ActionExpression VisitActionForEachMatrixExpression(PulseParser.ActionForEachMatrixExpressionContext context)
        {

            string sLib = ScriptingHelper.GetLibName(context.scalar_name());
            string sName = ScriptingHelper.GetVarName(context.scalar_name());
            MatrixExpression m = this._mFactory.Render(context.matrix_expression());

            this._sFactory.Map.DeclareScalar(sLib, sName, CellValues.Null(m.ReturnAffinity()));

            ActionExpression y = new ActionExpressionForEachMatrixExpression(this._Host, this._Master, m, sLib, sName);
            foreach (PulseParser.Action_expressionContext ctx in context.action_expression())
            {
                ActionExpression x = this.Visit(ctx);
                y.AddChild(x);
            }

            return y;

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

        //    PulseParser.ParameterContext[] b = context.Select((x) => { return x.parameter(); }).ToArray();

        //    Parameters = this.Render(b);

        //}

    }

}
