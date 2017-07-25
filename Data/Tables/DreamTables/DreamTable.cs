using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents an in memory only table
    /// </summary>
    //public abstract class DreamTable : Table
    //{

    //    /*
    //     * The user still needs to implement the following:
    //     * IsFull
    //     * InsertKey(Record)
    //     * Partiton(int, int)
    //     * 
    //     */

    //    public const int MAX_MEMORY = 1024 * 1024 * 16; // 16 mb

    //    protected int _PageCapacity;

    //    protected DreamTable(Host Host, string Name, Schema Columns, int PageSize, TableState State)
    //        : base(Host, TableHeader.DreamHeader(Name, Columns, PageSize))
    //    {

    //        // Save our working variables //
    //        this._PageCapacity = MAX_MEMORY / PageSize;
    //        this._State = State;

    //        this._Host.TableStore.AddDreamTable(this);

    //    }

    //    public abstract bool IsFull { get; }

    //    public override Record Select(RecordKey Position)
    //    {

    //        if (this.State == TableState.WriteOnly || this.State == TableState.FullLock)
    //            throw new Exception("Table is locked for reading");

    //        return this.GetBranchPage(Position.PAGE_ID).Select(Position.ROW_ID);

    //    }

    //    public override void Insert(IEnumerable<Record> Records)
    //    {

    //        if (this.State == TableState.ReadOnly || this.State == TableState.FullLock)
    //            throw new Exception("Table is locked for writing");

    //        foreach (Record r in Records)
    //        {
    //            this.Insert(r);
    //        }

    //    }

    //    public override Page GenerateNewPage()
    //    {

    //        // Create the new page //
    //        Page p = new Page(this.PageSize, this.GenerateNewPageID, this.TerminalPageID, -1, this.Columns);

    //        // Get the last page and switch it's next page id //
    //        Page q = this.TerminalPage;
    //        q.NextPageID = p.PageID;

    //        // Add this page //
    //        this.SetPage(p);

    //        return p;

    //    }

    //    public override Page GetBranchPage(int PageID)
    //    {
    //        return this._Host.TableStore.RequestDreamPage(this.Key, PageID);
    //    }

    //    public override void SetPage(Page Key)
    //    {
    //        this._Host.TableStore.PushDreamPage(this.Key, Key);
    //    }

    //    public override bool PageExists(int PageID)
    //    {
    //        return this._Host.TableStore.DreamPageExists(new PageUID(this.Key, PageID));
    //    }

    //}

}
