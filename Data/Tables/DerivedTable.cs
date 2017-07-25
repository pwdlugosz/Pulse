using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    //public abstract class DerivedTable : Table
    //{

    //    /*
    //     * The following are turned off and will throw an exception:
    //     *      AddIndex
    //     *      GetIndex
    //     *      Partition
    //     *      
    //     * The following are taken from the parent table:
    //     *      GeneratePageID
    //     *      
    //     * The following must be overwritten:
    //     *      GenerateNewPage
    //     *      InsertKey
    //     *      
    //     */

    //    protected Table _Parent;

    //    public DerivedTable(Host Host, TableHeader DerivedHeader, Table Parent)
    //        : base(Host, DerivedHeader)
    //    {
    //        this._Parent = Parent;
    //    }

    //    public override int GenerateNewPageID
    //    {
    //        get
    //        {
    //            return this._Parent.GenerateNewPageID;
    //        }
    //    }

    //    public override void SetPage(Page Key)
    //    {
    //        this._Parent.SetPage(Key);
    //    }

    //    public override Page GetBranchPage(int PageID)
    //    {
    //        return this._Parent.GetBranchPage(PageID);
    //    }

    //    public override void CreateIndex(string Name, Key IndexColumns)
    //    {
    //        throw new InvalidOperationException("Indexes cannot exist on derived tables");
    //    }

    //    public override Index GetIndex(string Name)
    //    {
    //        throw new InvalidOperationException("Indexes cannot exist on derived tables");
    //    }

    //    public override Table Partition(int PartitionIndex, int PartitionCount)
    //    {
    //        throw new InvalidOperationException("Cannot partition derived tables");
    //    }

    //}

}
