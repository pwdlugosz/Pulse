using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.RecordExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionLiteral : MatrixExpression
    {

        private CellMatrix _value;

        public MatrixExpressionLiteral(MatrixExpression Parent, CellMatrix Value)
            : base(Parent)
        {
            this._value = Value;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            return this._value;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._value.Affinity;
        }

        public override int ReturnSize()
        {
            return this._value.Size;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionLiteral(this.ParentNode, new CellMatrix(this._value));
        }

    }

    public sealed class MatrixExpressionLiteral2 : MatrixExpression
    {

        private List<ScalarExpressionSet> _exp;
        private int _MaxCol = -1;
        private CellAffinity _ReturnAffinity = CellAffinity.BOOL;
        private int _ReturnSize = -1;

        public MatrixExpressionLiteral2(MatrixExpression Parent, List<ScalarExpressionSet> Values)
            : base(Parent)
        {
            this._exp = Values;
            foreach (ScalarExpressionSet ses in this._exp)
            {
                this._ReturnAffinity = CellAffinityHelper.Highest(ses.MaxAffinity, this._ReturnAffinity);
                this._ReturnSize = Math.Max(ses.MaxSize, this._ReturnSize);
                this._MaxCol = Math.Max(ses.Columns.Count, this._MaxCol);
            }

        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {

            CellMatrix m = new CellMatrix(this._exp.Count, this._MaxCol, this._ReturnAffinity, this._ReturnSize);
            Cell n = CellValues.Null(this._ReturnAffinity);
            for (int i = 0; i < this._exp.Count; i++)
            {

                for (int j = 0; j < this._MaxCol; j++)
                {

                    if (this._exp[i].Count > j)
                        m[i, j] = this._exp[i][j].Evaluate(Variant);

                }
            
            }

            return m;

        }

        public override CellAffinity ReturnAffinity()
        {
            return this._ReturnAffinity;
        }

        public override int ReturnSize()
        {
            return this._ReturnSize;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionLiteral2(this.ParentNode, this._exp);
        }

    }

    public sealed class MatrixExpressionCTOR : MatrixExpression
    {

        private ScalarExpression _Rows;
        private ScalarExpression _Columns;
        private CellAffinity _Affinity;
        private int _Size;

        public MatrixExpressionCTOR(MatrixExpression Parent, ScalarExpression Rows, ScalarExpression Columns, CellAffinity Affinity, int Size)
            : base(Parent)
        {
            this._Rows = Rows;
            this._Columns = Columns;
            this._Affinity = Affinity;
            this._Size = Size;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {

            int rows = this._Rows.Evaluate(Variant).valueINT;
            int cols = this._Columns.Evaluate(Variant).valueINT;
            return new CellMatrix(rows, cols, this._Affinity, this._Size);

        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Affinity;
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionCTOR(this.ParentNode, this._Rows, this._Columns, this._Affinity, this._Size);
        }

    }

}
