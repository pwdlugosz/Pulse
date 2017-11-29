using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.RecordExpressions;
using Pulse.Elements;

namespace Pulse.Expressions.ScalarExpressions
{


    /// <summary>
    /// Represents a record in a heap
    /// </summary>
    public sealed class ScalarExpressionRecordRef : ScalarExpression
    {

        private ScalarExpressionSet _rex;
        private string _Ref;

        public ScalarExpressionRecordRef(ScalarExpression Parent, ScalarExpressionSet Value, string ColRef)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._rex = Value;
            this._Ref = ColRef;
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return null;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordRef(this._ParentNode, this._rex, this._Ref);
        }

        public override int ExpressionSize()
        {
            return this._rex.Columns.ColumnSize(this._Ref);
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._rex.Columns.ColumnAffinity(this._Ref);
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return this._rex[this._Ref].Evaluate(Variants);
        }

    }

    public sealed class ScalarExpressionRecordRef2 : ScalarExpression
    {

        private RecordExpression _Record;
        private string _Name;

        public ScalarExpressionRecordRef2(ScalarExpression Parent, RecordExpression Value, string Name)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._Record = Value;
            this._Name = Name;
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return null;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordRef2(this._ParentNode, this._Record, this._Name);
        }

        public override int ExpressionSize()
        {
            return this._Record.Columns.ColumnSize(this._Name);
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._Record.Columns.ColumnAffinity(this._Name);
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return this._Record.EvaluateAssociative(Variants)[this._Name];
        }

    }


}
