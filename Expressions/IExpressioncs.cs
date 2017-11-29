using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;

namespace Pulse.Expressions
{


    public enum SuperExpressionAffinity
    {
        Scalar,
        Matrix,
        Record,
        Table
    }

    public interface IExpression
    {

        SuperExpressionAffinity SuperAffinity { get; }

        ScalarExpression Scalar { get; }

        MatrixExpression Matrix { get; }

        RecordExpression Record { get; }

        TableExpression Table { get; }

    }

    public interface IExpressionFunction : IExpression
    {

        void AddParameter(IExpression Value);

    }

}
