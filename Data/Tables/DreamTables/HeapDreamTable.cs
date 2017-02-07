using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// An in memory only table where records are stored in an unorder heap
    /// </summary>
    public class HeapDreamTable : DreamTable
    {

        protected Page _Terminis;
        private RecordKey _LastKey;

        public HeapDreamTable(Host Host, string Name, Schema Columns, int PageSize)
            : base(Host, Name, Columns, PageSize, TableState.ReadWrite)
        {

            // Create the radix page //
            this._Terminis = new Page(this.PageSize, 0, -1, -1, this.Columns);
            this._Header.PageCount++;
            this.SetPage(this._Terminis);
            this._Header.OriginPageID = 0;
            this._TableType = "HEAP_DREAM";
            this._LastKey = RecordKey.RecordNotFound;

        }

        public HeapDreamTable(Host Host, string Name, Schema Columns)
            : this(Host, Name, Columns, Page.DEFAULT_SIZE)
        {
        }

        public override bool IsFull
        {
            get { return this._Terminis.IsFull && this.PageCount == this._PageCapacity; }
        }

        public RecordKey LastInsertKey
        {
            get { return this._LastKey; }
        }

        public override Page TerminalPage
        {
            get
            {
                return this._Terminis;
            }
        }

        public override int TerminalPageID
        {
            get
            {
                return this._Terminis.PageID;
            }
        }

        public override void Insert(Record Value)
        {

            if (this.State == TableState.ReadOnly || this.State == TableState.FullLock)
                throw new Exception("Table is locked for writing");

            // Handle the terminal page being full //
            if (this._Terminis.IsFull)
            {

                Page p = new Page(this.PageSize, this.GenerateNewPageID, this._Terminis.PageID, -1, this.Columns);
                this._Header.PageCount++;
                this._Terminis.NextPageID = p.PageID;
                this.SetPage(p);
                this._Terminis = p;
                this._Header.TerminalPageID = p.PageID;

            }

            // Add the actual record //
            this._Terminis.Insert(Value);
            this.RecordCount++;

            // Get the pointer //
            this._LastKey = new RecordKey(this._Terminis.PageID, this._Terminis.Count - 1);

            // Add to the indexes //
            this._Indexes.Insert(Value, this._LastKey);

        }

        public override Table Partition(int PartitionIndex, int PartitionCount)
        {

            HeapDreamTable t = new HeapDreamTable(this._Host, this.Name, this.Columns, this.PageSize);
            t._State = TableState.ReadOnly;
            int[] counts = this.Map(PartitionCount, (int)this.PageCount);
            int Start = this.StartIndex(PartitionIndex, counts);
            int Count = counts[PartitionIndex];

            for (int i = Start; i < Start + Count; i++)
            {
                Page p = this.GetPage(i);
                t.SetPage(p);
                t._Terminis = p;
            }

            return t;

        }

    }

}
