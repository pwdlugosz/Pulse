using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.Aggregates;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.TableExpressions
{

    /// <summary>
    /// Represents a table  expression for a basic select statement
    /// </summary>
    public sealed class TableExpressionSelect : TableExpression
    {

        private ScalarExpressionSet _Fields;
        private Filter _Where;
        private string _Alias;
        private long _Limit;

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionSet Fields, Filter Where, string Alias, long Limit)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this.Alias = "SELECT";
            this._Limit = Limit;
            this._Alias = Alias;
        }

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionSet Fields, Filter Where, string Alias)
            : this(Host, Parent, Fields, Where, Alias, -1)
        {
        }

        /// <summary>
        /// The limit of records to return
        /// </summary>
        public long Limit
        {
            get { return this._Limit; }
        }

        /// <summary>
        /// Gets the underlying columns
        /// </summary>
        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public override FieldResolver CreateResolver(FieldResolver Variants)
        {
            FieldResolver x = Variants;
            return x;
        }

        /// <summary>
        /// Gets the seleced fields
        /// </summary>
        public ScalarExpressionSet Fields
        {
            get { return this._Fields; }
        }

        /// <summary>
        /// Gets the filter
        /// </summary>
        public Filter Where
        {
            get { return this._Where; }
        }

        /// <summary>
        /// Renders the expression
        /// </summary>
        /// <param name="Writer"></param>
        public override void Evaluate(FieldResolver Variants, RecordWriter Writer)
        {

            // Render the base table //
            Table t = this.Children[0].Select(Variants);

            // Initialize //
            this.InitializeResolver(Variants);

            // Open the reader //
            RecordReader rs = t.OpenReader();

            // Ticks //
            long ticks = 0;
            
            // Main read loop //
            while (rs.CanAdvance)
            {

                if (this._Limit >= ticks)
                    break;

                Variants.Local.SetRecord(this._Alias, new AssociativeRecord(t.Columns, rs.ReadNext()));

                if (Where.Evaluate(Variants))
                {
                    Writer.Insert(Fields.Evaluate(Variants));
                }

                ticks++;

            }

            // Clean up //
            if (this._Host.IsSystemTemp(t))
                this._Host.TableStore.DropTable(t.Key);

            // Fix Resolver //
            this.CleanUpResolver(Variants);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void InitializeResolver(FieldResolver Variants)
        {
            // Fix the resolver //
            if (!Variants.Local.ExistsRecord(this._Alias))
                Variants.Local.DeclareRecord(this._Alias, new AssociativeRecord(this._Children[0].Columns));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Variants"></param>
        public override void CleanUpResolver(FieldResolver Variants)
        {
            // Fix the resolver //
            if (Variants.Local.ExistsRecord(this._Alias))
                Variants.Local.RemoveRecord(this._Alias);
        }

        /// <summary>
        /// Gets the estimated record count
        /// </summary>
        public override long EstimatedCount
        {
            get
            {
                return this._Children.Max((x) => { return x.EstimatedCount; });
            }
        }

    }



}
