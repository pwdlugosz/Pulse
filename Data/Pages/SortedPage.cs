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
    /// Represents a page that stores data in a sequental, sorted, order
    /// </summary>
    public class SortedPage : Page
    {

        public const int ELEMENT_NOT_FOUND = -1;

        private IRecordMatcher _Matcher;

        public SortedPage(int PageSize, int PageID, int LastPageID, int NextPageID, int FieldCount, int DataDiskCost, IRecordMatcher Matcher)
            : base(PageSize, PageID, LastPageID, NextPageID, FieldCount, DataDiskCost)
        {
            this._Matcher = Matcher;
        }

        public SortedPage(int PageSize, int PageID, int LastPageID, int NextPageID, Schema Columns, IRecordMatcher Matcher)
            : base(PageSize, PageID, LastPageID, NextPageID, Columns)
        {
            this._Matcher = Matcher;
        }

        public SortedPage(Page Primitive, IRecordMatcher Matcher)
            : this(Primitive.PageSize, Primitive.PageID, Primitive.LastPageID, Primitive.NextPageID, Primitive.FieldCount, Primitive.DataDiskCost, Matcher)
        {
            this._Elements = Primitive.Cache;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int PageType
        {
            get { return Page.SORTED_PAGE_TYPE; }
        }

        /// <summary>
        /// Inserts a given record at it's correct position in the page
        /// </summary>
        /// <param name="Key"></param>
        public override void Insert(Record Element)
        {

            int IndexOf = this.Search(Element, false);
            base.Insert(Element, IndexOf);

        }

        /// <summary>
        /// Turn off inserting at a given point
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="RowID"></param>
        public override void Insert(Record Element, int RowID)
        {
            throw new InvalidDataException("Cannot insert a record at specific point in a sorted datapage");
        }

        /// <summary>
        /// Turn off sorting
        /// </summary>
        /// <param name="ClusterKey"></param>
        public override void Sort(IRecordMatcher SortKey)
        {
            throw new ArgumentException("Cannot sort a sorted page; it's already sorted and the key cannot be changed");
        }

        /// <summary>
        /// Searches for an element; will pass back the location of the element closest to but exceeding the desired element if the element is not found
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public override int Search(Record Element)
        {
            return this.Search(Element, false);
        }

        /// <summary>
        /// Searches for a given record on the page
        /// </summary>
        /// <param name="Key">The record we're searching for</param>
        /// <param name="Exact">If true and the record isn't found, return ELEMENT_NOT_FOUND, otherwise it returns the insertion point</param>
        /// <returns>The index of the record on the page</returns>
        public int Search(Record Element, bool Exact)
        {

            // 0 records //
            if (this.Count == 0)
                return (Exact ? ELEMENT_NOT_FOUND : 0);

            // Set the radix //
            int i = 0;
            int Radix = this.Count - 1;

            // Seek!!! //
            while (i <= Radix)
            {

                int Index = i + (Radix - i >> 1);
                int Seek = this._Matcher.Compare(this._Elements[Index], Element);
                if (Seek == 0)
                {
                    return Index;
                }
                else if (Seek < 0)
                {
                    i = Index + 1;
                }
                else
                {
                    Radix = Index - 1;
                }

            }

            return (Exact ? ELEMENT_NOT_FOUND : i);

        }

        /// <summary>
        /// Searches for the given element, returning ELEMENT_NOT_FOUND if it doesnt exist
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public int SearchExact(Record Element)
        {
            return this.Search(Element, true);
        }

        /// <summary>
        /// Searches for the exact start of end location
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Lower"></param>
        /// <returns></returns>
        public int SearchPrecise(Record Element, bool Lower)
        {

            // Find the index and return if null or at the edges of the page //
            int idx = this.Search(Element, true);
            if (idx == NULL_INDEX)
                return idx;
            if (idx == 0 && Lower)
                return idx;
            if (idx == this.Count - 1 && !Lower)
                return idx;

            int step = (Lower ? -1 : 1);
            idx += step;
            while (idx >= 0 && idx < this.Count)
            {

                if (this._Matcher.Compare(Element, this._Elements[idx]) != 0)
                    return idx - step;
                idx += step;

            }

            return idx - step;

        }

        /// <summary>
        /// Checks if this record belongs in this domain
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool InDomain(Record Element)
        {

            if (this.Count == 0)
                return false;

            return this._Matcher.Between(Element, this.OriginRecord, this.TerminalRecord) == 0;

        }

        /// <summary>
        /// Generates a sorted data page; overwrites the base method which returns just a vanilla page
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NextPageID"></param>
        /// <returns></returns>
        public override Page Generate(int PageID, int LastPageID, int NextPageID)
        {
            return new SortedPage(this.PageSize, PageID, LastPageID, NextPageID, this._FieldCount, this._DataDiskCost, this._Matcher);
        }

    }

}
