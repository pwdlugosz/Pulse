using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Tables;

namespace Pulse.Scripting
{

    public sealed class ObjectFactory
    {

        private ScalarExpressionVisitor _sFactory;
        private MatrixExpressionVisitor _mFactory;
        private RecordExpressionVisitor _rFactory;
        private TableExpressionVisitor _tFactory;
        private Host _Host;

        public ObjectFactory(Host Host, ScalarExpressionVisitor SFactory, MatrixExpressionVisitor MFactory, RecordExpressionVisitor RFactory, TableExpressionVisitor TFactory)
        {
            this._Host = Host;
            this._sFactory = SFactory;
            this._mFactory = MFactory;
            this._rFactory = RFactory;
            this._tFactory = TFactory;
        }

        public ObjectFactory(Host Host, ScalarExpressionVisitor SFactory)
        {
            this._Host = Host;
            this._sFactory = SFactory;
            this._mFactory = new MatrixExpressionVisitor(this._Host, this._sFactory);
            this._rFactory = new RecordExpressionVisitor(this._Host, this._sFactory);
            this._tFactory = new TableExpressionVisitor(this._Host, this._sFactory);
        }

        public ObjectFactory(Host Host)
            : this(Host, new ScalarExpressionVisitor(Host))
        {
        }

        public ScalarExpressionVisitor BaseScalarFactory
        {
            get { return this._sFactory; }
        }

        public MatrixExpressionVisitor BaseMatrixFactory
        {
            get { return this._mFactory; }
        }

        public RecordExpressionVisitor BaseRecordVisitor
        {
            get { return this._rFactory; }
        }

        public TableExpressionVisitor BaseTableVisitor
        {
            get { return this._tFactory; }
        }

        public Parameter Render(PulseParser.ParamContext context)
        {

            // If null, treat as missing //
            if (context == null)
                return new Parameter();
            else if (context.scalar_expression() != null)
                return new Parameter(this._sFactory.Render(context.scalar_expression()));
            else if (context.matrix_expression() != null)
                return new Parameter(this._mFactory.Render(context.matrix_expression()));
            else if (context.record_expression() != null)
                return new Parameter(this._rFactory.Render(context.record_expression()));
            else if (context.table_expression() != null)
                return new Parameter(this._tFactory.Render(context.table_expression()));

            throw new Exception("Context is invalid");

        }

        public List<Parameter> Render(PulseParser.ParamContext[] context)
        {

            List<Parameter> parameters = new List<Parameter>();
            foreach (PulseParser.ParamContext ctx in context)
            {
                parameters.Add(this.Render(ctx));
            }
            return parameters;

        }

    }


}
