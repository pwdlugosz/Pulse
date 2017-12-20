using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.RecordExpressions;
using Pulse.Elements;

namespace Pulse.Expressions.ScalarExpressions
{

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

    public sealed class ScalarExpressionRecordRef : ScalarExpression
    {

        private string _StoreName;
        private string _RecordName;
        private string _FieldName;
        private CellAffinity _CellAffinity;
        private int _Size;

        public ScalarExpressionRecordRef(ScalarExpression Parent, string StoreName, string RecordName, string FieldName, CellAffinity Affinity, int Size)
            : base(Parent, ScalarExpressionAffinity.Heap)
        {
            this._StoreName = StoreName;
            this._RecordName = RecordName;
            this._FieldName = FieldName;
            this._CellAffinity = Affinity;
            this._Size = Size;
        }

        public string StoreName
        {
            get { return this._StoreName; }
        }

        public string RecordName
        {
            get { return this._RecordName; }
        }

        public string FieldName
        {
            get { return this._FieldName; }
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return null;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionRecordRef(this._ParentNode, this._StoreName, this._RecordName, this._FieldName, this._CellAffinity, this._Size);
        }

        public override int ExpressionSize()
        {
            return this._Size;
        }

        public override CellAffinity ExpressionReturnAffinity()
        {
            return this._CellAffinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return Variants.Stores[this._StoreName].GetRecord(this._RecordName)[this._FieldName];
        }

    }

}
