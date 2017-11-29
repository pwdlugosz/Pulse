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

namespace Pulse.Expressions
{

    //public abstract class ExpressionFunctionBase : IExpressionFunction
    //{

    //    protected Host _Host;
    //    protected List<IExpression> _Parameters;
    //    protected string _Name;

    //    public ExpressionFunctionBase(Host Host, string Name)
    //    {
    //        this._Host = Host;
    //        this._Name = Name;
    //        this._Parameters = new Heap<IExpression>();
    //    }

    //    public void AddParameter(IExpression Value)
    //    {
    //        this._Parameters.Add(Value);
    //    }

    //    public sealed class ExpressionIf : ExpressionFunctionBase
    //    {

    //        public ExpressionIf(Host Host)
    //            : base(Host, "IF")
    //        {
    //        }

    //        public SuperExpressionAffinity Affinity
    //        {
    //            get { return SuperExpressionAffinity.Scalar; }
    //        }

    //        public ScalarExpression Scalar
    //        {
    //            get
    //            {
    //                var x = new ScalarExpressionFunction.ExpressionIf();
    //                x.AddChildren(this._Parameters[0].Scalar, this._Parameters[1].Scalar, this._Parameters[2].Scalar);
    //                return x;
    //            }
    //        }

    //        public MatrixExpression Matrix
    //        {
    //            get { return null; }
    //        }

    //        public RecordExpression Record
    //        {
    //            get { return null; }
    //        }

    //        public TableExpression Table
    //        {
    //            get { return null; }
    //        }

    //    }


    //}

}
