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
        public abstract void Render(Host Host, RecordWriter Output, Table Data, FieldResolver Variants, int RecordRef, 
            ScalarExpressionCollection Fields, Filter Where, long Limit, SelectMetaData MetaData);

        public void Render(Host Host, RecordWriter Output, Table Data, FieldResolver Variants, int RecordRef, ScalarExpressionCollection Fields, Filter Where)
        {
            this.Render(Host, Output, Data, Variants, RecordRef, Fields, Where, -1, new SelectMetaData());
        }


    }

}
