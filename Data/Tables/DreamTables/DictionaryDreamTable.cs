using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents an in memory only key-value table
    /// </summary>
    public class DictionaryDreamTable : ClusteredDreamTable
    {

        protected int _KeyCount = 0;
        protected int _ValueCount = 0;
        protected RecordKey _LastRef = RecordKey.RecordNotFound;
        protected Key _KeyFields;
        protected Key _ValueFields;

        public DictionaryDreamTable(Host Host, string Name, Schema KeyColumns, Schema ValueColumns, int PageSize)
            : base(Host, Name, Schema.Join(KeyColumns, ValueColumns), Data.Key.Build(KeyColumns.Count), true)
        {
            this._KeyCount = KeyColumns.Count;
            this._ValueCount = ValueColumns.Count;
            this._KeyFields = Data.Key.Build(this._KeyCount);
            this._ValueFields = Data.Key.Build(this._KeyCount, this._ValueCount);
            this._MaxRecords = DreamTable.MAX_MEMORY / Columns.RecordDiskCost;
            this._TableType = "DICTIONARY_DREAM";
        }

        // Methods not implemented //   
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
        /// Splits a table into N sub tables
        /// </summary>
        /// <param name="PartitionIndex"></param>
        /// <param name="PartitionCount"></param>
        /// <returns></returns>
        public override Table Partition(int PartitionIndex, int PartitionCount)
        {
            throw new NotImplementedException();
        }

        // Dictionary Methods //
        /// <summary>
        /// Adds a key-value pair
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void Add(Record Key, Record Value)
        {

            // Check that everything's ok //
            if (Key.Count != this._KeyCount || Value.Count != this._ValueCount)
                throw new ArgumentException("Key or value passed is/are invalid");

            Record r = Record.Join(Key, Value);

            this.Insert(r);

        }

        /// <summary>
        /// Sets a value
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void SetValue(Record Key, Record Value)
        {

            Record r = Record.Join(Key, Value);
            BPlusTreePage p = this._Cluster.SeekPage(r);
            int idx = p.Search(r);
            if (idx < 0)
                throw new BPlusTree.DuplicateKeyException("Key not found");
            p.Update(r, idx);

        }

        /// <summary>
        /// Gets a value from the tree
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetValue(Record Key)
        {

            Record x = this.GetKeyValue(Key);

            if (x == null)
                return null;

            return Record.Split(x, this._ValueFields);

        }

        /// <summary>
        /// Gets a record, invluding the key and value
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetKeyValue(Record Key)
        {

            // Get the record key //
            RecordKey x = this._Cluster.SeekFirst(Key);

            // This should really only trigger if the table is empty; actually, this should never trigger //
            if (x.IsNotFound)
                return null;

            // Get the record //
            Record y = this._Cluster.Storage.Select(x);

            // Need to check if we didn't find the actual record //
            if (Record.Compare(y, Key, this._KeyFields) == 0)
                return y;

            // Not found //
            return null;

        }

        /// <summary>
        /// Checks if the table contains a key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool ContainsKey(Record Key)
        {
            return this.GetKeyValue(Key) == null;
        }

    }


}
