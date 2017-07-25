using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pulse.Data
{

    /// <summary>
    /// This class holds all in memory pages and manages buffering pages and tables from disk
    /// </summary>
    //public sealed class PageManager
    //{

    //    /// <summary>
    //    /// The default capacity is 32 MB
    //    /// </summary>
    //    public const long DEFAULT_CAPACITY = 1024 * 1024 * 32;

    //    /// <summary>
    //    /// The minimum memory capacity is 8 MB
    //    /// </summary>
    //    public const long MIN_CAPACITY = 1024 * 1024 * 8;

    //    private long _MaxMemory = 0;
    //    private long _Memory = 0;
    //    private Host _Host;

    //    // It's easier to keep two sets of books, one for the dream tables and one for scribe tables //
    //    private Dictionary<string, ScribeTable> _ScribeTables;
    //    private Dictionary<PageUID, Page> _ScribePages;
    //    private Dictionary<PageUID, int> _ScribeWrites;
    //    private Dictionary<string, DreamTable> _DreamTables;
    //    private Dictionary<PageUID, Page> _DreamPages;
    //    private SortedSet<string> _TempObjects;

    //    // This is the collection that holds pages to be burnt //
    //    private FloatingQueue<PageUID> _BurnPile;

    //    // Constructors //
    //    public PageManager(Host Host, long Capacity)
    //    {

    //        this._ScribeTables = new Dictionary<string, ScribeTable>(StringComparer.OrdinalIgnoreCase);
    //        this._ScribePages = new Dictionary<PageUID, Page>(PageUID.DefaultComparer);
    //        this._ScribeWrites = new Dictionary<PageUID, int>(PageUID.DefaultComparer);
    //        this._DreamTables = new Dictionary<string, DreamTable>(StringComparer.OrdinalIgnoreCase);
    //        this._DreamPages = new Dictionary<PageUID, Page>(PageUID.DefaultComparer);
    //        this._BurnPile = new FloatingQueue<PageUID>(4096, PageUID.DefaultComparer);
    //        this._TempObjects = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

    //        this._MaxMemory = Capacity;
    //        this._Host = Host;
            
    //    }

    //    public PageManager(Host Host)
    //        : this(Host, DEFAULT_CAPACITY)
    //    {
    //    }

    //    // Properties //
    //    /// <summary>
    //    /// Maximum memory available
    //    /// </summary>
    //    public long MaxMemory
    //    {
    //        get { return this._MaxMemory; }
    //    }

    //    /// <summary>
    //    /// Memory in use
    //    /// </summary>
    //    public long UsedMemory
    //    {
    //        get { return this._Memory; }
    //    }

    //    /// <summary>
    //    /// Memory that's free
    //    /// </summary>
    //    public long FreeMemory
    //    {
    //        get { return this._MaxMemory - this._Memory; }
    //    }

    //    // Scribe Tables //
    //    public void AddScribeTable(ScribeTable Table)
    //    {

    //        if (this._ScribeTables.ContainsKey(Table.Key))
    //        {
    //            throw new ElementDoesNotExistException(Table.Key);
    //        }

    //        this._ScribeTables.Add(Table.Key, Table);
    //        this._Memory += TableHeader.SIZE;

    //    }

    //    public ScribeTable RequestScribeTable(string Key)
    //    {

    //        if (this._ScribeTables.ContainsKey(Key))
    //        {
    //            return this._ScribeTables[Key];
    //        }
    //        else if (File.Exists(Key))
    //        {

    //            // Get the table header //
    //            TableHeader h = this.Buffer(Key);

    //            // Create the table //
    //            ScribeTable t;
    //            if (h.RootPageID == -1)
    //            {
    //                t = new HeapTable(this._Host, h);
    //            }
    //            else
    //            {
    //                t = new ClusteredTable(this._Host, h); // The ctor adds the table to the cache
    //            }

    //            // Check to see how many pages we can buffer //
    //            int MaxPages = (int)(this.FreeMemory / h.PageSize);
    //            int Pages = Math.Min(h.PageCount, MaxPages);

    //            // Buffer a block of pages //
    //            this.BufferBlock(h, 0, Pages);

    //            return t;

    //        }

    //        throw new ElementDoesNotExistException(Key);

    //    }

    //    public bool ScribeTableExists(string Key)
    //    {
    //        if (this._ScribeTables.ContainsKey(Key))
    //            return true;
    //        return File.Exists(Key);
    //    }

    //    public bool ScribePageExists(PageUID PID)
    //    {
    //        return this._ScribePages.ContainsKey(PID);
    //    }

    //    public void PushScribePage(string Key, Page Key, bool Write)
    //    {

    //        // Check if the element key doesnt exist //
    //        if (!this.ScribeTableExists(Key))
    //            throw new ElementDoesNotExistException(Key);

    //        // Check if we have space //
    //        if (Key.PageSize > this.FreeMemory)
    //        {
    //            this.ReleaseMemory(Key.PageSize);
    //        }

    //        // Build a PID //
    //        PageUID pid = new PageUID(Key, Key.PageID);
    //        this._BurnPile.EnqueueOrTag(pid);

    //        // If the page exists already //
    //        if (this.ScribePageExists(pid))
    //        {
    //            this._ScribePages[pid] = Key;
    //            this._ScribeWrites[pid]++;
    //            Key.Cached = true; // just in case...
    //        }
    //        // Otherwise, add the page //
    //        else
    //        {
    //            this._ScribePages.Add(pid, Key);
    //            this._ScribeWrites.Add(pid, Write ? 1 : 0);
    //            this._Memory += Key.PageID;
    //            Key.Cached = true;
    //        }


    //    }

    //    public void PushScribePage(string Key, Page Key)
    //    {
    //        this.PushScribePage(Key, Key, true);
    //    }

    //    public Page RequestScribePage(string Key, int PageID)
    //    {

    //        PageUID pid = new PageUID(Key, PageID);
    //        this._BurnPile.EnqueueOrTag(pid);

    //        if (!this.ScribeTableExists(Key))
    //            throw new ElementDoesNotExistException(Key);

    //        if (this.ScribePageExists(pid))
    //        {
    //            return this._ScribePages[pid];
    //        }
    //        else
    //        {
    //            Page p = this.Buffer(Key, PageID);
    //            this.PushScribePage(Key, p, false);
    //            return p;
    //        }

    //    }

    //    public void BurnScribePage(string Key, int PageID, bool Flush)
    //    {

    //        PageUID pid = new PageUID(Key, PageID);

    //        // See if the page exists //
    //        if (!this.ScribePageExists(pid))
    //            throw new Exception("Page does not exist");

    //        // Get the page //
    //        Page p = this._ScribePages[pid];
    //        int Writes = this._ScribeWrites[pid];

    //        // Remove from the cache //
    //        this._ScribePages.Remove(pid);
    //        this._ScribeWrites.Remove(pid);
    //        this._BurnPile.Remove(pid);

    //        // Remove from memory //
    //        this._Memory -= p.PageSize;

    //        // Set the page to non-cached //
    //        p.Cached = false;

    //        // Actually hit disk //
    //        if (Flush && Writes > 0)
    //            this.Flush(Key, p);

    //    }

    //    public void BurnScribeTable(string Key, bool Flush)
    //    {

    //        if (!this._ScribeTables.ContainsKey(Key))
    //            throw new ElementDoesNotExistException(Key);

    //        // Get the table //
    //        Table t = this._ScribeTables[Key];

    //        // Close it //
    //        t.PreSerialize();

    //        // Remove from memory //
    //        this._ScribeTables.Remove(Key);
    //        this._Memory -= TableHeader.SIZE;

    //        // Get all the pages //
    //        var Pages = this._ScribePages.Select((OriginalPage) => { return OriginalPage.Key.Key == Key; });

    //        // Burn every page //
    //        foreach (KeyValuePair<PageUID, Page> kv in this._ScribePages.ToList())
    //        {
    //            this.BurnScribePage(kv.Key.Key, kv.Key.PageID, Flush);
    //        }

    //        // Dump the table to disk //
    //        if (Flush)
    //        {
    //            this.Flush(Key, t.Header);
    //        }


    //    }

    //    public void ReleaseMemory(int MemoryNeeded)
    //    {

    //        while (this.FreeMemory < (long)MemoryNeeded)
    //        {

    //            if (this._ScribePages.Count == 0)
    //                throw new OutOfMemoryException("Cannot free enough memory");

    //            PageUID pid = this._BurnPile.Dequeue();

    //            this.BurnScribePage(pid.Key, pid.PageID, true);

    //        }

    //    }

    //    // Dream Tables //
    //    public void AddDreamTable(DreamTable Table)
    //    {

    //        if (this.DreamTableExists(Table.Key))
    //        {
    //            throw new ElementExistsException(Table.Key);
    //        }

    //        this._DreamTables.Add(Table.Key, Table);

    //    }

    //    public DreamTable RequestDreamTable(string Key)
    //    {
    //        if (!this.DreamTableExists(Key))
    //            throw new ElementDoesNotExistException(Key);
    //        return this._DreamTables[Key];
    //    }

    //    public bool DreamTableExists(string Key)
    //    {
    //        return this._DreamTables.ContainsKey(Key);
    //    }

    //    public bool DreamPageExists(PageUID PID)
    //    {
    //        return this._DreamPages.ContainsKey(PID);
    //    }

    //    public void PushDreamPage(string Key, Page Key)
    //    {

    //        // Check if the element key doesnt exist //
    //        if (!this.DreamTableExists(Key))
    //            throw new ElementDoesNotExistException(Key);

    //        // Build a PID //
    //        PageUID pid = new PageUID(Key, Key.PageID);

    //        // If the page exists already //
    //        if (this.DreamPageExists(pid))
    //        {
    //            this._DreamPages[pid] = Key;
    //            Key.Cached = true;
    //        }
    //        // Otherwise, add the page //
    //        else
    //        {
    //            this._DreamPages.Add(pid, Key);
    //            this._Memory += Key.PageID;
    //            Key.Cached = true;
    //        }

    //    }

    //    public Page RequestDreamPage(string Key, int PageID)
    //    {

    //        PageUID pid = new PageUID(Key, PageID);

    //        if (!this.DreamTableExists(Key))
    //            throw new ElementDoesNotExistException(Key);

    //        if (this.DreamPageExists(pid))
    //        {
    //            return this._DreamPages[pid];
    //        }

    //        throw new Exception("Page does not exist");

    //    }

    //    public void BurnDreamPage(string Key, int PageID)
    //    {

    //        PageUID pid = new PageUID(Key, PageID);

    //        // See if the page exists //
    //        if (!this.DreamPageExists(pid))
    //            throw new Exception("Page does not exist");

    //        // Get the page //
    //        Page p = this._DreamPages[pid];

    //        // Remove from the cache //
    //        this._DreamPages.Remove(pid);

    //        // Remove from memory //
    //        this._Memory -= p.PageSize;

    //        // Set the page to non-cached //
    //        p.Cached = false;

    //    }

    //    public void BurnDreamTable(string Key)
    //    {

    //        if (!this.DreamTableExists(Key))
    //            throw new ElementDoesNotExistException(Key);

    //        // Get the table //
    //        Table t = this._DreamTables[Key];

    //        // Remove from memory //
    //        this._DreamTables.Remove(Key);
    //        this._Memory -= TableHeader.SIZE;

    //        // Get all the pages //
    //        var Pages = this._DreamPages.Select((OriginalPage) => { return OriginalPage.Key.Key == Key; });

    //        // Burn every page //
    //        foreach (KeyValuePair<PageUID, Page> kv in this._DreamPages.ToList())
    //        {
    //            this.BurnDreamPage(kv.Key.Key, kv.Key.PageID);
    //        }

    //    }

    //    // Other methods //
    //    /// <summary>
    //    /// Run when the query system shuts down
    //    /// </summary>
    //    public void ShutDown()
    //    {

    //        // Handle temp objects first //
    //        foreach (string s in this._TempObjects)
    //            this.DropTable(s);

    //        // Dump the scribe tables //
    //        List<string> keys = this._ScribeTables.Keys.ToList();
    //        foreach (string s in keys)
    //            this.BurnScribeTable(s, true);

    //        // Dump the dream tables //
    //        keys = this._DreamTables.Keys.ToList();
    //        foreach (string s in keys)
    //            this.BurnDreamTable(s);

    //    }

    //    /// <summary>
    //    /// Adds a tempory object
    //    /// </summary>
    //    /// <param name="Name"></param>
    //    public void TagTempObject(string Name)
    //    {
    //        this._TempObjects.Add(Name);
    //    }

    //    // Table Drops //
    //    /// <summary>
    //    /// Removes dream tables from memory; removes scribe tables from memory and disk
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    public void DropTable(string Key)
    //    {

    //        // Don't throw an error if the table doesnt exist //
    //        if (!this.ScribeTableExists(Key))
    //            return;

    //        // Take care of the entry //
    //        if (this._ScribeTables.ContainsKey(Key))
    //            this.BurnScribeTable(Key, false);

    //        // Check if it's a temp object //
    //        if (this._TempObjects.Contains(Key))
    //            this._TempObjects.Remove(Key);

    //        // Take care of the file on disk //
    //        if (File.Exists(Key))
    //        {
    //            File.Delete(Key);
    //        }

    //    }

    //    // Disk methods //
    //    /// <summary>
    //    /// Buffers a page
    //    /// </summary>
    //    /// <param name="Path"></param>
    //    /// <param name="PageID"></param>
    //    /// <returns></returns>
    //    internal Page Buffer(string Path, int PageID)
    //    {

    //        // Get the header //
    //        TableHeader h = this._ScribeTables[Path].Header;

    //        // Get the location on disk of the page //
    //        long Location = PageAddress(PageID, h.PageSize);

    //        // Buffer the page //
    //        byte[] b = new byte[h.PageSize];
    //        using (FileStream OriginalPage = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.None))
    //        {

    //            // Go to the file offset //
    //            OriginalPage.Position = Location;

    //            // Buffer the bytes //
    //            OriginalPage.Read(b, 0, h.PageSize);

    //        }

    //        Page p = Page.Read(b, 0);

    //        return p;

    //    }

    //    /// <summary>
    //    /// Flushes a page to disk
    //    /// </summary>
    //    /// <param name="Path"></param>
    //    /// <param name="Key"></param>
    //    internal void Flush(string Path, Page Key)
    //    {

    //        // Get the disk location //
    //        long Position = this.PageAddress(Key.PageID, Key.PageSize);

    //        // Convert to a hash //
    //        byte[] b = new byte[Key.PageSize];
    //        Page.Write(b, 0, Key);

    //        // Hit the disk //
    //        using (FileStream OriginalPage = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
    //        {
    //            OriginalPage.Position = Position;
    //            OriginalPage.Write(b, 0, Key.PageSize);
    //        }


    //    }

    //    /// <summary>
    //    /// Reads the table header from disk, but does NOT allocate in the current heap
    //    /// </summary>
    //    /// <param name="Path"></param>
    //    /// <returns></returns>
    //    internal TableHeader Buffer(string Path)
    //    {

    //        byte[] buffer = new byte[TableHeader.SIZE];
    //        using (FileStream OriginalPage = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
    //        {
    //            OriginalPage.Read(buffer, 0, TableHeader.SIZE);
    //        }

    //        TableHeader h = TableHeader.FromHash(buffer, 0);

    //        return h;

    //    }

    //    /// <summary>
    //    /// Flushes a table header to disk
    //    /// </summary>
    //    /// <param name="Path"></param>
    //    /// <param name="Key"></param>
    //    internal void Flush(string Path, TableHeader Key)
    //    {

    //        // Convert to a hash //
    //        byte[] b = new byte[TableHeader.SIZE];
    //        TableHeader.ToHash(b, 0, Key);

    //        // Hit the disk //
    //        using (FileStream OriginalPage = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
    //        {
    //            OriginalPage.Write(b, 0, b.Length);
    //        }

    //    }

    //    /// <summary>
    //    /// Gets the on disk address of a given page
    //    /// </summary>
    //    /// <param name="PageID"></param>
    //    /// <param name="PageSize"></param>
    //    /// <returns></returns>
    //    internal long PageAddress(int PageID, int PageSize)
    //    {

    //        long HeaderOffset = TableHeader.SIZE;
    //        long pid = (long)PageID;
    //        long ps = (long)PageSize;
    //        return HeaderOffset + pid * ps;

    //    }

    //    /// <summary>
    //    /// Buffers a block of pages from disk
    //    /// </summary>
    //    /// <param name="Header"></param>
    //    /// <param name="PageOffset"></param>
    //    /// <param name="PageCount"></param>
    //    private void BufferBlock(TableHeader Header, int PageOffset, int PageCount)
    //    {

    //        long Offset = TableHeader.SIZE + (long)(PageOffset) * (long)Header.PageSize;
    //        long ByteCount = (long)PageCount * (long)Header.PageSize;
    //        if (ByteCount > (long)int.MaxValue)
    //            throw new IndexOutOfRangeException("Cannot read more than 2gb into memory at once");

    //        byte[] b = new byte[ByteCount];
    //        using (FileStream fs = File.Open(Header.Path, FileMode.Open, FileAccess.Read, FileShare.None))
    //        {
    //            fs.Position = Offset;
    //            fs.Read(b, 0, (int)ByteCount);
    //        }

    //        RecordMatcher matcher = new RecordMatcher(Header.ClusterKey);
    //        long Location = 0;
    //        for (int i = 0; i < PageCount; i++)
    //        {

    //            Page p = Page.Read(b, Location);
    //            if (p.PageType == Page.SORTED_PAGE_TYPE)
    //            {
    //                p = new SortedPage(p, matcher);
    //            }
    //            Location += Header.PageSize;
    //            this.PushScribePage(Header.Key, p, false);

    //        }

    //    }

    //    // Sub-Classes //
    //    /// <summary>
    //    /// An entry holds all pages for a given database object
    //    /// </summary>
    //    public class Entry
    //    {

    //        private Dictionary<int, Page> _Pages; // Key = page id, value = page
    //        private Dictionary<int, Tuple<int, int>> _Requests; // Value1 = read requests, value2 = write requests
    //        private Queue<int> _BurnQueue;
    //        private string _Key; // the name of the database object
    //        private int _PageSize; // the database object's page size
    //        private bool _IsDream = false;
    //        private Table _Parent;

    //        public Entry(Table Table)
    //        {
    //            this._Key = Table.Key;
    //            this._PageSize = Table.PageSize;
    //            this._Pages = new Dictionary<int, Page>();
    //            this._Requests = new Dictionary<int, Tuple<int, int>>();
    //            this._BurnQueue = new Queue<int>();
    //            this._IsDream = Table.Header.IsMemoryOnly;
    //            this._Parent = Table;
    //        }

    //        // Properties //
    //        public int Count
    //        {
    //            get { return this._Pages.Count; }
    //        }

    //        public int PageSize
    //        {
    //            get { return this._PageSize; }
    //        }

    //        public long MemoryUsage
    //        {
    //            get { return (long)(this._Pages.Count) * (long)(this._PageSize); }
    //        }

    //        public string Key
    //        {
    //            get { return this._Key; }
    //        }

    //        public bool IsDream
    //        {
    //            get { return this._IsDream; }
    //        }

    //        public Table Parent
    //        {
    //            get { return this._Parent; }
    //        }

    //        // Methods //
    //        public bool Exists(int PageID)
    //        {
    //            return this._Pages.ContainsKey(PageID);
    //        }

    //        public int ReadCount(int PageID)
    //        {
    //            return this._Requests[PageID].Item1;
    //        }

    //        public void IncrementReadCount(int PageID)
    //        {
    //            Tuple<int, int> OriginalPage = this._Requests[PageID];
    //            this._Requests[PageID] = new Tuple<int, int>(OriginalPage.Item1 + 1, OriginalPage.Item2);
    //        }

    //        public int WriteCount(int PageID)
    //        {
    //            return this._Requests[PageID].Item2;
    //        }

    //        public void IncrementWriteCount(int PageID)
    //        {
    //            Tuple<int, int> OriginalPage = this._Requests[PageID];
    //            this._Requests[PageID] = new Tuple<int, int>(OriginalPage.Item1, OriginalPage.Item2 + 1);
    //        }

    //        public void Push(Page Key, bool Write)
    //        {

    //            // Check to see if the page exists in memory already //
    //            if (this.Exists(Key.PageID))
    //            {
    //                this._Pages[Key.PageID] = Key;
    //                this.IncrementWriteCount(Key.PageID);
    //                return;
    //            }

    //            // Otherwise allocate //
    //            this._Pages.Add(Key.PageID, Key);
    //            this._Requests.Add(Key.PageID, new Tuple<int, int>(0, 0));
    //            this._BurnQueue.Enqueue(Key.PageID);
    //            if (Write) this.IncrementWriteCount(Key.PageID);

    //        }

    //        public void Push(Page Key)
    //        {
    //            this.Push(Key, false);
    //        }

    //        public Page Peek(int PageID)
    //        {

    //            this.IncrementReadCount(PageID);
    //            return this._Pages[PageID];

    //        }

    //        public void Burn(int PageID)
    //        {

    //            if (!this.Exists(PageID))
    //                return;

    //            this._Pages.Remove(PageID);
    //            this._Requests.Remove(PageID);

    //        }

    //        public Page Pop(int PageID)
    //        {
    //            Page p = this.Peek(PageID);
    //            this.Burn(PageID);
    //            return p;
    //        }

    //        public int SuggestBurnPage()
    //        {

    //            if (this.Count == 0)
    //                return -1;

    //            int id = this._BurnQueue.Dequeue();
    //            while (!this.Exists(id))
    //            {
    //                id = this._BurnQueue.Dequeue();
    //            }

    //            return id;
    //        }

    //        public string PageMap()
    //        {

    //            StringBuilder sb = new StringBuilder();
    //            foreach (Page p in this._Pages.Values)
    //            {
    //                sb.AppendLine(p.MapElement());
    //            }
    //            return sb.ToString();

    //        }

    //    }

    //    /// <summary>
    //    /// Thrown if a given page does not exist
    //    /// </summary>
    //    public class PageDoesNotExistException : Exception
    //    {

    //        public PageDoesNotExistException(string ObjectName, int PageID)
    //            : base(string.Format("Page {0} does not exist for '{1}'", PageID, ObjectName))
    //        {
    //        }

    //    }

    //    /// <summary>
    //    /// Throw when an object doesnt exist
    //    /// </summary>
    //    public class ElementDoesNotExistException : Exception
    //    {

    //        public ElementDoesNotExistException(string ObjectName)
    //            : base(string.Format("Object '{0}' does not exist", ObjectName))
    //        {

    //        }

    //    }

    //    /// <summary>
    //    /// Exception for duplicate elements
    //    /// </summary>
    //    public class ElementExistsException : Exception
    //    {

    //        public ElementExistsException(string ObjectName)
    //            : base(string.Format("Object '{0}' already exists", ObjectName))
    //        {

    //        }

    //    }

    //}

}
