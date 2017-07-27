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


    public sealed class TableExpressionSelect : TableExpression
    {

        private ScalarExpressionCollection _Fields;
        private Filter _Where;
        private FieldResolver _Resolver;
        private int _RecordRef;

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, Filter Where, FieldResolver Variants, int RecordRef)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this._Resolver = Variants;
            this._RecordRef = RecordRef;
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
        /// Gets the base resolver
        /// </summary>
        public FieldResolver BaseResolver
        {
            get { return this._Resolver; }
        }

        /// <summary>
        /// Gets the pointer to the position in the field resolver to pull from
        /// </summary>
        public int RecordRef
        {
            get { return this._RecordRef; }
            set { this._RecordRef = value; }
        }

        /// <summary>
        /// Renders the expression
        /// </summary>
        /// <param name="Writer"></param>
        public override void Evaluate(RecordWriter Writer)
        {
            
            Table t = this.Children[0].Evaluate();
            RecordReader rs = t.OpenReader();

            while (rs.CanAdvance)
            {
                this._Resolver.SetValue(this._RecordRef, rs.ReadNext());
                if (Where.Evaluate(this._Resolver))
                    Writer.Insert(Fields.Evaluate(this._Resolver));
            }

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
