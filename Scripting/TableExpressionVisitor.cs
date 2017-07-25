﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.TableExpressions;
using Pulse.ScalarExpressions;
using Pulse.Data;
using Pulse.Aggregates;
using Pulse.Query.Aggregate;
using Pulse.Query.Join;
using Pulse.Query.Select;
using Pulse.Query.Union;

namespace Pulse.Scripting
{

    /// <summary>
    /// The base class that renders expressions
    /// </summary>
    public class TableExpressionVisitor : PulseParserBaseVisitor<TableExpression>
    {

        private Host _Host;
        private TableExpression _Master;

        public TableExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this.SeedVisitor = new ScalarExpressionVisitor(Host);
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
            string a1 = context.t_alias()[0].IDENTIFIER().GetText();
            string a2 = context.t_alias()[1].IDENTIFIER().GetText();

            // Build and load the keys //
            Key k1 = new Key(), k2 = new Key();
            foreach (PulseParser.T_join_onContext ctx in context.t_join_on())
            {
                this.AppendKey(ctx, k1, a1, t1.Columns, k2, a2, t2.Columns);
            }
            RecordMatcher predicate = new RecordMatcher(k1, k2);

            // Get the affinity //
            Query.Join.JoinType t = Query.Join.JoinType.INNER;
            if (context.K_ALEFT_JOIN() != null)
                t = Query.Join.JoinType.ANTI_LEFT;
            else if (context.K_LEFT_JOIN() == null)
                t = Query.Join.JoinType.LEFT;

            // Get the join engine //
            Query.Join.JoinAlgorithm ja = Query.Join.JoinOptimizer.LowestCost(t1.EstimatedCount, t2.EstimatedCount);

            // Create the join engine //
            JoinEngine je;
            if (ja == JoinAlgorithm.SortMerge)
                je = new SortMergeJoinEngine();
            else if (ja == JoinAlgorithm.QuasiNestedLoop)
                je = new QuasiNestedLoopJoinEngine();
            else
                je = new QuasiNestedLoopJoinEngine();

            // Get the expressions //
            ScalarExpressionVisitor sev = this.SeedVisitor.CloneOfMe();
            sev.AddSchema(a1, t1.Columns);
            sev.AddSchema(a2, t2.Columns);
            ScalarExpressionCollection sec = sev.Render(context.t_select().expression_or_wildcard_set());
            Filter f = sev.Render(context.where_clause());

            // Create the expression //
            TableExpressionJoin tej = new TableExpressionJoin(this._Host, this._Master, sec, predicate, f, je, t);

            // Handle the clustering, the distinct, and the ordering //
            this.AddModifications(tej, context.tmod_distinct(), context.tmod_order());

            // Add the children //
            tej.AddChild(t1);
            tej.AddChild(t2);

            // Point the master here //
            this._Master = tej;

            return tej;

        }

        public override TableExpression VisitTableExpressionFold(PulseParser.TableExpressionFoldContext context)
        {

            // Get the source expression //
            TableExpression te = this.Visit(context.table_expression());

            // Get the alias //
            string a_prime = context.t_alias() == null ? "T" : context.t_alias().IDENTIFIER().GetText();
            string a_val = context.t_fold().t_alias() == null ? "T" : context.t_fold().t_alias().IDENTIFIER().GetText();
            
            // Visitor used by the grouper //
            ScalarExpressionVisitor G_Visitor = this.SeedVisitor.CloneOfMe();
            G_Visitor.AddSchema(a_prime, te.Columns);
            
            // Get the grouping and the aggregates //
            ScalarExpressionCollection grouper = G_Visitor.Render(context.t_fold().expression_or_wildcard_set());
            AggregateCollection folds = G_Visitor.Render(context.t_fold().beta_reduction_list());
            Filter where = G_Visitor.Render(context.where_clause());

            // Create the visitor used by the selector //
            ScalarExpressionVisitor S_Visitor = this.SeedVisitor.CloneOfMe();
            S_Visitor.AddSchema(a_val, Schema.Join(grouper.Columns, folds.Columns));

            // Get the selector //
            ScalarExpressionCollection selector = new ScalarExpressionCollection(Schema.Join(grouper.Columns, folds.Columns), 0);
            if (context.t_select() != null)
                selector = S_Visitor.Render(context.t_select().expression_or_wildcard_set());

            // Build the engine //
            AggregateEngine ae = new DictionaryAggregateEngine();

            // Create the expression //
            TableExpressionFold tef = new TableExpressionFold(this._Host, this._Master, grouper, folds, where, selector, ae);

            // Add the mods //
            this.AddModifications(tef, context.tmod_distinct(), context.tmod_order());

            // Add childrend //
            tef.AddChild(te);

            this._Master = tef;

            return tef;

        }

        public override TableExpression VisitTableExpressionUnion(PulseParser.TableExpressionUnionContext context)
        {
            
            // Create a union engine //
            UnionEngine ue = new BasicUnionEngine();

            // Create a union expression //
            TableExpressionUnion teu = new TableExpressionUnion(this._Host, this._Master, ue);

            // Load all the tables //
            foreach (PulseParser.Table_expressionContext ctx in context.table_expression())
            {
                TableExpression t_first = this.Visit(ctx);
                teu.AddChild(t_first);
            }
            
            // Handles the mods //
            this.AddModifications(teu, context.tmod_distinct(), context.tmod_order());

            this._Master = teu;

            return teu;

        }

        public override TableExpression VisitTableExpressionSelect(PulseParser.TableExpressionSelectContext context)
        {
            
            // Get the base expression //
            TableExpression t = this.Visit(context.table_expression());
            string a_prime = context.t_alias() == null ? "T" : context.t_alias().IDENTIFIER().GetText();
            
            // Get the where and the fields //
            ScalarExpressionVisitor vis = this.SeedVisitor.CloneOfMe();
            vis.AddSchema(a_prime, t.Columns);
            ScalarExpressionCollection select = vis.Render(context.t_select().expression_or_wildcard_set());
            Filter where = vis.Render(context.where_clause());

            // Create the expression //
            TableExpression x = new TableExpressionSelect(this._Host, this._Master, select, where, new BasicSelectEngine());
            x.AddChild(t);

            // Modifiers //
            this.AddModifications(x, context.tmod_distinct(), context.tmod_order());
            
            // Set the master //
            this._Master = x;

            return x;

        }

        public override TableExpression VisitTableExpressionLookup(PulseParser.TableExpressionLookupContext context)
        {

            Table t = this._Host.OpenTable(context.IDENTIFIER()[0].GetText(), context.IDENTIFIER()[1].GetText());

            return new TableExpressionValue(this._Host, this._Master, t);

        }

        public override TableExpression VisitTableExpressionLiteral(PulseParser.TableExpressionLiteralContext context)
        {
            
            // Build a visitor //
            ScalarExpressionVisitor vis = this.SeedVisitor.CloneOfMe();

            // Get the first item //
            ScalarExpressionCollection fields = vis.Render(context.expression_or_wildcard_set()[0]);
            FieldResolver r = vis.ImpliedResolver;

            // Create the table //
            Table t = this._Host.CreateTempTable(fields.Columns);

            // Load the table //
            using (RecordWriter ws = t.OpenWriter())
            {

                // Load the first record //
                ws.Insert(fields.Evaluate(r));

                // Load every other record //
                for (int i = 1; i < context.expression_or_wildcard_set().Length; i++)
                {
                    int ExpectedLen = fields.Count;
                    fields = vis.Render(context.expression_or_wildcard_set()[i]);
                    if (fields.Count != ExpectedLen)
                        throw new Exception(string.Format("Invalid record lenght of {0}; expecting {1}", fields.Count, ExpectedLen));
                    ws.Insert(fields.Evaluate(r));
                }

            }

            // Return the table //
            return new TableExpressionValue(this._Host, this._Master, t);

        }

        public override TableExpression VisitTableExpressionShell(PulseParser.TableExpressionShellContext context)
        {

            Schema s = new Schema();

            for (int i = 0; i < context.IDENTIFIER().Length; i++)
            {
                CellAffinity t = ScriptingHelper.GetTypeAffinity(context.type()[i]);
                int z = ScriptingHelper.GetTypeSize(context.type()[i]);
                string n = context.IDENTIFIER()[i].GetText();
                s.Add(n, t, z);
            }

            return new TableExpressionShell(this._Host, this._Master, s);

        }

        public override TableExpression VisitTableExpressionParens(PulseParser.TableExpressionParensContext context)
        {
            return this.Visit(context);
        }

        // Main //
        public TableExpression Render(PulseParser.Table_expressionContext context)
        {
            return this.Visit(context);
        }

        // Support //
        private void AppendKey(PulseParser.T_join_onContext context, Key LeftKey, string LeftAlias, Schema LeftSchema, Key RightKey, string RightAlias, Schema RightSchema)
        {

            string Alias1 = context.IDENTIFIER()[0].GetText();
            string Alias2 = context.IDENTIFIER()[2].GetText();
            string Name1 = context.IDENTIFIER()[1].GetText();
            string Name2 = context.IDENTIFIER()[3].GetText();

            if (Alias1 == LeftAlias && Alias2 == RightAlias)
            {

                // Find the indexes //
                int idx1 = LeftSchema.ColumnIndex(Name1);
                int idx2 = RightSchema.ColumnIndex(Name2);

                // Check the indexes //
                if (idx1 == -1) throw new Exception(string.Format("Field '{0}' not found in '{1}'", Name1, Alias1));
                if (idx2 == -1) throw new Exception(string.Format("Field '{0}' not found in '{1}'", Name2, Alias2));

                // Append the key //
                LeftKey.Add(idx1);
                RightKey.Add(idx2);

            }
            else if (Alias1 == RightAlias && Alias2 == LeftAlias)
            {

                // Find the indexes //
                int idx1 = LeftSchema.ColumnIndex(Name2);
                int idx2 = RightSchema.ColumnIndex(Name1);

                // Check the indexes //
                if (idx1 == -1) throw new Exception(string.Format("Field '{0}' not found in '{1}'", Name2, Alias2));
                if (idx2 == -1) throw new Exception(string.Format("Field '{0}' not found in '{1}'", Name1, Alias1));

                // Append the key //
                LeftKey.Add(idx1);
                RightKey.Add(idx2);

            }
            else
            {

                if (Alias1 != LeftAlias && Alias1 != RightAlias) 
                    throw new Exception(string.Format("Alias '{0}' not found", Alias1));
                if (Alias2 != LeftAlias && Alias2 != RightAlias)
                    throw new Exception(string.Format("Alias '{0}' not found", Alias2));
                throw new ArgumentException("Unknown error");

            }

        }

        private Key GetOrderBy(PulseParser.Tmod_orderContext context)
        {

            if (context.K_ORDER() == null)
                return null;

            Key k = new Key();
            for (int i = 0; i < context.LITERAL_INT().Length; i++)
            {
                int v = int.Parse(context.LITERAL_INT()[i].GetText());
                k.Add(v);
            }

            return k;

        }

        private void AddModifications(TableExpression Expression, PulseParser.Tmod_distinctContext DistinctContext, PulseParser.Tmod_orderContext OrderContext)
        {

            if (OrderContext == null)
                return;

            Key k = this.GetOrderBy(OrderContext);
            if (k != null)
                Expression.OrderBy = k;

            if (DistinctContext != null)
                Expression.IsDistinct = true;

        }

    }

}