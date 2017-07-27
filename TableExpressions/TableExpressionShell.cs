using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.TableExpressions
{
    
    public class TableExpressionShell : TableExpression
    {

        Schema _Columns;

        public TableExpressionShell(Host Host, TableExpression Parent, Schema Columns)
            : base(Host, Parent)
        {
            this._Columns = Columns;
            this.Alias = "SHELL";
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override void Evaluate(RecordWriter Writer)
        {
            
        }

        public override long EstimatedCount
        {
            get
            {
                return 0;
            }
        }

        public override bool IsIndexedBy(Key IndexColumns)
        {
            return false;
        }

    }

}
