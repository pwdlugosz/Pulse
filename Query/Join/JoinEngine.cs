using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Expressions;
using Pulse.Util;

namespace Pulse.Query.Join
{

    public abstract class JoinEngine
    {

        /// <summary>
        /// 
        /// </summary>
        public JoinEngine()
        {
        }

        /// <summary>
        /// Renders the join over two tables
        /// </summary>
        /// <param name="Left">The left table</param>
        /// <param name="Right">The right table</param>
        /// <param name="Predicate">The join predicate</param>
        /// <param name="Fields">The fields to keep</param>
        /// <param name="Where">The filter to apply</param>
        /// <param name="Type">The type of join to perform</param>
        /// <param name="ActualCost">Output of the actual cost of running this join</param>
        public abstract void Render(WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ExpressionCollection Fields, Filter Where, JoinType Type, JoinMetaData MetaData);

        /// <summary>
        /// Renders the join over two tables
        /// </summary>
        /// <param name="Left">The left table</param>
        /// <param name="Right">The right table</param>
        /// <param name="Predicate">The join predicate</param>
        /// <param name="Fields">The fields to keep</param>
        /// <param name="Where">The filter to apply</param>
        /// <param name="Type">The type of join to perform</param>
        public void Render(WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ExpressionCollection Fields, Filter Where, JoinType Type)
        {
            JoinMetaData x = new JoinMetaData();
            this.Render(Output, Left, Right, Predicate, Fields, Where, Type, x);
        }

    }

    
}
