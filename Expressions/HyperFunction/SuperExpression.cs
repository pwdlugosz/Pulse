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
using Pulse.Elements;
using Pulse.Tables;

namespace Pulse.Expressions.SuperExpressions
{
    
    
    public abstract class SuperExpression
    {

        protected List<SuperExpression> _Children;
        protected SuperExpression _Parent;
        
        public SuperExpression(SuperExpression Parent)
        {
            this._Parent = Parent;
        }

        public SuperExpression Parent
        {
            get { return this._Parent; }
            set { this._Parent = value; }
        }

        public List<SuperExpression> Children
        {
            get { return this._Children; }
        }

        public void AddChild(SuperExpression Expression)
        {
            Expression.Parent = Expression;
            this._Children.Add(Expression);
        }

        // Table/Record Expression MetaData //
        public abstract Schema ReturnColumns
        {
            get;
        }

        // Scalar / Matrix Expression //
        public abstract CellAffinity ReturnAffinity
        {
            get;
        }

        public abstract int ReturnSize
        {
            get;
        }

        // Evaluations //
        public abstract SuperAffinity Affinity
        {
            get;
        }

        public abstract ScalarExpression RenderScalarExpression(FieldResolver Fields);

        public abstract RecordExpression RenderRecordExpression(FieldResolver Fields);

        public abstract MatrixExpression RenderMatrixExpression(FieldResolver Fields);

        public abstract TableExpression RenderTableExpression(FieldResolver Fields);

    }

    public abstract class SuperExpressionFunction : SuperExpression
    {

        private int _MaxParameterCount = -1;
        private string _Name;

        public SuperExpressionFunction(SuperExpression Parent, string Name, int MaxParameterCount)
            : base(Parent)
        {
            this._Name = Name;
            this._MaxParameterCount = MaxParameterCount;
        }

        protected virtual void CheckParameters()
        {

            if (this._MaxParameterCount < 0 && this._Children.Count > Math.Abs(this._MaxParameterCount))
            {
                throw new Exception(string.Format("Expecting at most {0} parameters; passed {1}", Math.Abs(this._MaxParameterCount), this._Children.Count));   
            }
            else if (this._Children.Count != this._MaxParameterCount)
            {
                throw new Exception(string.Format("Expecting {0} parameters; passed {1}", this._MaxParameterCount, this._Children.Count));
            }

        }

        public class SuperExpressionFunctionSplit : SuperExpressionFunction
        {

            public SuperExpressionFunctionSplit()
                : base(null, "SPLIT", -3)
            {
            }

            public override SuperAffinity Affinity
            {
                get { return SuperAffinity.MatrixExpression; }
            }

            public override CellAffinity ReturnAffinity
            {
                get { return CellAffinity.STRING; }
            }

            public override int ReturnSize
            {
                get { return this._Children[0].ReturnSize; }
            }

            public override MatrixExpression RenderMatrixExpression(FieldResolver Fields)
            {

                string text = this._Children[0].RenderScalarExpression(Fields).Evaluate(Fields).valueSTRING;
                string delim = this._Children[1].RenderScalarExpression(Fields).Evaluate(Fields).valueSTRING;
                string escap = (this._Children.Count <= 2 ? new string(char.MaxValue, 1) : this._Children[2].RenderScalarExpression(Fields).Evaluate(Fields).valueSTRING);

                string[] v = Util.StringUtil.Split(text, delim.ToCharArray(), escap.ToCharArray()[0]);
                CellMatrix m = new CellMatrix(v.Length, 0, CellValues.NullSTRING);
                for (int i = 0; i < m.RowCount; i++)
                    m[i] = new Cell(v[i]);

                return new MatrixExpressionLiteral(null, m);

            }

            // Non Implemented Functions //
            public override ScalarExpression RenderScalarExpression(FieldResolver Fields)
            {
                throw new NotImplementedException();
            }
            
            public override RecordExpression RenderRecordExpression(FieldResolver Fields)
            {
                throw new NotImplementedException();
            }

            public override TableExpression RenderTableExpression(FieldResolver Fields)
            {
                throw new NotImplementedException();
            }

            public override Schema ReturnColumns
            {
                get { throw new NotImplementedException(); }
            }


        }

    }

}
