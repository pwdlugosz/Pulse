using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Select
{


    /// <summary>
    /// Performs a select over the entire
    /// </summary>
    public abstract class SelectEngine
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Output"></param>
        /// <param name="Data"></param>
        /// <param name="Keys"></param>
        /// <param name="Where"></param>
        /// <param name="MetaData"></param>
        public abstract void Render(Host Host, WriteStream Output, Table Data, ScalarExpressionCollection Fields, Filter Where, long Limit, SelectMetaData MetaData);

        public void Render(Host Host, WriteStream Output, Table Data, ScalarExpressionCollection Fields, Filter Where)
        {
            this.Render(Host, Output, Data, Fields, Where, -1, new SelectMetaData());
        }

        public ClusteredScribeTable Select(Host Host, string DB, string Name, Key ClusterColumns, Table Data, ScalarExpressionCollection Fields, Filter Where)
        {

            ClusteredScribeTable t = Data.Host.CreateTable(DB, Name, Fields.Columns, ClusterColumns);
            WriteStream w = t.OpenWriter();
            this.Render(Host, w, Data, Fields, Where);
            w.Close();
            return t;

        }

        public ClusteredScribeTable Select(Host Host, string DB, string Name, Key ClusterColumns, Table Data, ScalarExpressionCollection Fields)
        {
            return this.Select(Host, DB, Name, ClusterColumns, Data, Fields, Filter.TrueForAll);
        }

        public HeapScribeTable Select(Host Host, string DB, string Name, Table Data, ScalarExpressionCollection Fields, Filter Where)
        {

            HeapScribeTable t = Data.Host.CreateTable(DB, Name, Fields.Columns);
            WriteStream w = t.OpenWriter();
            this.Render(Host, w, Data, Fields, Where);
            w.Close();
            return t;

        }

        public HeapScribeTable Select(Host Host, string DB, string Name, Table Data, ScalarExpressionCollection Fields)
        {
            return this.Select(Host, DB, Name, Data, Fields, Filter.TrueForAll);
        }


    }

}
