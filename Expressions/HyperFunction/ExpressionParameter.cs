using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.SuperExpressions
{

    public sealed class SuperExpressionParameter
    {

        // CTOR //
        public SuperExpressionParameter(ScalarExpression Value)
        {
            this.Affinity = SuperAffinity.ScalarExpression;
            this.ScalarExpression = Value;
        }

        public SuperExpressionParameter(RecordExpression Value)
        {
            this.Affinity = SuperAffinity.RecordExpression;
            this.RecordExpression = Value;
        }

        public SuperExpressionParameter(MatrixExpression Value)
        {
            this.Affinity = SuperAffinity.MatrixExpression;
            this.MatrixExpression = Value;
        }

        public SuperExpressionParameter(TableExpression Value)
        {
            this.Affinity = SuperAffinity.TableExpression;
            this.TableExpression = Value;
        }

        // Members //
        public SuperAffinity Affinity
        {
            get;
            private set;
        }

        public ScalarExpression ScalarExpression
        {
            get;
            private set;
        }

        public RecordExpression RecordExpression
        {
            get;
            private set;
        }

        public MatrixExpression MatrixExpression
        {
            get;
            private set;
        }

        public TableExpression TableExpression
        {
            get;
            private set;
        }

    }

}
