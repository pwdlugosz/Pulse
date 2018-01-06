using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;

namespace Pulse.Expressions.ScalarExpressions
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class ScalarExpressionFieldRef2 : ScalarExpression
    {

        private string _StoreName = FieldResolver.LOCAL;
        private string _RecordName = null;
        private string _FieldName = null;
        private int _FieldSize;
        private CellAffinity _FieldAffinity;
        private Host _Host;

        public ScalarExpressionFieldRef2(Host Host, ScalarExpression Parent, string RecordName, string FieldName, CellAffinity FieldAffinity, int FieldSize)
            : base(Parent, ScalarExpressionAffinity.Field)
        {
            this._RecordName = RecordName;
            this._FieldName = FieldName;
            this._FieldAffinity = FieldAffinity;
            this._FieldSize = FieldSize;
            this._Host = Host;
        }

        // Field Name //
        public string StoreName
        {
            get { return this._StoreName; }
        }

        public string RecordName
        {
            get { return this._RecordName; }
        }

        // Overrides //
        public override string Unparse(FieldResolver Variants)
        {
            return this._RecordName + "." + this._FieldName;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionFieldRef2(this._Host, this._ParentNode, this._RecordName, this._FieldName, this._FieldAffinity, this._FieldSize);
        }

        public override int ReturnSize()
        {
            return this._FieldSize;
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._FieldAffinity;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            return Variants.GetRecord(this._StoreName, this._RecordName)[this._FieldName];
        }

        public override int GetHashCode()
        {
            return this._StoreName.GetHashCode() ^ this._RecordName.GetHashCode() ^ this._FieldName.GetHashCode() ^ (int)this._FieldAffinity ^ this._FieldSize;
        }

        public override string BuildAlias()
        {
            return this._FieldName;
        }

    }


}
