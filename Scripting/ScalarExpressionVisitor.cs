using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.Libraries;
using Pulse.Aggregates;

namespace Pulse.Scripting
{

    public class ScalarExpressionVisitor : PulseParserBaseVisitor<ScalarExpression>
    {

        public const string STRING_TAG1 = "'";
        public const string STRING_TAG2 = "\"";
        public const string STRING_TAG3 = "$$";
        public const string STRING_TAB = "TAB";
        public const string STRING_CRLF = "CRLF";
        public const string DATE_TAG1 = "T";
        public const string DATE_TAG2 = "t";
        public const string NUM_TAG1 = "D";
        public const string NUM_TAG2 = "d";

        private Host _Host;
        private Heap<Schema> _Columns;
        private Heap<int> _PointerSize;
        private Heap<CellAffinity> _PointerAffinity;
        private IScalarExpressionLookup _BaseFunctionLibrary;
        private Heap<Library> _Libraries;
        private ScalarExpression _Master;

        public ScalarExpressionVisitor(Host Host)
            :base()
        {
            this._Host = Host;
            this._Columns = new Data.Heap<Data.Schema>();
            this._PointerSize = new Data.Heap<int>();
            this._PointerAffinity = new Data.Heap<Data.CellAffinity>();
            this._BaseFunctionLibrary = Host.BaseLibrary.FunctionSet;
            this._Libraries = Host.Libraries;
        }

        // Properties //
        public FieldResolver ImpliedResolver
        {

            get
            {
                
                FieldResolver fr = new FieldResolver(this._Host);
                
                // Add columns //
                foreach (KeyValuePair<string, Schema> kv in this._Columns.Entries)
                {
                    fr.AddSchema(kv.Key, kv.Value);
                }

                // Add libraries //
                foreach (KeyValuePair<string, Library> kv in this._Libraries.Entries)
                {

                    if (!fr.AliasExists(kv.Key))
                    {
                        fr.AddLibrary(kv.Key, kv.Value);
                    }
                    
                }

                return fr;

            }

        }

        public Heap<Schema> ColumnCube
        {
            get { return this._Columns; }
        }

        // Support Filed Methods //
        public void AddSchema(string Alias, Schema Columns, out int Pointer)
        {
            this._Columns.Allocate(Alias, Columns);
            Pointer = this._Columns.Count - 1;
        }

        public void AddSchema(string Alias, Schema Columns)
        {
            int x = 0;
            this.AddSchema(Alias, Columns, out x);
        }

        public void AddPointer(string Alias, CellAffinity Affinity, int Size)
        {
            this._PointerSize.Allocate(Alias, Size);
            this._PointerAffinity.Allocate(Alias, Affinity);
        }

        // Tree walkers //
        public override ScalarExpression VisitPointer(PulseParser.PointerContext context)
        {
            
            // Get the name, size and affinity //
            string Name = context.IDENTIFIER().GetText();
            int Size = ScriptingHelper.GetTypeSize(context.type());
            CellAffinity Type = ScriptingHelper.GetTypeAffinity(context.type());

            return new ScalarExpressionPointer(this._Master, Name, Type, Size);

        }

        public override ScalarExpression VisitUniary(PulseParser.UniaryContext context)
        {

            ScalarExpression a = this.Visit(context.expression());
            a.ParentNode = this._Master;
            
            ScalarExpressionFunction f = null;
            if (context.PLUS() != null)
                f = new ScalarExpressionFunction.ExpressionPlus();
            else if (context.MINUS() != null)
                f = new ScalarExpressionFunction.ExpressionMinus();
            else if (context.NOT() != null)
                f = new ScalarExpressionFunction.ExpressionNot();
            else
                throw new Exception("Unknow opperation");
            
            f.AddChildren(a);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitPower(PulseParser.PowerContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpressionFunction.ExpressionPower f = new ScalarExpressionFunction.ExpressionPower();
            f.AddChildren(a, b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitMultDivMod(PulseParser.MultDivModContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.MUL() != null)
                f = new ScalarExpressionFunction.ExpressionMultiply();
            else if (context.DIV() != null)
                f = new ScalarExpressionFunction.ExpressionDivide();
            else if (context.MOD() != null)
                f = new ScalarExpressionFunction.ExpressionModulo();
            else if (context.DIV2() != null)
                f = new ScalarExpressionFunction.ExpressionCheckedDivide();
            else
                throw new Exception("Unknow opperation");

            f.AddChildren(a, b);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitAddSub(PulseParser.AddSubContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.PLUS() != null)
                f = new ScalarExpressionFunction.ExpressionAdd();
            else if (context.MINUS() != null)
                f = new ScalarExpressionFunction.ExpressionSubtract();
            else
                throw new Exception("Unknow opperation");

            f.AddChildren(a, b);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitGreaterLesser(PulseParser.GreaterLesserContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.GT() != null)
                f = new ScalarExpressionFunction.ExpressionGreaterThan();
            else if (context.LT() != null)
                f = new ScalarExpressionFunction.ExpressionLessThan();
            else if (context.GTE() != null)
                f = new ScalarExpressionFunction.ExpressionGreaterThanOrEqualTo();
            else if (context.LTE() != null)
                f = new ScalarExpressionFunction.ExpressionLessThanOrEqualTo();
            else
                throw new Exception("Unknow opperation");

            f.AddChildren(a, b);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitLogicalAnd(PulseParser.LogicalAndContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = new ScalarExpressionFunction.ExpressionAnd();

            f.AddChildren(a, b);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitEquality(PulseParser.EqualityContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.EQ() != null)
                f = new ScalarExpressionFunction.ExpressionEquals();
            else if (context.NEQ() != null)
                f = new ScalarExpressionFunction.ExpressionNotEquals();
            else
                throw new Exception("Unknow opperation");

            f.AddChildren(a, b);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitLogicalOr(PulseParser.LogicalOrContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.OR() != null)
                f = new ScalarExpressionFunction.ExpressionOr();
            else if (context.XOR() != null)
                f = new ScalarExpressionFunction.ExpressionXor();
            else
                throw new Exception("Unknow opperation");

            f.AddChildren(a, b);
            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitNakedVariable(PulseParser.NakedVariableContext context)
        {

            string Value = context.IDENTIFIER().GetText();
            int FieldRef = 0;

            // Table Fields //
            for (int i = 0; i < this._Columns.Count; i++)
            {

                // Find the field in the table //
                FieldRef = this._Columns[i].ColumnIndex(Value);
                if (FieldRef != -1)
                {
                    return new ScalarExpressionFieldRef(this._Master, i, FieldRef, this._Columns[i].ColumnAffinity(FieldRef), this._Columns[i].ColumnSize(FieldRef));
                }

            }

            // Heap Fields //
            for (int i = 0; i < this._Libraries.Count; i++)
            {

                // Find the field in the table //
                FieldRef = this._Libraries[i].Values.GetPointer(Value);
                if (FieldRef != -1)
                {
                    return new ScalarExpressionScalarRef(this._Master, i, FieldRef, this._Libraries[i].Values[FieldRef].Affinity, this._Libraries[i].Values[FieldRef].DataCost);
                }

            }

            throw new Exception(string.Format("Field not found '{0}'", Value));

        }

        public override ScalarExpression VisitSpecificVariable(PulseParser.SpecificVariableContext context)
        {

            string Alias = context.IDENTIFIER()[0].GetText();
            string Value = context.IDENTIFIER()[1].GetText();
            int MainRef = 0;
            int FieldRef = 0;

            // Table Fields //
            for (int i = 0; i < this._Columns.Count; i++)
            {
                
                // Match found //
                if (StringComparer.OrdinalIgnoreCase.Compare(this._Columns.Name(i), Alias) == 0)
                {
                   
                    // Set the main ref //
                    MainRef = i;

                    // Find the field in the table //
                    FieldRef = this._Columns[i].ColumnIndex(Value);
                    if (FieldRef == -1)
                        throw new Exception(string.Format("Column '{0}' not found in '{1}'", Value, Alias));

                    // Create the final expression //
                    return new ScalarExpressionFieldRef(this._Master, MainRef, FieldRef, this._Columns[i].ColumnAffinity(FieldRef), this._Columns[i].ColumnSize(FieldRef));

                }

            }

            // Heap Fields //
            for (int i = 0; i < this._Libraries.Count; i++)
            {

                // Match found //
                if (StringComparer.OrdinalIgnoreCase.Compare(this._Libraries[i].Name, Alias) == 0)
                {

                    // Set the main ref //
                    MainRef = i;

                    // Find the field in the table //
                    FieldRef = this._Libraries[i].Values.GetPointer(Value);
                    if (FieldRef == -1)
                        throw new Exception(string.Format("Field '{0}' not found in '{1}'", Value, Alias));

                    // Create the final expression //
                    return new ScalarExpressionScalarRef(this._Master, i + 1, FieldRef, this._Libraries[i].Values[FieldRef].Affinity, this._Libraries[i].Values[FieldRef].DataCost);

                }

            }

            throw new Exception(string.Format("Table or library not found '{0}'", Alias));

        }

        public override ScalarExpression VisitLiteralBool(PulseParser.LiteralBoolContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, Cell.Parse(s, CellAffinity.BOOL));
        }

        public override ScalarExpression VisitLiteralInt(PulseParser.LiteralIntContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, Cell.Parse(s, CellAffinity.INT));
        }

        public override ScalarExpression VisitLiteralDouble(PulseParser.LiteralDoubleContext context)
        {
            string s = context.GetText();
            s = s.Replace("D", "").Replace("d", "");
            return new ScalarExpressionConstant(this._Master, Cell.Parse(s, CellAffinity.DOUBLE));
        }

        public override ScalarExpression VisitLiteralDate(PulseParser.LiteralDateContext context)
        {
            string s = context.GetText();
            s = s.Replace("T", "").Replace("t", "");
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, Cell.Parse(s, CellAffinity.DATE_TIME));
        }

        public override ScalarExpression VisitLiteralString(PulseParser.LiteralStringContext context)
        {
            string s = context.GetText();
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, Cell.Parse(s, CellAffinity.STRING));
        }

        public override ScalarExpression VisitLiteralBLOB(PulseParser.LiteralBLOBContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, Cell.Parse(s, CellAffinity.BLOB));
        }

        public override ScalarExpression VisitLiteralNull(PulseParser.LiteralNullContext context)
        {
            return new ScalarExpressionConstant(this._Master, Cell.NULL_INT);
        }

        public override ScalarExpression VisitExpressionType(PulseParser.ExpressionTypeContext context)
        {
            CellAffinity x = ScriptingHelper.GetTypeAffinity(context.type());
            byte y = (byte)x;
            return new ScalarExpressionConstant(this._Master, new Cell((long)y));
        }

        public override ScalarExpression VisitIfNullOp(PulseParser.IfNullOpContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpressionFunction.ExpressionIfNull f = new ScalarExpressionFunction.ExpressionIfNull();
            f.AddChildren(a, b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitIfOp(PulseParser.IfOpContext context)
        {

            ScalarExpression a = this.Visit(context.expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression c = (context.expression().Length == 3 ? this.Visit(context.expression()[2]) : new ScalarExpressionConstant(this._Master, new Cell(b.ExpressionReturnAffinity())));
            b.ParentNode = this._Master;

            ScalarExpressionFunction.ExpressionIf f = new ScalarExpressionFunction.ExpressionIf();
            f.AddChildren(a, b, c);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitBaseFunction(PulseParser.BaseFunctionContext context)
        {

            string Name = context.IDENTIFIER().GetText();
            if (!this._BaseFunctionLibrary.Exists(Name))
                throw new Exception(string.Format("Function does not exist '{0}' in the base library", Name));
            ScalarExpressionFunction f = this._BaseFunctionLibrary.Lookup(Name);
            foreach (PulseParser.ExpressionContext ctx in context.expression())
            {
                f.AddChildNode(this.Visit(ctx));
            }

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitLibraryFunction(PulseParser.LibraryFunctionContext context)
        {

            string LibName = context.IDENTIFIER()[0].GetText();
            string FuncName = context.IDENTIFIER()[1].GetText();

            if (!this._Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Libraries[LibName].FunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            ScalarExpressionFunction f = this._Libraries[LibName].LookupFunction(FuncName);
            foreach (PulseParser.ExpressionContext ctx in context.expression())
            {
                f.AddChildNode(this.Visit(ctx));
            }

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitCast(PulseParser.CastContext context)
        {

            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            ScalarExpression s = this.Visit(context.expression());
            ScalarExpression x = new ScalarExpressionFunction.ExpressionCast(t);
            x.AddChildNode(s);

            this._Master = x;

            return x;

        }

        public override ScalarExpression VisitMatrix2D(PulseParser.Matrix2DContext context)
        {

            // Get the matrix //
            string LibName = context.IDENTIFIER()[0].GetText();
            string MatrixName = context.IDENTIFIER()[1].GetText();

            ScalarExpression row = this.Visit(context.expression()[0]);
            ScalarExpression col = this.Visit(context.expression()[1]);

            if (this._Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            int HeapRef = this._Libraries.GetPointer(LibName);
            int MatrixRef = this._Libraries[HeapRef].Matrixes.GetPointer(MatrixName);

            return new ScalarExpressionMatrixRef(this._Master, HeapRef, MatrixRef, this._Libraries[HeapRef].Matrixes[MatrixRef].Affinity, this._Libraries[HeapRef].Matrixes[MatrixRef].Size);

        }

        public override ScalarExpression VisitMatrix2DNaked(PulseParser.Matrix2DNakedContext context)
        {

            // Get the matrix //
            string MatrixName = context.IDENTIFIER().GetText();

            ScalarExpression row = this.Visit(context.expression()[0]);
            ScalarExpression col = this.Visit(context.expression()[1]);

            // Loop through //
            for (int i = 0; i < this._Libraries.Count; i++)
            {

                if (this._Libraries[i].Matrixes.Exists(MatrixName))
                {
                    int MatrixRef = this._Libraries[i].Matrixes.GetPointer(MatrixName);
                    return new ScalarExpressionMatrixRef(this._Master, i, MatrixRef, this._Libraries[i].Matrixes[MatrixRef].Affinity, this._Libraries[i].Matrixes[MatrixRef].Size);
                }
                
            }
            
            throw new Exception(string.Format("Matrix does not exist '{0}'", MatrixName));

        }

        public override ScalarExpression VisitMatrix1D(PulseParser.Matrix1DContext context)
        {
            // Get the matrix //
            string LibName = context.IDENTIFIER()[0].GetText();
            string MatrixName = context.IDENTIFIER()[1].GetText();

            ScalarExpression row = this.Visit(context.expression());
            ScalarExpression col = ScalarExpression.OneINT;

            if (this._Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            int HeapRef = this._Libraries.GetPointer(LibName);
            int MatrixRef = this._Libraries[HeapRef].Matrixes.GetPointer(MatrixName);

            return new ScalarExpressionMatrixRef(this._Master, HeapRef, MatrixRef, this._Libraries[HeapRef].Matrixes[MatrixRef].Affinity, this._Libraries[HeapRef].Matrixes[MatrixRef].Size);

        }

        public override ScalarExpression VisitMatrix1DNaked(PulseParser.Matrix1DNakedContext context)
        {

            // Get the matrix //
            string MatrixName = context.IDENTIFIER().GetText();

            ScalarExpression row = this.Visit(context.expression());
            ScalarExpression col = ScalarExpression.OneINT;

            // Loop through //
            for (int i = 0; i < this._Libraries.Count; i++)
            {

                if (this._Libraries[i].Matrixes.Exists(MatrixName))
                {
                    int MatrixRef = this._Libraries[i].Matrixes.GetPointer(MatrixName);
                    return new ScalarExpressionMatrixRef(this._Master, i, MatrixRef, this._Libraries[i].Matrixes[MatrixRef].Affinity, this._Libraries[i].Matrixes[MatrixRef].Size);
                }

            }

            throw new Exception(string.Format("Matrix does not exist '{0}'", MatrixName));

        }

        public override ScalarExpression VisitParens(PulseParser.ParensContext context)
        {
            return this.Visit(context.expression());
        }

        // Visit Expression //
        public ScalarExpression Render(PulseParser.ExpressionContext context)
        {
            this._Master = null;
            return this.Visit(context);
        }

        // Visit Expression / Alias Collection //
        public ScalarExpressionCollection Render(PulseParser.Expression_or_wildcard_setContext context)
        {

            ScalarExpressionCollection fields = new ScalarExpressionCollection();

            foreach (PulseParser.Expression_or_wildcardContext ctx in context.expression_or_wildcard())
            {

                // Non-wild card clauses //
                if (ctx.expression_alias() != null)
                {

                    string alias = (ctx.expression_alias().IDENTIFIER() == null ? "F" + fields.Count.ToString() : ctx.expression_alias().IDENTIFIER().GetText());
                    ScalarExpression se = this.Render(ctx.expression_alias().expression());

                    // If 'se' is a scalar expression and we were not passed an alias, then assume the column name is the alias
                    if (se is ScalarExpressionFieldRef && ctx.expression_alias().IDENTIFIER() == null)
                    {
                        ScalarExpressionFieldRef x = (se as ScalarExpressionFieldRef);
                        int hidx = x.HeapRef;
                        int cidx = x.ColumnRef;
                        alias = this._Columns[hidx].ColumnName(cidx);
                    }

                    // If 'se' is a heap ref and we were not passed an alias, then assume the field ref is the alias
                    if (se is ScalarExpressionScalarRef && ctx.expression_alias().IDENTIFIER() == null)
                    {
                        ScalarExpressionScalarRef x = (se as ScalarExpressionScalarRef);
                        int hidx = x.HeapRef;
                        int cidx = x.ScalarRef;
                        alias = this._Libraries[hidx].Values.Name(cidx);
                    }

                    fields.Add(alias, se);

                }
                // Wild card 
                else
                {
                    
                    string alias = ctx.IDENTIFIER().GetText();
                    int idx = this._Columns.GetPointer(alias);
                    if (idx == -1) throw new Exception(string.Format("Table '{0}' does not exist", alias));
                    ScalarExpressionCollection sec = new ScalarExpressionCollection(this._Columns[idx], idx);
                    fields.Add(sec);

                }

            }

            return fields;

        }

        // Visit Where //
        public Filter Render(PulseParser.Where_clauseContext context)
        {
            if (context == null) return Filter.TrueForAll;
            ScalarExpression se = this.Render(context.expression());
            return new Filter(se);
        }

        // Visit Aggregate //
        public Aggregate Render(PulseParser.Beta_reductionContext context)
        {

            // Get the paramters //
            ScalarExpressionCollection sec = this.Render(context.expression_or_wildcard_set());

            // Get the filter //
            Filter f = this.Render(context.where_clause());

            // Get the aggregate //
            string name = context.SET_REDUCTIONS().GetText();
            AggregateLookup al = new AggregateLookup();

            // Check if it exists //
            if (!al.Exists(name))
                throw new Exception(string.Format("Aggregate '{0}' does not exist", name));

            return al.Lookup(name, sec, f);
            
        }

        // Visit Aggregate Collection //
        public AggregateCollection Render(PulseParser.Beta_reduction_listContext context)
        {

            AggregateCollection ac = new AggregateCollection();

            foreach (PulseParser.Beta_reductionContext ctx in context.beta_reduction())
            {

                string alias = (ctx.IDENTIFIER() == null ? "A" + ac.Count.ToString() : ctx.IDENTIFIER().GetText());
                Aggregate a = this.Render(ctx);

                ac.Add(alias, a);

            }

            return ac;

        }
        
        // Cloning //
        public ScalarExpressionVisitor CloneOfMe()
        {

            // Create the visitor //
            ScalarExpressionVisitor sev = new ScalarExpressionVisitor(this._Host);

            // Add all the columns //
            for (int i = 0; i < this._Columns.Count; i++)
            {
                sev.AddSchema(this._Columns.Name(i), this._Columns[i]);
            }

            // Add all the pointers //
            for (int i = 0; i < this._PointerAffinity.Count; i++)
            {
                this.AddPointer(this._PointerAffinity.Name(i), this._PointerAffinity[i], this._PointerSize[i]);
            }

            return sev;

        }

        // Statics //
        /// <summary>
        /// Cleans an incoming string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string CleanString(string Value)
        {

            // Check for empty text //
            if (Value == "" || Value == "''" || Value == "$$$$" || Value == "\"\"")
                return "";

            // Check for tab //
            if (Value.ToUpper() == "TAB")
                return "\t";

            // Check for newline //
            if (Value.ToUpper() == "CRLF")
                return "\n";

            // Check for "'", $$'$$, '"', $$"$$ //
            if (Value == "\"'\"" || Value == "$$'$$")
                return "'";
            if (Value == "'\"'" || Value == "$$\"$$")
                return "\"";

            // Check for lengths less than two //
            if (Value.Length < 2)
            {
                return Value.Replace("\\n", "\n").Replace("\\t", "\t");
            }

            // Handle 'ABC' to ABC //
            if (Value.First() == '\'' && Value.Last() == '\'' && Value.Length >= 2)
            {
                Value = Value.Substring(1, Value.Length - 2);
                while (Value.Contains("''"))
                {
                    Value = Value.Replace("''", "'");
                }
            }

            // Handle "ABC" to ABC //
            if (Value.First() == '"' && Value.Last() == '"' && Value.Length >= 2)
            {
                Value = Value.Substring(1, Value.Length - 2);
                while (Value.Contains("\"\""))
                {
                    Value = Value.Replace("\"\"", "\"");
                }
            }

            // Check for lengths less than four //
            if (Value.Length < 4)
            {
                return Value.Replace("\\n", "\n").Replace("\\t", "\t");
            }

            // Handle $$ABC$$ to ABC //
            int Len = Value.Length;
            if (Value[0] == '$' && Value[1] == '$' && Value[Len - 2] == '$' && Value[Len - 1] == '$')
            {
                Value = Value.Substring(2, Value.Length - 4);
                while (Value.Contains("$$$$"))
                {
                    Value = Value.Replace("$$$$", "$$");
                }
            }

            // Otherwise, return Value //
            return Value.Replace("\\n", "\n").Replace("\\t", "\t");

        }



    }

}
