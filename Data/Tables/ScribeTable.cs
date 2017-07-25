using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents the base class for all tables written to disk
    /// </summary>
    //public abstract class ScribeTable : Table
    //{

    //    /// <summary>
    //    /// This method should be used for creating a table object from an existing table on disk
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Header"></param>
    //    /// <param name="ClusterKey"></param>
    //    public ScribeTable(Host Host, TableHeader Header)
    //        : base(Host, Header)
    //    {
    //        this._Host.TableStore.AddScribeTable(this);
    //    }

    //    /// <summary>
    //    /// This method should be used for creating a brand new table object
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Alias"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="PageSize"></param>
    //    /// <param name="ClusterKey"></param>
    //    public ScribeTable(Host Host, string Name, string Dir, Schema Columns, int PageSize)
    //        : base(Host, new TableHeader(Name, Dir, TableHeader.V1_EXTENSION, 0, 0, -1, -1, PageSize, Columns))
    //    {
    //        this._Host.TableStore.DropTable(this.Key);
    //        this._Host.TableStore.AddScribeTable(this);
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
    //        return this._Host.TableStore.RequestScribePage(this.Key, PageID);
    //    }

    //    public override void SetPage(Page Key)
    //    {

    //        this._Host.TableStore.PushScribePage(this.Key, Key, true); // want to ensure our pages end up getting saved to disk

    //    }

    //    public override bool PageExists(int PageID)
    //    {
    //        return this._Host.TableStore.ScribePageExists(new PageUID(this.Key, PageID));
    //    }

    //}

}
