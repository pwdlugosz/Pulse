using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Util;
using Pulse.ScalarExpressions;

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
        public abstract void Render(Host Host, RecordWriter Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type, JoinMetaData MetaData);

        /// <summary>
        /// Renders the join over two tables
        /// </summary>
        /// <param name="Left">The left table</param>
        /// <param name="Right">The right table</param>
        /// <param name="Predicate">The join predicate</param>
        /// <param name="Fields">The fields to keep</param>
        /// <param name="Where">The filter to apply</param>
        /// <param name="Type">The type of join to perform</param>
        public void Render(Host Host, RecordWriter Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type)
        {
            JoinMetaData x = new JoinMetaData();
            this.Render(Host, Output, Left, Right, Predicate, Fields, Where, Type, x);
        }

        public HeapTable Join(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type)
        {

            HeapTable t = Left.Host.CreateTable(DB, Name, Fields.Columns);
            RecordWriter w = t.OpenWriter();
            this.Render(Host, w, Left, Right, Predicate, Fields, Where, Type);
            w.Close();
            return t;

        }

        public HeapTable InnerJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Where, JoinType.INNER);
        }

        public HeapTable InnerJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.INNER);
        }

        public HeapTable LeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Where, JoinType.LEFT);
        }

        public HeapTable LeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.LEFT);
        }

        public HeapTable AntiLeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Where, JoinType.ANTI_LEFT);
        }

        public HeapTable AntiLeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.ANTI_LEFT);
        }

        public ClusteredTable Join(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type)
        {

            ClusteredTable t = Left.Host.CreateTable(DB, Name, Fields.Columns, ClusterColumns);
            RecordWriter w = t.OpenWriter();
            this.Render(Host, w, Left, Right, Predicate, Fields, Where, Type);
            w.Close();
            return t;

        }

        public ClusteredTable InnerJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Where, JoinType.INNER);
        }

        public ClusteredTable InnerJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.INNER);
        }

        public ClusteredTable LeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Where, JoinType.LEFT);
        }

        public ClusteredTable LeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.LEFT);
        }

        public ClusteredTable AntiLeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Where, JoinType.ANTI_LEFT);
        }

        public ClusteredTable AntiLeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.ANTI_LEFT);
        }


    }

    
}
