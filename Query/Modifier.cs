using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;


namespace Pulse.Query
{
    
    /// <summary>
    /// Represents a table expression modifier
    /// </summary>
    public sealed class Modifier
    {

        public Modifier(Key OrderBy, bool Distinct)
        {
            this.OrderByColumns = OrderBy;
            this.Distinct = Distinct;
        }

        public Modifier(Key OrderBy)
            :this(OrderBy, false)
        {
        }

        public Modifier(bool Distinct)
            : this(new Key(), Distinct)
        {
        }

        public Modifier()
            : this(new Key(), false)
        {
        }

        public Key OrderByColumns
        {
            get;
            private set;
        }

        public bool Distinct
        {
            get;
            set;
        }

        public Table Create(Host Host, string DB, string Name, Schema Columns)
        {

            // No order by or distinct
            if (this.OrderByColumns.Count == 0 && !this.Distinct)
            {
                return new HeapScribeTable(Host, Name, Host.Connections[DB], Columns, Page.DEFAULT_SIZE);
            }
            // Yes order by, no distinct, create a clustered table
            else if (this.OrderByColumns.Count != 0 && !this.Distinct)
            {
                return new ClusteredScribeTable(Host, Name, Host.Connections[DB], Columns, OrderByColumns, ClusterState.Universal);
            }
            // No order by and yes distinct //
            else if (this.OrderByColumns.Count == 0 && this.Distinct)
            {
                return new ClusteredScribeTable(Host, Name, Host.Connections[DB], Columns, Columns.KeyParse("*"), ClusterState.Distinct);
            }
            // both order by and distinct
            else if (this.OrderByColumns.Count != 0 && this.Distinct)
            {
                return new ClusteredScribeTable(Host, Name, Host.Connections[DB], Columns, this.OrderByColumns, ClusterState.Distinct);
            }

            throw new Exception("Critical error");

        }

    }

}
