using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.TableExpressions;

namespace Pulse.ActionExpressions
{
    
    public class ActionExpressionTableAssign : ActionExpression
    {

        private string _DataBase;
        private string _Name;
        private TableExpression _Data;

        public ActionExpressionTableAssign(Host Host, ActionExpression Parent, string Database, string Name, TableExpression Data)
            : base(Host, Parent)
        {
            this._DataBase = Database;
            this._Name = Name;
            this._Data = Data;
        }

        public override void Invoke(FieldResolver Variant)
        {
            Table t = this._Data.RenderTable(this._DataBase, this._Name, Variant);
        }

    }

}
