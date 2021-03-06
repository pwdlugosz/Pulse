﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Libraries;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.RecordExpressions;


namespace Pulse.Scripting
{

    /// <summary>
    /// Represents a visitor for expressions
    /// </summary>
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
        private ScalarExpression _Master;
        private FieldResolver _Map;
        private Heap<int> _PointerSize;
        private Heap<CellAffinity> _PointerAffinity;
        private string _PrimaryContext;

        public ScalarExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._PointerSize = new Elements.Heap<int>();
            this._PointerAffinity = new Elements.Heap<Elements.CellAffinity>();
            this._Map = new FieldResolver(this._Host);
        }

        // Properties //
        public FieldResolver Map
        {
            get { return this._Map; }
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public string PrimaryContext
        {
            get { return this._PrimaryContext; }
            set { this._PrimaryContext = value; }
        }

        // Support Filed Methods //
        public void AddSchema(string Alias, Schema Columns, out int Pointer) 
        {
            Pointer = this._Map.Local.Records.Count;
            this._Map.Local.DeclareRecord(Alias, new AssociativeRecord(Columns));
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

            return new ScalarExpressionPointer(this._Host, this._Master, Name, Type, Size);

        }

        public override ScalarExpression VisitUniary(PulseParser.UniaryContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression());
            a.ParentNode = this._Master;

            ScalarExpression f = null;
            if (context.PLUS() != null)
                f = +a;
            else if (context.MINUS() != null)
                f = -a;
            else if (context.NOT() != null)
                f = !a;
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitPower(PulseParser.PowerContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = new ScalarExpressionBinary.ScalarExpressionPower(a,b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitMultDivMod(PulseParser.MultDivModContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.MUL() != null)
                f = a * b;
            else if (context.DIV() != null)
                f = a / b;
            else if (context.MOD() != null)
                f = a % b;
            else if (context.DIV2() != null)
                f = ScalarExpression.CDIV(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitAddSub(PulseParser.AddSubContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.PLUS() != null)
                f = a + b;
            else if (context.MINUS() != null)
                f = a - b;
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitGreaterLesser(PulseParser.GreaterLesserContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.GT() != null)
                f = ScalarExpression.GT(a, b);
            else if (context.LT() != null)
                f = ScalarExpression.LT(a, b);
            else if (context.GTE() != null)
                f = ScalarExpression.GTE(a, b);
            else if (context.LTE() != null)
                f = ScalarExpression.LTE(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitLogicalAnd(PulseParser.LogicalAndContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = ScalarExpression.AND(a, b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitEquality(PulseParser.EqualityContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.EQ() != null)
                f = ScalarExpression.EQ(a, b);
            else if (context.NEQ() != null)
                f = ScalarExpression.NEQ(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitLogicalOr(PulseParser.LogicalOrContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = null;

            if (context.OR() != null)
                f = ScalarExpression.OR(a, b);
            else if (context.XOR() != null)
                f = ScalarExpression.XOR(a, b);
            else
                throw new Exception("Unknow opperation");

            this._Master = f;

            return f;

        }

        public override ScalarExpression VisitBitShiftRotate(PulseParser.BitShiftRotateContext context)
        {

            ScalarExpression l = this.Visit(context.scalar_expression()[0]);
            ScalarExpression r = this.Visit(context.scalar_expression()[1]);

            ScalarExpression e = null;
            if (context.L_SHIFT() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionLeftShift(l, r);
            }
            else if (context.L_ROTATE() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionLeftRotate(l, r);
            }
            else if (context.R_SHIFT() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionRightShift(l, r);
            }
            else if (context.R_ROTATE() != null)
            {
                e = new ScalarExpressionBinary.ScalarExpressionRightRotate(l, r);
            }
            else
            {
                throw new Exception();
            }

            this._Master = e;

            return e;


        }

        public override ScalarExpression VisitTableOrScalarMember(PulseParser.TableOrScalarMemberContext context)
        {

            // Specific variable: Name.Name //
            if (context.scalar_name().lib_name() != null)
            {

                string Major = context.scalar_name().lib_name().GetText();
                string Minor = context.scalar_name().IDENTIFIER().GetText();

                // Check for other store variable, such as GLOBAL.X //
                if (this._Map.StoreExists(Major))
                {
                    if (this._Map[Major].ExistsScalar(Minor))
                    {
                        return new ScalarExpressionStoreRef(this._Host, this._Master, Major, Minor, this._Map[Major].Scalars[Minor].Affinity, this._Map[Major].Scalars[Minor].Length);
                    }
                }

                // Must be a record ref //
                if (this._Map.Global.ExistsRecord(Major) && this._Map.Global.GetRecord(Major).Columns.ColumnIndex(Minor) != -1)
                {
                    Schema v = this._Map.Global.GetColumns(Major);
                    return new ScalarExpressionRecordRef(this._Host, this._Master, FieldResolver.GLOBAL, Major, Minor, v.ColumnAffinity(Minor), v.ColumnSize(Minor));
                }
                else if (this._Map.Local.ExistsRecord(Major) && this._Map.Local.GetRecord(Major).Columns.ColumnIndex(Minor) != -1)
                {
                    Schema v = this._Map.Local.GetColumns(Major);
                    return new ScalarExpressionRecordRef(this._Host, this._Master, FieldResolver.LOCAL, Major, Minor, v.ColumnAffinity(Minor), v.ColumnSize(Minor));
                }

                throw new Exception(string.Format("Field '{0}.{1}' is invalid", Major, Minor));

            }

            // Ambiguios variable name //
            else
            {

                string Minor = context.scalar_name().IDENTIFIER().GetText();

                // Check if it's a global variable //
                if (this._Map.Global.ExistsScalar(Minor))
                {
                    return new ScalarExpressionStoreRef(this._Host, this._Master, FieldResolver.GLOBAL, Minor, this._Map.Global.Scalars[Minor].Affinity, this._Map.Global.Scalars[Minor].Length);
                }

                // Check local //
                if (this._Map.Local.ExistsScalar(Minor))
                {
                    return new ScalarExpressionStoreRef(this._Host, this._Master, FieldResolver.LOCAL, Minor, this._Map.Local.Scalars[Minor].Affinity, this._Map.Local.Scalars[Minor].Length);
                }

                // Otherwise, check if the primary context exists in local and the value is in the primary context
                if (this._PrimaryContext != null && this._Map.Local.ExistsRecord(this._PrimaryContext) && this._Map.Local.Records[this._PrimaryContext].Columns.ColumnIndex(Minor) != -1)
                {
                    Schema v = this._Map.Local.GetColumns(this._PrimaryContext);
                    return new ScalarExpressionRecordRef(this._Host, this._Master, FieldResolver.LOCAL, this._PrimaryContext, Minor, v.ColumnAffinity(Minor), v.ColumnSize(Minor));
                }
                throw new Exception(string.Format("Field '{0}' is invalid", Minor));

            }

        }

        public override ScalarExpression VisitRecordMember(PulseParser.RecordMemberContext context)
        {
            string Major = ScriptingHelper.GetLibName(context.record_name());
            string Medium = ScriptingHelper.GetVarName(context.record_name());
            string Minor = context.IDENTIFIER().GetText();
            return new ScalarExpressionRecordRef(this._Host, this._Master, Major, Medium, Minor, this._Map[Major].GetRecord(Medium)[Minor].Affinity, this._Map[Major].GetColumns(Medium).ColumnSize(Minor));
        }

        public override ScalarExpression VisitMatrixMember(PulseParser.MatrixMemberContext context)
        {

            ScalarExpression row = this.Visit(context.scalar_expression()[0]);
            ScalarExpression col = (context.scalar_expression().Length >= 2 ? this.Visit(context.scalar_expression()[1]) : new ScalarExpressionConstant(null, CellValues.ZeroINT));
            MatrixExpression m = (new MatrixExpressionVisitor(this._Host, this)).Render(context.matrix_expression());
            ScalarExpressionMatrixRef s = new ScalarExpressionMatrixRef(this._Host, this._Master, m, row, col);
            return s;

        }

        public override ScalarExpression VisitLiteralBool(PulseParser.LiteralBoolContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BOOL));
        }

        public override ScalarExpression VisitLiteralDateTime(PulseParser.LiteralDateTimeContext context)
        {
            string s = context.GetText();
            s = s.Replace("T", "").Replace("t", "");
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.DATE_TIME));
        }

        public override ScalarExpression VisitLiteralByte(PulseParser.LiteralByteContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BYTE));
        }

        public override ScalarExpression VisitLiteralShort(PulseParser.LiteralShortContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.SHORT));
        }

        public override ScalarExpression VisitLiteralInt(PulseParser.LiteralIntContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.INT));
        }

        public override ScalarExpression VisitLiteralLong(PulseParser.LiteralLongContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.LONG));
        }

        public override ScalarExpression VisitLiteralSingle(PulseParser.LiteralSingleContext context)
        {
            string s = context.GetText();
            s = s.Replace("F", "").Replace("f", "");
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.SINGLE));
        }

        public override ScalarExpression VisitLiteralDouble(PulseParser.LiteralDoubleContext context)
        {
            string s = context.GetText();
            s = s.Replace("D", "").Replace("d", "");
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.DOUBLE));
        }

        public override ScalarExpression VisitLiteralBinary(PulseParser.LiteralBinaryContext context)
        {
            string s = context.GetText();
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BINARY));
        }

        public override ScalarExpression VisitLiteralBString(PulseParser.LiteralBStringContext context)
        {
            string s = context.GetText();
            if (s.Last() == 'b' || s.Last() == 'B')
                s = s.Substring(0, s.Length - 1);
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.BSTRING));
        }

        public override ScalarExpression VisitLiteralCString(PulseParser.LiteralCStringContext context)
        {
            string s = context.GetText();
            if (s.Last() == 'c' || s.Last() == 'C')
                s = s.Substring(0, s.Length - 1);
            s = CleanString(s);
            return new ScalarExpressionConstant(this._Master, CellParser.Parse(s, CellAffinity.CSTRING));
        }

        public override ScalarExpression VisitLiteralNull(PulseParser.LiteralNullContext context)
        {
            return new ScalarExpressionConstant(this._Master, CellValues.NullLONG);
        }

        public override ScalarExpression VisitExpressionType(PulseParser.ExpressionTypeContext context)
        {
            CellAffinity x = ScriptingHelper.GetTypeAffinity(context.type());
            byte y = (byte)x;
            return new ScalarExpressionConstant(this._Master, new Cell(y));
        }

        public override ScalarExpression VisitIfNullOp(PulseParser.IfNullOpContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression f = ScalarExpression.IFNULL(a, b);
            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitIfOp(PulseParser.IfOpContext context)
        {

            ScalarExpression a = this.Visit(context.scalar_expression()[0]);
            a.ParentNode = this._Master;
            ScalarExpression b = this.Visit(context.scalar_expression()[1]);
            b.ParentNode = this._Master;
            ScalarExpression c = (context.scalar_expression().Length == 3 ? this.Visit(context.scalar_expression()[2]) : new ScalarExpressionConstant(this._Master, new Cell(b.ReturnAffinity())));
            c.ParentNode = this._Master;

            ScalarExpression f = ScalarExpression.IF(a, b, c);

            this._Master = f;
            return f;

        }

        public override ScalarExpression VisitScalarExpressionFunction(PulseParser.ScalarExpressionFunctionContext context)
        {

            string LibName = ScriptingHelper.GetLibName(context.scalar_name());
            string FuncName = ScriptingHelper.GetVarName(context.scalar_name());

            if (!this._Host.Libraries.Exists(LibName))
                throw new Exception(string.Format("Library does not exist '{0}'", LibName));

            if (!this._Host.Libraries[LibName].ScalarFunctionExists(FuncName))
                throw new Exception(string.Format("Function '{0}' does not exist in '{1}'", FuncName, LibName));

            ObjectFactory of = new ObjectFactory(this._Host, this);

            ScalarExpressionFunction f = this._Host.Libraries[LibName].ScalarFunctionLookup(FuncName);
            foreach (PulseParser.ParamContext ctx in context.param())
            {
                Parameter p = of.Render(ctx);
                f.AddParameter(p);
            }

            this._Master = f;

            return f;
        }

        public override ScalarExpression VisitCast(PulseParser.CastContext context)
        {

            CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type());
            ScalarExpression s = this.Visit(context.scalar_expression());
            ScalarExpression x = ScalarExpression.CAST(s, t);

            this._Master = x;

            return x;

        }

        public override ScalarExpression VisitParens(PulseParser.ParensContext context)
        {
            return this.Visit(context.scalar_expression());
        }

        // Visit Expressions //
        public ScalarExpression Render(PulseParser.Scalar_expressionContext context)
        {
            this._Master = null;
            return this.Visit(context);
        }

        public ScalarExpressionSet Render(PulseParser.NframeContext context)
        {
            ScalarExpressionSet rex = new ScalarExpressionSet(this._Host);
            foreach (PulseParser.NelementContext s in context.nelement())
            {
                ScalarExpression se = this.Render(s.scalar_expression());
                string alias = (s.IDENTIFIER() == null ? se.BuildAlias() : s.IDENTIFIER().GetText());
                
                rex.Add(alias, se);
            }
            return rex;
        }

        public Filter Render(PulseParser.WhereContext context)
        {
            if (context == null) return Filter.TrueForAll;
            ScalarExpression se = this.Render(context.scalar_expression());
            return new Filter(se);
        }

        // Visit Aggregate //
        public Aggregate Render(PulseParser.AggContext context)
        {

            // Get the paramters //
            ScalarExpressionSet rex = this.Render(context.nframe());

            // Get the filter //
            Filter f = this.Render(context.where());

            // Get the aggregate //
            string name = context.SET_REDUCTIONS().GetText();
            AggregateLookup al = new AggregateLookup();

            // Check if it exists //
            if (!al.Exists(name))
                throw new Exception(string.Format("Aggregate '{0}' does not exist", name));

            return al.Lookup(name, rex, f);

        }

        // Visit Aggregate Collection //
        public AggregateCollection Render(PulseParser.AframeContext context)
        {

            AggregateCollection ac = new AggregateCollection();

            foreach (PulseParser.AggContext ctx in context.agg())
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
            sev._Map = this._Map;

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
