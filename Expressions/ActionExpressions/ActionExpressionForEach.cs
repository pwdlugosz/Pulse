using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.TableExpressions;
using Pulse.Tables;
using Pulse.Expressions;

namespace Pulse.Expressions.ActionExpressions
{
    
    public class ActionExpressionForEach : ActionExpression
    {

        private TableExpression _t;
        private string _a;
        private int _hidx = 0;

        public ActionExpressionForEach(Host Host, ActionExpression Parent, TableExpression Data, string Alias, int HeapIndex)
            : base(Host, Parent)
        {
            this._t = Data;
            this._a = Alias;
            this._hidx = HeapIndex;
        }

        public override void BeginInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => x.BeginInvoke(Variant));
        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => x.EndInvoke(Variant));
        }

        public override void Invoke(FieldResolver Variant)
        {

            Table t = this._t.Select(Variant);
            RecordReader rr = t.OpenReader();

            while (rr.CanAdvance)
            {
                Variant.SetValue(this._hidx, rr.ReadNext());
                this._Children.ForEach((x) => { x.Invoke(Variant); });
            }

            if (this._Host.IsSystemTemp(t))
                this._Host.Store.DropTable(t.Key);

        }

        public override FieldResolver CreateResolver()
        {
            FieldResolver f = base.CreateResolver();
            f.AddSchema(this._a, this._t.Columns);
            return f;
        }

    }

}
