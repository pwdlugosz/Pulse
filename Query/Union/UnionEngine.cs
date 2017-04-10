using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Union
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class UnionEngine
    {

        public abstract void Render(Host Host, WriteStream Output, IEnumerable<Table> Tables, UnionMetaData MetaData);

        public void Render(Host Host, WriteStream Output, IEnumerable<Table> Tables)
        {
            this.Render(Host, Output, Tables, new UnionMetaData());
        }

        public ClusteredScribeTable Union(Host Host, string DB, string Name, Key ClusterColumns, IEnumerable<Table> Tables)
        {

            ClusteredScribeTable t = Host.CreateTable(DB, Name, Tables.First().Columns, ClusterColumns);
            WriteStream w = t.OpenWriter();
            this.Render(Host, w, Tables);
            w.Close();
            return t;

        }

        public HeapScribeTable Union(Host Host, string DB, string Name, IEnumerable<Table> Tables)
        {

            HeapScribeTable t = Host.CreateTable(DB, Name, Tables.First().Columns);
            WriteStream w = t.OpenWriter();
            this.Render(Host, w, Tables);
            w.Close();
            return t;

        }

    }

}
