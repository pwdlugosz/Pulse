using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Aggregates;
using Pulse.Data;
using Pulse.ScalarExpressions;

namespace Pulse.Query.Aggregate
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class AggregateEngine
    {

        /// <summary>
        /// Groups values by keys
        /// </summary>
        /// <param name="Output">The stream to write the data to</param>
        /// <param name="Data">The source data</param>
        /// <param name="Keys">The expressions that form the unique set in the data</param>
        /// <param name="Values">The aggregate functions over which to consolidate the data</param>
        /// <param name="Where">The filter to apply to the data</param>
        /// <param name="MetaData">The meta data to update</param>
        public abstract void Render(Host Host, RecordWriter Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, 
            ScalarExpressionCollection Select, AggregateMetaData MetaData);

        /// <summary>
        /// Groups values by keys
        /// </summary>
        /// <param name="Output">The stream to write the data to</param>
        /// <param name="Data">The source data</param>
        /// <param name="Keys">The expressions that form the unique set in the data</param>
        /// <param name="Values">The aggregate functions over which to consolidate the data</param>
        /// <param name="Where">The filter to apply to the data</param>
        /// <param name="MetaData">The meta data to update</param>
        public void Render(Host Host, RecordWriter Output, Table Data, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, 
            ScalarExpressionCollection Select)
        {
            AggregateMetaData meta = new AggregateMetaData();
            this.Render(Host, Output, Data, Keys, Values, Where, Select, meta);
        }

        // Supporting aggregates //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetOutputSchema(ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            return Schema.Join(Keys.Columns, Values.Columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public Schema GetWorkSchema(ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            return Schema.Join(Keys.Columns, Values.WorkColumns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WorkData"></param>
        /// <param name="Key"></param>
        public virtual void OverLay(Record WorkData, Record Key)
        {
            Array.Copy(Key._data, WorkData._data, Key.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Keys"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public virtual Record GetWorkRecord(ScalarExpressionCollection Keys, AggregateCollection Values)
        {
            int woffset = Keys.Count;
            Record r = this.GetWorkSchema(Keys, Values).NullRecord;
            Values.Initialize(r, woffset);
            return r;
        }

    }

}
