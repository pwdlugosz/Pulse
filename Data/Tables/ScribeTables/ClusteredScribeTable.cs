using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Creates a table sorted usina a b+tree that can be saved to disk
    /// </summary>
    public class ClusteredScribeTable : ScribeTable
    {

        protected BPlusTree _Cluster;

        /// <summary>
        /// This method should be used for creating a table object from an existing table on disk
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Header"></param>
        /// <param name="SortKey"></param>
        public ClusteredScribeTable(Host Host, TableHeader Header)
            : base(Host, Header)
        {

            if (Header.RootPageID == -1)
                throw new ArgumentException("The root page ID cannot be null");

            // Get the sort key //
            Key k = Header.SortKey;

            // Get the root page ID //
            BPlusTreePage root = BPlusTreePage.Mutate(this.GetPage(Header.RootPageID), k);

            // Cluster //
            this._Cluster = new BPlusTree(this, this.Columns, k, root, this.Header, false);

            this._TableType = "CLUSTER_SCRIBE";

        }

        /// <summary>
        /// This method should be used for creating a brand new table object
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Alias"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="PageSize"></param>
        /// <param name="SortKey"></param>
        public ClusteredScribeTable(Host Host, string Name, string Dir, Schema Columns, Key ClusterColumns, bool IsUnique, int PageSize)
            : base(Host, Name, Dir, Columns, PageSize)
        {

            this._Cluster = BPlusTree.CreateClusteredIndex(this, ClusterColumns, IsUnique);
            this._TableType = "CLUSTER_SCRIBE";
            this._Header.SortKey = ClusterColumns;

        }

        /// <summary>
        /// This method should be used for creating a brand new table object
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Alias"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        public ClusteredScribeTable(Host Host, string Name, string Dir, Schema Columns, Key ClusterColumns, bool IsUnique)
            : this(Host, Name, Dir, Columns, ClusterColumns, IsUnique, Page.DEFAULT_SIZE)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Name"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        public ClusteredScribeTable(Host Host, string Name, string Dir, Schema Columns, Key ClusterColumns)
            : this(Host, Name, Dir, Columns, ClusterColumns, false, Page.DEFAULT_SIZE)
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
            RecordKey l = this._Cluster.SeekFirst(Key, false);
            RecordKey u = this._Cluster.SeekLast(Key, false);
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
            RecordKey lk = this._Cluster.SeekFirst(LKey, false);
            RecordKey uk = this._Cluster.SeekLast(UKey, false);
            return new VanillaReadStream(this, lk, uk);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="IndexColumns"></param>
        public override void CreateIndex(string Name, Key IndexColumns)
        {
            throw new Exception("Cannot create indexes on clustered tables");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public override Index GetIndex(string Name)
        {
            throw new Exception("Cannot request indexes on clustered tables");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public override Index GetIndex(Key IndexColumns)
        {
            if (Data.Key.EqualsStrong(IndexColumns, this._Header.SortKey))
                return new DerivedIndex(this);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PreSerialize()
        {
            this.SetPage(this._Cluster.Root);
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
