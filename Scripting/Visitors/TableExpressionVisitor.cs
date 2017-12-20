using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Elements;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Scripting
{

    /// <summary>
    /// The base class that renders expressions
    /// </summary>
    public class TableExpressionVisitor : PulseParserBaseVisitor<TableExpression>
    {

        public const string ALIAS2_PREFIX = TableExpressionFold.SECOND_ALIAS_PREFIX;

        private Host _Host;
        private TableExpression _Master;

        public TableExpressionVisitor(Host Host, ScalarExpressionVisitor SFactory)
            : base()
        {
            this._Host = Host;
            this.SeedVisitor = SFactory;
        }

        public TableExpressionVisitor(Host Host)
            : this(Host, new ScalarExpressionVisitor(Host))
        {
        }

        // Properties //
        /// <summary>
        /// This is used as a seed visitor for each table render
        /// </summary>
        public ScalarExpressionVisitor SeedVisitor
        {
            get;
            set;
        }

        // Expressions //
        public override TableExpression VisitTableExpressionJoin(PulseParser.TableExpressionJoinContext context)
        {

            // Get the tables //
            TableExpression t1 = this.Visit(context.table_expression()[0]);
            TableExpression t2 = this.Visit(context.table_expression()[1]);

            // Get the aliases //
            string a1 = context.IDENTIFIER()[0].GetText();
            string a2 = context.IDENTIFIER()[1].GetText();

            // Build and load the keys //
            Tuple<Key, Key> x = this.RenderJoinPredicate(context.jframe(), t1.Columns, a1, t2.Columns, a2);
            RecordMatcher predicate = new RecordMatcher(x.Item1, x.Item2);

            // Get the affinity //
            TableExpressionJoin.JoinType t = TableExpressionJoin.JoinType.INNER;
            if (context.NOT() != null) t = TableExpressionJoin.JoinType.ANTI_LEFT;
            else if (context.PIPE() != null) t = TableExpressionJoin.JoinType.LEFT;

            // Get the join engine //
            TableExpressionJoin.JoinAlgorithm ja = TableExpressionJoin.Optimize(t1.EstimatedCount, t2.EstimatedCount, t1.IsIndexedBy(x.Item1), t2.IsIndexedBy(x.Item2));

            // Get the expressions //
            ScalarExpressionVisitor sev = this.SeedVisitor.CloneOfMe();
            this.SeedVisitor.Map.Local.DeclareRecord(a1, new AssociativeRecord(t1.Columns));
            this.SeedVisitor.Map.Local.DeclareRecord(a2, new AssociativeRecord(t2.Columns));
            ScalarExpressionSet sec = sev.Render(context.nframe());
            Filter f = sev.Render(context.where());

            // Create the expression //
            TableExpressionJoin tej = new TableExpressionJoin.TableExpressionJoinSortMerge(this._Host, this._Master, sec, predicate, f, t);
            tej.LeftAlias = a1;
            tej.RightAlias = a2;

            // Add the children //
            tej.AddChild(t1);
            tej.AddChild(t2);

            // Remove Map //
            this.SeedVisitor.Map.Local.RemoveRecord(a1);
            this.SeedVisitor.Map.Local.RemoveRecord(a2);

            // Point the master here //
            this._Master = tej;

            return tej;

        }

        public override TableExpression VisitTableExpressionFold1(PulseParser.TableExpressionFold1Context context)
        {

            TableExpression t = this.Visit(context);
            string alias = t.Name;
            string salias = ALIAS2_PREFIX + alias;

            this.SeedVisitor.Map.Local.DeclareRecord(alias, new AssociativeRecord(t.Columns));
            this.SeedVisitor.PrimaryContext = alias;
            ScalarExpressionSet grouper = this.SeedVisitor.Render(context.nframe());
            AggregateCollection aggs = this.SeedVisitor.Render(context.aframe());
            Filter f = this.SeedVisitor.Render(context.where());
            
            this.SeedVisitor.Map.Local.DeclareRecord(salias, new AssociativeRecord(Schema.Join(grouper.Columns, aggs.Columns)));
            ScalarExpressionSet select = new ScalarExpressionSet(Schema.Join(grouper.Columns, aggs.Columns), salias);

            TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, this._Master, grouper, aggs, f, select, alias);
            x.AddChild(t);

            this.SeedVisitor.Map.Local.RemoveRecord(alias);
            this.SeedVisitor.Map.Local.RemoveRecord(salias);
            this.SeedVisitor.PrimaryContext = null;
            
            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionFold2(PulseParser.TableExpressionFold2Context context)
        {

            TableExpression t = this.Visit(context);
            string alias = t.Name;
            string salias = ALIAS2_PREFIX + alias;

            this.SeedVisitor.Map.Local.DeclareRecord(alias, new AssociativeRecord(t.Columns));
            this.SeedVisitor.PrimaryContext = alias;
            ScalarExpressionSet grouper = this.SeedVisitor.Render(context.nframe());
            AggregateCollection aggs = new AggregateCollection();
            Filter f = this.SeedVisitor.Render(context.where());

            this.SeedVisitor.Map.Local.DeclareRecord(salias, new AssociativeRecord(Schema.Join(grouper.Columns, aggs.Columns)));
            ScalarExpressionSet select = new ScalarExpressionSet(Schema.Join(grouper.Columns, aggs.Columns), salias);

            TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, this._Master, grouper, aggs, f, select, alias);
            x.AddChild(t);

            this.SeedVisitor.Map.Local.RemoveRecord(alias);
            this.SeedVisitor.Map.Local.RemoveRecord(salias);
            this.SeedVisitor.PrimaryContext = null;

            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionFold3(PulseParser.TableExpressionFold3Context context)
        {

            TableExpression t = this.Visit(context);
            string alias = t.Name;
            string salias = ALIAS2_PREFIX + alias;

            this.SeedVisitor.Map.Local.DeclareRecord(alias, new AssociativeRecord(t.Columns));
            this.SeedVisitor.PrimaryContext = alias;
            ScalarExpressionSet grouper = new ScalarExpressionSet();
            AggregateCollection aggs = this.SeedVisitor.Render(context.aframe());
            Filter f = this.SeedVisitor.Render(context.where());

            this.SeedVisitor.Map.Local.DeclareRecord(salias, new AssociativeRecord(Schema.Join(grouper.Columns, aggs.Columns)));
            ScalarExpressionSet select = new ScalarExpressionSet(Schema.Join(grouper.Columns, aggs.Columns), salias);

            TableExpressionFold x = new TableExpressionFold.TableExpressionFoldDictionary(this._Host, this._Master, grouper, aggs, f, select, alias);
            x.AddChild(t);

            this.SeedVisitor.Map.Local.RemoveRecord(alias);
            this.SeedVisitor.Map.Local.RemoveRecord(salias);
            this.SeedVisitor.PrimaryContext = null;

            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionUnion(PulseParser.TableExpressionUnionContext context)
        {

            // Create a union expression //
            TableExpressionUnion teu = new TableExpressionUnion(this._Host, this._Master);

            // Load all the tables //
            foreach (PulseParser.Table_expressionContext ctx in context.table_expression())
            {
                TableExpression t_first = this.Visit(ctx);
                teu.AddChild(t_first);
            }

            this._Master = teu;

            return teu;

        }

        public override TableExpression VisitTableExpressionSelect(PulseParser.TableExpressionSelectContext context)
        {

            // Get the base expression //
            TableExpression t = this.Visit(context.table_expression());
            string alias = t.Name;

            // Get the where and the fields //
            ScalarExpressionVisitor vis = this.SeedVisitor.CloneOfMe();
            vis.AddSchema(alias, t.Columns);
            ScalarExpressionSet select = vis.Render(context.nframe());
            Filter where = vis.Render(context.where());

            // Create the expression //
            TableExpression x = new TableExpressionSelect(this._Host, this._Master, select, where, alias);
            x.AddChild(t);

            // Set the master //
            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionLookup(PulseParser.TableExpressionLookupContext context)
        {
            string lib = ScriptingHelper.GetLibName(context.table_name());
            string name = ScriptingHelper.GetVarName(context.table_name());
            Table t = this._Host.OpenTable(lib, name);
            TableExpression x = new TableExpressionValue(this._Host, this._Master, t);
            x.Alias = t.Name;
            return x;
        }

        public override TableExpression VisitTableExpressionCTOR(PulseParser.TableExpressionCTORContext context)
        {

            Schema cols = new Schema();
            for (int i = 0; i < context.IDENTIFIER().Length; i++)
            {
                cols.Add(context.IDENTIFIER()[i].GetText(), ScriptingHelper.GetTypeAffinity(context.type()[i]), ScriptingHelper.GetTypeSize(context.type()[i]));
            }
            string db = context.db_name().IDENTIFIER()[0].GetText();
            string name = context.db_name().IDENTIFIER()[1].GetText();

            return new TableExpressionCTOR(this._Host, null, cols, db, name, new Key());

        }

        public override TableExpression VisitTableExpressionParens(PulseParser.TableExpressionParensContext context)
        {
            return this.Visit(context.table_expression());
        }

        // Main //
        public TableExpression Render(PulseParser.Table_expressionContext context)
        {
            return this.Visit(context);
        }

       //  Support //
        private Key RenderKey(PulseParser.OframeContext context)
        {

            if (context == null)
                return null;

            Key k = new Key();

            foreach (PulseParser.OrderContext ctx in context.order())
            {

                int index = int.Parse(ctx.LITERAL_INT().GetText());
                KeyAffinity t = KeyAffinity.Ascending;
                if (ctx.K_DESC() != null) t = KeyAffinity.Descending;
                k.Add(index, t);

            }

            return k;

        }

        private Tuple<Key, Key> RenderJoinPredicate(PulseParser.JframeContext context, Schema LColumns, string LAlias, Schema RColumns, string RAlias)
        {

            Key left = new Key();
            Key right = new Key();

            foreach (PulseParser.JelementContext ctx in context.jelement())
            {

                string LeftAlias = ScriptingHelper.GetLibName(ctx.scalar_name()[0]);
                string RightAlias = ScriptingHelper.GetLibName(ctx.scalar_name()[1]);

                string LeftName = ScriptingHelper.GetVarName(ctx.scalar_name()[0]);
                string RightName = ScriptingHelper.GetVarName(ctx.scalar_name()[1]);

                if (LeftAlias == LAlias && RightAlias == RAlias)
                {
                    // Do nothing
                }
                else if (RightAlias == LAlias && LeftAlias == RAlias)
                {
                    string t = LeftAlias;
                    LeftAlias = RightAlias;
                    RightAlias = t;
                }
                else
                {
                    throw new Exception(string.Format("One of '{0}' or '{1}' is invalid", LeftAlias, RightAlias));
                }

                int LeftIndex = LColumns.ColumnIndex(LeftName);
                if (LeftIndex == -1)
                    throw new Exception(string.Format("Field '{0}' does not exist in '{1}'", LeftName, LeftAlias));
                int RightIndex = RColumns.ColumnIndex(RightName);
                if (RightIndex == -1)
                    throw new Exception(string.Format("Field '{0}' does not exist in '{1}'", RightName, RightAlias));

                left.Add(LeftIndex);
                right.Add(RightIndex);

            }

            return new Tuple<Key, Key>(left, right);

        }
    
    
    
    }

}
