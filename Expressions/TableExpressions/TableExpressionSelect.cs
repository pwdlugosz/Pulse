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

        private RecordExpression _Fields;
        private Filter _Where;
        private int _RecordRef = -1;
        private long _Limit;

        public TableExpressionSelect(Host Host, TableExpression Parent, RecordExpression Fields, Filter Where, long Limit)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this.Alias = "SELECT";
            this._Limit = Limit;
        }

        public TableExpressionSelect(Host Host, TableExpression Parent, RecordExpression Fields, Filter Where)
            : this(Host, Parent, Fields, Where, -1)
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

            FieldResolver x = Variants.CloneOfMeFull();
            x.AddSchema(this._Children[0].Alias, this._Children[0].Columns, out this._RecordRef);
            return x;

        }

        /// <summary>
        /// Gets the seleced fields
        /// </summary>
        public RecordExpression Fields
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

            // Create the resolver //
            FieldResolver pointer = this.CreateResolver(Variants);

            // Open the reader //
            RecordReader rs = t.OpenReader();

            // Ticks //
            long ticks = 0;
            
            // Main read loop //
            while (rs.CanAdvance)
            {

                if (this._Limit >= ticks)
                    break;

                pointer.SetValue(this._RecordRef, rs.ReadNext());

                if (Where.Evaluate(pointer))
                {
                    Writer.Insert(Fields.Evaluate(pointer));
                }

                ticks++;

            }

            // Clean up //
            if (this._Host.IsSystemTemp(t))
                this._Host.Store.DropTable(t.Key);

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
