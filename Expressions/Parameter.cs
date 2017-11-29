using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions
{


    public sealed class Parameter
    {

        public enum ParameterAffinity
        {
            Scalar,
            Record,
            Matrix,
            Table
        }

        public Parameter(ScalarExpression Scalar)
        {
            this.Scalar = Scalar;
        }

        public ScalarExpression Scalar
        {
            get;
            private set;
        }

        public ScalarExpressionSet Record
        {
            get;
            private set;
        }

        public MatrixExpression Matrix
        {
            get;
            private set;
        }

        public TableExpression Table
        {
            get;
            private set;
        }
    
    }


}
