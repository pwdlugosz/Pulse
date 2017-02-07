using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents a tables with a given clustered index
    /// </summary>
    public class ClusteredDreamTable : DreamTable
    {

        protected BPlusTree _Cluster;
        protected int _MaxRecords;

        public ClusteredDreamTable(Host Host, string Name, Schema Columns, Key IndexColumns, int PageSize)
            : base(Host, Name, Columns, PageSize, TableState.ReadWrite)
        {
            this._Cluster = BPlusTree.CreateClusteredIndex(this, IndexColumns);
            this._MaxRecords = DreamTable.MAX_MEMORY / Columns.RecordDiskCost;
            this._TableType = "CLUSTER_DREAM";
        }

        public ClusteredDreamTable(Host Host, string Name, Schema Columns, Key IndexColumns)
            : this(Host, Name, Columns, IndexColumns, Page.DEFAULT_SIZE)
        {
        }

        /// <summary>
        /// Inner B+Tree
        /// </summary>
        public BPlusTree BaseTree
        {
            get { return this._Cluster; }
        }

        /// <summary>
        /// True if the table cannot accept any more records
        /// </summary>
        public override bool IsFull
        {
            get { return this._MaxRecords < this._Header.RecordCount; }
        }

        /// <summary>
        /// Appends a record to a table
        /// </summary>
        /// <param name="Value"></param>
        public override void Insert(Record Value)
        {
            this._Cluster.Insert(Value);
        }

        /// <summary>
        /// Opens a record reader that focuses on a single key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public override ReadStream OpenReader(Record Key)
        {

            if (Key.Count != this._Cluster.IndexColumns.Count)
                return this.OpenReader();
            RecordKey l = this._Cluster.SeekFirst(Key);
            RecordKey u = this._Cluster.SeekLast(Key);
            return new VanillaReadStream(this, l, u);

        }

        /// <summary>
        /// Opens a record reader to focus on records between A and B (inclusive)
        /// </summary>
        /// <param name="LKey"></param>
        /// <param name="UKey"></param>
        /// <returns></returns>
        public override ReadStream OpenReader(Record LKey, Record UKey)
        {

            if (LKey.Count != UKey.Count || LKey.Count != this._Cluster.IndexColumns.Count)
                return this.OpenReader();
            RecordKey lk = this._Cluster.SeekFirst(LKey);
            RecordKey uk = this._Cluster.SeekLast(UKey);
            return new VanillaReadStream(this, lk, uk);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IndexColumns"></param>
        public override void CreateIndex(string Name, Key IndexColumns)
        {
            throw new Exception("Cannot create indexes on clustered tables");
        }

        // Methods not implemented //
        /// <summary>
        /// Splits a table into N sub tables
        /// </summary>
        /// <param name="PartitionIndex"></param>
        /// <param name="PartitionCount"></param>
        /// <returns></returns>
        public override Table Partition(int PartitionIndex, int PartitionCount)
        {
            throw new NotImplementedException();
        }

    }

}
