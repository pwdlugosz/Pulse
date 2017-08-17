using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Query;
using Pulse.ScalarExpressions;
using Pulse.Aggregates;
using Pulse.Query.Select;
using Pulse.Query.Aggregate;
using Pulse.Query.Join;
using Pulse.Query.Union;

namespace Pulse.TableExpressions
{

    /// <summary>
    /// Represents a table  expression for a basic select statement
    /// </summary>
    public sealed class TableExpressionSelect : TableExpression
    {

        private ScalarExpressionCollection _Fields;
        private Filter _Where;
        private int _RecordRef = -1;

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, Filter Where)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this.Alias = "SELECT";
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
        public ScalarExpressionCollection Fields
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
            
            // Main read loop //
            while (rs.CanAdvance)
            {

                pointer.SetValue(this._RecordRef, rs.ReadNext());

                if (Where.Evaluate(pointer))
                {
                    Writer.Insert(Fields.Evaluate(pointer));
                }

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
