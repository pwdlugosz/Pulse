using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pulse.Data
{

    /// <summary>
    /// This is the base class for all tables
    /// </summary>
    public abstract class Table : IColumns
    {

        /// <summary>
        /// Describes a table's access permission
        /// </summary>
        public enum TableState
        {
            ReadWrite,
            ReadOnly,
            WriteOnly,
            FullLock
        }

        protected Host _Host;
        protected TableState _State = TableState.ReadWrite;
        protected TableHeader _Header;
        protected IndexCollection _Indexes;
        protected string _TableType = "BASE_TABLE";

        public Table(Host Host, TableHeader Header)
        {
            this._Host = Host;
            this._Header = Header;
            this._Indexes = new IndexCollection();
        }

        // Non virtual / abstract //
        /// <summary>
        /// Gets the access state of the table
        /// </summary>
        public TableState State
        {
            get { return this._State; }
            protected set { this._State = value; }
        }

        /// <summary>
        /// Inner table host
        /// </summary>
        public Host Host
        {
            get { return this._Host; }
        }

        // Meta Data //
        /// <summary>
        /// Table's columns
        /// </summary>
        public virtual Schema Columns 
        {
            get { return this._Header.Columns; }
        }

        /// <summary>
        /// The table's names
        /// </summary>
        public virtual string Name 
        {
            get { return this._Header.Name; }
        }

        /// <summary>
        /// The lookup key for the table
        /// </summary>
        public virtual string Key
        {
            get { return this._Header.Key; }
        }

        /// <summary>
        /// The table's header
        /// </summary>
        public virtual TableHeader Header 
        {
            get { return this._Header; }
        }

        // Indexes //
        /// <summary>
        /// Adds an index to the table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Value"></param>
        public virtual void CreateIndex(string Name, Key IndexColumns)
        {
            this._Indexes.CreateIndex(this, Name, IndexColumns);
        }

        /// <summary>
        /// Get's an index from the table
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public virtual Index GetIndex(string Name)
        {
            return this._Indexes[Name];
        }

        /// <summary>
        /// Splits a table into many tables
        /// </summary>
        /// <param name="PartitionIndex"></param>
        /// <param name="PartitionCount"></param>
        /// <returns></returns>
        public abstract Table Partition(int PartitionIndex, int PartitionCount);

        // Reading / Writing Info //
        /// <summary>
        /// Inserts a value into the table
        /// </summary>
        /// <param name="Value"></param>
        public abstract void Insert(Record Value);

        /// <summary>
        /// Inserts many values into a table
        /// </summary>
        /// <param name="Records"></param>
        public virtual void Insert(IEnumerable<Record> Records)
        {
            foreach (Record r in Records)
            {
                this.Insert(r);
            }
        }
        
        /// <summary>
        /// Represents the total record count
        /// </summary>
        public virtual long RecordCount 
        {
            get { return this._Header.RecordCount; }
            set { this._Header.RecordCount = value; }
        }

        /// <summary>
        /// Gets a record from a table
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public virtual Record Select(RecordKey Position)
        {
            return this.GetPage(Position.PAGE_ID).Select(Position.ROW_ID);
        }

        /// <summary>
        /// Attempts to select a record, if it fails to find the record, returns null
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public virtual Record TrySelect(RecordKey Position)
        {
            
            if (this._Header.PageCount <= Position.PAGE_ID)
                return null;

            Page p = this.GetPage(Position.PAGE_ID);

            if (Position.ROW_ID >= p.Count)
                return null;

            return p.Select(Position.ROW_ID);

        }

        /// <summary>
        /// Opens a read stream
        /// </summary>
        /// <returns></returns>
        public virtual ReadStream OpenReader()
        {
            return new VanillaReadStream(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual ReadStream OpenReader(Record Key)
        {
            return new VanillaReadStream(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LKey"></param>
        /// <param name="UKey"></param>
        /// <returns></returns>
        public virtual ReadStream OpenReader(Record LKey, Record UKey)
        {
            return new VanillaReadStream(this);
        }

        /// <summary>
        /// Opens a write stream
        /// </summary>
        /// <returns></returns>
        public virtual WriteStream OpenWriter()
        {
            return new VanillaWriteStream(this);
        }

        /// <summary>
        /// This method gets called right before the table is removed from memory; 
        /// </summary>
        public virtual void PreSerialize()
        {
        }

        // Page Info //
        /// <summary>
        /// Gets the table's page size
        /// </summary>
        public virtual int PageSize 
        { 
            get { return this._Header.PageSize; } 
        }

        /// <summary>
        /// Gets the total page count, including non-data pages
        /// </summary>
        public virtual int PageCount 
        {
            get { return this._Header.PageCount; }
            set { this._Header.PageCount = value; }
        }

        /// <summary>
        /// Gets a page from the table
        /// </summary>
        /// <param name="PageID"></param>
        /// <returns></returns>
        public abstract Page GetPage(int PageID);

        /// <summary>
        /// Sets a age into the table; if the page exists, nothing happens; otherwise it gets added
        /// </summary>
        /// <param name="Key"></param>
        public abstract void SetPage(Page Element);

        /// <summary>
        /// Checks if a page exists in the table
        /// </summary>
        /// <param name="PageID"></param>
        /// <returns></returns>
        public abstract bool PageExists(int PageID);

        /// <summary>
        /// Gets the first data page ID
        /// </summary>
        public virtual int OriginPageID
        {
            get { return this._Header.OriginPageID; }
        }

        /// <summary>
        /// Gets the first data page
        /// </summary>
        public virtual Page OriginPage
        {
            get { return (this.PageExists(this.OriginPageID) ? this.GetPage(this.OriginPageID) : null); }
        }

        /// <summary>
        /// Gets the ID of the last data page
        /// </summary>
        public virtual int TerminalPageID
        {
            get { return this.Header.TerminalPageID; }
            set { this.Header.TerminalPageID = value; }
        }

        /// <summary>
        /// Gets the last data page
        /// </summary>
        public virtual Page TerminalPage
        {
            get { return this.GetPage(this.TerminalPageID); }
        }

        /// <summary>
        /// Generates a brand new page ID that does not exist in the table
        /// </summary>
        public virtual int GenerateNewPageID
        {
            get { return this.PageCount; }
        }

        /// <summary>
        /// Creates a new page
        /// </summary>
        /// <returns></returns>
        public abstract Page GenerateNewPage();

        /// <summary>
        /// Given a page ID, returns a page N steps above or below it
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="Steps"></param>
        /// <param name="SeekDown"></param>
        /// <returns></returns>
        public virtual Page Seek(int PageID, int Steps, bool SeekDown)
        {

            Page p = this.GetPage(PageID);
            for (int i = 0; i < Steps; i++)
            {

                int NewID = (SeekDown ? p.LastPageID : p.NextPageID);
                if (NewID == -1)
                    break;
                p = this.GetPage(NewID);

            }
            return p;

        }

        /// <summary>
        /// A string representing all data pages in order
        /// </summary>
        /// <returns></returns>
        public string PageMap()
        {

            StringBuilder sb = new StringBuilder();
            Page p = this.OriginPage;
            sb.AppendLine(p.MapElement());
            while (p.NextPageID != -1)
            {
                p = this.GetPage(p.NextPageID);
                sb.AppendLine(p.MapElement());
            }

            return sb.ToString();

        }

        /// <summary>
        /// Represents a collection of all the pages in the table
        /// </summary>
        public IEnumerable<Page> Pages
        {
            get { return new PageEnumerator(this); }
        }

        /// <summary>
        /// Splits a page in two, and preserves all other links in the chain
        /// </summary>
        /// <param name="PageID"></param>
        public void ForkPage(int PageID)
        {

            /*
             * Basically, the current page chain looks like this ... p, r, ...
             * But we are going to change it to: ... p', q, r, ...
             * where p' = the lower half of p, and q is the upper half of p on a new page
             * 
             */

            // Get this page //
            Page p = this.GetPage(PageID);

            // Define the new page variables //
            int NewPageID = this.GenerateNewPageID;

            // Create the forked page //
            Page q = p.Split(NewPageID, -1, -1);
            if (this.PageExists(NewPageID))
                throw new Exception(string.Format("Page exists! {0}", NewPageID));

            // Add q after p //
            this.AddPageAfter(p.PageID, q);

            // Add q to the cache //
            this.SetPage(q);

        }

        /// <summary>
        /// Adds a page after a given page id
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="Key"></param>
        public void AddPageAfter(int PageID, Page Element)
        {

            // we want to add Key between PageID and NextPageID //
            Page A = this.GetPage(PageID);

            // Set the elements' LastPageID = A.PageID, and the Key's NextPageID = A.NextPageID
            Element.LastPageID = A.PageID;
            Element.NextPageID = A.NextPageID;
            A.NextPageID = Element.PageID;

            // Set the page ids for the NextPage //
            if (Element.NextPageID != -1)
            {
                Page C = this.GetPage(Element.NextPageID);
                C.LastPageID = Element.PageID;
                if (A.PageID == this._Header.RootPageID)
                    this._Header.RootPageID = Element.PageID;
            }
            // Otherwise, set Key as the new terminal page //
            else
            {
                this.TerminalPageID = Element.PageID;
            }


        }

        // Protected helper //
        protected int[] Map(int PartitionCount, int ElementCount)
        {

            int[] map = new int[PartitionCount];
            for (int i = 0; i < ElementCount; i++)
            {
                int idx = i % PartitionCount;
                map[idx]++;
            }
            return map;

        }

        protected int StartIndex(int PartitionIndex, int[] Map)
        {

            int idx = 0;
            for (int i = 0; i < PartitionIndex; i++)
            {
                idx += Map[i];
            }
            return idx;

        }

        protected void IncrementTerminus()
        {

            Page p = this.GenerateNewPage();
            int NewPageID = this.GenerateNewPageID;
            p.NextPageID = -1;
            p.LastPageID = this.TerminalPageID;


        }

        // Internal debugging //
        internal void Dump(string Path)
        {

            using (StreamWriter sw = new StreamWriter(Path))
            {

                sw.WriteLine(this.Columns.ToNameString('\t'));
                ReadStream rs = this.OpenReader();
                while (rs.CanAdvance)
                {

                    sw.WriteLine(rs.ReadNext().ToString('\t'));

                }

                sw.Flush();

            }

        }

        internal string MetaData()
        {
            return this._TableType + "\n" + this._Header.DebugPrint() + this.PageMap();
        }

        // Statics //
        public static void Dump(string Path, ReadStream Stream)
        {

            using (StreamWriter sw = new StreamWriter(Path))
            {

                sw.WriteLine(Stream.Columns.ToNameString('\t'));
                while (Stream.CanAdvance)
                {
                    sw.WriteLine(Stream.ReadNext().ToString('\t'));
                }
                sw.Flush();

            }

        }

        // Classes //
        protected class PageEnumerator : IEnumerable<Page>, IEnumerator<Page>, IEnumerable, IEnumerator, IDisposable
        {

            private Table _Table;
            private Page _Page;

            public PageEnumerator(Table Table)
            {
                this._Table = Table;
                this._Page = (Table.PageCount == 0 ? null : Table.OriginPage);
            }

            public bool MoveNext()
            {

                if (this._Page == null)
                    return false;
                if (this._Page.NextPageID != -1)
                {
                    this._Page = this._Table.GetPage(this._Page.NextPageID);
                    return true;
                }
                return false;

            }

            Page IEnumerator<Page>.Current
            {
                get
                {
                    return this._Page;
                }
            }

            object IEnumerator.Current
            {
                get { return this._Page; }
            }

            public void Reset()
            {
                if (this._Table.PageCount == 0)
                    this._Page = null;
                else
                    this._Page = this._Table.OriginPage;
            }

            IEnumerator<Page> IEnumerable<Page>.GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            void IDisposable.Dispose()
            {
                // Do nothing
            }

        }

    }

}
