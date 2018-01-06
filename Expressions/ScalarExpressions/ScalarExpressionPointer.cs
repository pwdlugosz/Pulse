using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;

namespace Pulse.Expressions.ScalarExpressions
{

    public class ScalarExpressionPointer : ScalarExpression
    {

        private string _NameID;
        private CellAffinity _Type;
        private int _Size;
        private Host _Host;

        public ScalarExpressionPointer(Host Host, ScalarExpression Parent, string RefName, CellAffinity Type, int Size)
            : base(Parent, ScalarExpressionAffinity.Pointer)
        {
            this._Type = Type;
            this._NameID = RefName;
            this._name = RefName;
            this._Size = CellSerializer.FixLength(Type, Size);
            this._Host = Host;
        }

        public override Cell Evaluate(FieldResolver Variants)
        {
            throw new Exception(string.Format("Cannot evaluate pointer nodes; Name '{0}'", _NameID));
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Type;
        }

        public override string ToString()
        {
            return this.Affinity.ToString() + " : " + _NameID;
        }

        public override string Unparse(FieldResolver Variants)
        {
            //return this._Type.ToString().ToUpper() + "." + this._NameID.ToString();
            return this._NameID;
        }

        public override ScalarExpression CloneOfMe()
        {
            return new ScalarExpressionPointer(this._Host, this.ParentNode, this._NameID, this._Type, this._Size);
        }

        public override int ReturnSize()
        {
            return this._Size;
        }

        public string PointerName
        {
            get { return _NameID; }
        }

    }

}
