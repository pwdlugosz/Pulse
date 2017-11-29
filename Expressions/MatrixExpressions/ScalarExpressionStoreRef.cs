using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.ScalarExpressions;

namespace Pulse.Expressions.MatrixExpressions
{

    public sealed class MatrixExpressionStoreRef : MatrixExpression
    {

        private string _Name;
        private ObjectStore _Store;

        public MatrixExpressionStoreRef(MatrixExpression Parent, string Name, ObjectStore Store)
            : base(Parent)
        {
            this._Name = Name;
            this._Store = Store;
        }

        public override CellMatrix Evaluate(FieldResolver Variant)
        {
            return this._Store.Matrixes[this._Name];
        }

        public override CellAffinity ReturnAffinity()
        {
            return this._Store.Matrixes[this._Name].Affinity;
        }

        public override int ReturnSize()
        {
            return this._Store.Matrixes[this._Name].Size;
        }
        
        public override MatrixExpression CloneOfMe()
        {
            return new MatrixExpressionStoreRef(this.ParentNode, this._Name, this._Store);
        }

    }

}
