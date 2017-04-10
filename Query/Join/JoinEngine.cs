﻿using System;
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
        public abstract void Render(Host Host, WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type, JoinMetaData MetaData);

        /// <summary>
        /// Renders the join over two tables
        /// </summary>
        /// <param name="Left">The left table</param>
        /// <param name="Right">The right table</param>
        /// <param name="Predicate">The join predicate</param>
        /// <param name="Fields">The fields to keep</param>
        /// <param name="Where">The filter to apply</param>
        /// <param name="Type">The type of join to perform</param>
        public void Render(Host Host, WriteStream Output, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type)
        {
            JoinMetaData x = new JoinMetaData();
            this.Render(Host, Output, Left, Right, Predicate, Fields, Where, Type, x);
        }

        public HeapScribeTable Join(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type)
        {

            HeapScribeTable t = Left.Host.CreateTable(DB, Name, Fields.Columns);
            WriteStream w = t.OpenWriter();
            this.Render(Host, w, Left, Right, Predicate, Fields, Where, Type);
            w.Close();
            return t;

        }

        public HeapScribeTable InnerJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Where, JoinType.INNER);
        }

        public HeapScribeTable InnerJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.INNER);
        }

        public HeapScribeTable LeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Where, JoinType.LEFT);
        }

        public HeapScribeTable LeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.LEFT);
        }

        public HeapScribeTable AntiLeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Where, JoinType.ANTI_LEFT);
        }

        public HeapScribeTable AntiLeftJoin(Host Host, string DB, string Name, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.ANTI_LEFT);
        }

        public ClusteredScribeTable Join(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where, JoinType Type)
        {

            ClusteredScribeTable t = Left.Host.CreateTable(DB, Name, Fields.Columns, ClusterColumns);
            WriteStream w = t.OpenWriter();
            this.Render(Host, w, Left, Right, Predicate, Fields, Where, Type);
            w.Close();
            return t;

        }

        public ClusteredScribeTable InnerJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Where, JoinType.INNER);
        }

        public ClusteredScribeTable InnerJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.INNER);
        }

        public ClusteredScribeTable LeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Where, JoinType.LEFT);
        }

        public ClusteredScribeTable LeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.LEFT);
        }

        public ClusteredScribeTable AntiLeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields, Filter Where)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Where, JoinType.ANTI_LEFT);
        }

        public ClusteredScribeTable AntiLeftJoin(Host Host, string DB, string Name, Key ClusterColumns, Table Left, Table Right, RecordMatcher Predicate, ScalarExpressionCollection Fields)
        {
            return this.Join(Host, DB, Name, ClusterColumns, Left, Right, Predicate, Fields, Filter.TrueForAll, JoinType.ANTI_LEFT);
        }


    }

    
}
