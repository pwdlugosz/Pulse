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

        public ActionExpressionForEach(Host Host, ActionExpression Parent, TableExpression Data, string Alias)
            : base(Host, Parent)
        {
            this._t = Data;
            this._a = Alias;
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

            // Set up the resolver //
            if (!Variant.Local.ExistsRecord(this._a))
                Variant.Local.DeclareRecord(this._a, new AssociativeRecord(t.Columns));

            while (rr.CanAdvance)
            {
                Variant.SetRecord(FieldResolver.LOCAL, this._a, new AssociativeRecord(t.Columns, rr.ReadNext())); 
                this._Children.ForEach((x) => { x.Invoke(Variant); });
            }

            if (this._Host.IsSystemTemp(t))
                this._Host.TableStore.DropTable(t.Key);

        }

        public override FieldResolver CreateResolver()
        {
            FieldResolver f = base.CreateResolver();
            return f;
        }

    }

}
