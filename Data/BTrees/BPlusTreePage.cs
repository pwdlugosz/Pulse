﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents a single page in a b+ tree
    /// </summary>
    public class BPlusTreePage : Page
    {

        private int DEBUG_MAX_RECORDS = -1; // used only for debugging; set to -1 to revert to the classic logic

        public const int XPAGE_TYPE = 9;

        public enum BPlusTreeSearchType : int
        {
            FirstElement = -1,
            LastElement = 1,
            AnyElement = 0
        }

        // This overrides:
        // _X0 = parent page ID
        // _X1 = (1 == is leaf, 0 == is branch)
        // _X2 = is highest

        private RecordMatcher _StrongMatcher; // Matches all key columns + page id for the branch nodes
        private RecordMatcher _WeakMatcher; // Only matches key columns;
        private RecordMatcher _PageSearchMatcher;
        private Key _StrongKeyColumns;
        private Key _WeakKeyColumns;
        private Key _OriginalKeyColumns; // Used only for generating
        private int _RefColumn = 0;

        public BPlusTreePage(int PageSize, int PageID, int LastPageID, int NextPageID, int FieldCount, int DataDiskCost, Key KeyColumns, bool IsLeaf)
            : base(PageSize, PageID, LastPageID, NextPageID, FieldCount, DataDiskCost)
        {

            this.IsLeaf = IsLeaf;
            this._OriginalKeyColumns = KeyColumns;
            this._StrongKeyColumns = IsLeaf ? KeyColumns : BranchObjectiveClone(KeyColumns, false);
            if (this.IsLeaf)
            {
                this._StrongMatcher = new RecordMatcher(KeyColumns); // Designed to match keys to keys or elements to elements
                this._WeakMatcher = new RecordMatcher(KeyColumns); // Designed to match keys to keys or elements to elements
                this._PageSearchMatcher = null; // not used
                this._StrongKeyColumns = KeyColumns;
                this._WeakKeyColumns = KeyColumns;
            }
            else
            {
                this._StrongMatcher = new RecordMatcher(BranchObjectiveClone(KeyColumns, false)); // Designed to match keys to keys
                this._WeakMatcher = new RecordMatcher(BranchObjectiveClone(KeyColumns, true)); // Designed to match keys and keys
                this._PageSearchMatcher = new RecordMatcher(BranchObjectiveClone(KeyColumns, true), KeyColumns);
                this._StrongKeyColumns = BranchObjectiveClone(KeyColumns, false);
                this._WeakKeyColumns = BranchObjectiveClone(KeyColumns, true);
            }
            this._RefColumn = KeyColumns.Count;

        }

        // Overrides //
        public override bool IsFull
        {
            get
            {
                if (DEBUG_MAX_RECORDS == -1)
                    return base.IsFull;
                else
                    return this.Count >= DEBUG_MAX_RECORDS;
            }
        }

        public override void Insert(Record Element)
        {

            int idx = this._Elements.BinarySearch(Element, this._StrongMatcher);
            if (idx < 0) idx = ~idx;

            if (idx == this.Count && !this.IsHighest)
                throw new Exception("Cannot add a higher record to this page");
            this._Elements.Insert(idx, Element);

        }

        public override int Search(Record Element)
        {
            return this._Elements.BinarySearch(Element, this._StrongMatcher);
        }

        public override int PageType
        {
            get
            {
                return XPAGE_TYPE;
            }
        }

        // Join Leaf / Branch Methods //
        public int ParentPageID
        {
            get { return this._X0; }
            set { this._X0 = value; }
        }

        public bool IsLeaf
        {
            get { return this._X1 == 1; }
            set { this._X1 = (value ? 1 : 0); }
        }

        public bool IsHighest
        {
            get { return this._X2 == 1; }
            set { this._X2 = (value ? 1 : 0); }
        }

        public Key StrongKeyColumns
        {
            get { return this._StrongKeyColumns; }
        }

        public Key WeakKeyColumns
        {
            get { return this._WeakKeyColumns; }
        }

        public Key OriginalKeyColumns
        {
            get { return this._OriginalKeyColumns; }
        }

        public List<Record> SelectAll(Record Element)
        {

            int Lower = this.SearchLeaf(Record.Split(Element, this._WeakKeyColumns), BPlusTreeSearchType.FirstElement, true);
            int Upper = this.SearchLeaf(Record.Split(Element, this._WeakKeyColumns), BPlusTreeSearchType.LastElement, true);

            List<Record> elements = new List<Record>();
            if (Lower < 0 || Upper < 0)
                return elements;

            elements.AddRange(this._Elements.GetRange(Lower, Upper - Lower));
            return elements;

        }

        public BPlusTreePage GenerateXPage(int PageID, int LastPageID, int NextPageID)
        {
            BPlusTreePage x = new BPlusTreePage(this.PageSize, PageID, LastPageID, NextPageID, this._FieldCount, this._DataDiskCost, this._OriginalKeyColumns, this.IsLeaf);
            x.IsLeaf = this.IsLeaf;
            return x;
        }

        public BPlusTreePage SplitXPage(int PageID, int LastPageID, int NextPageID, int Pivot)
        {

            if (this.Count < 2)
                throw new IndexOutOfRangeException("Cannot split a page with fewer than 2 records");
            if (Pivot == 0 || Pivot == this.Count - 1)
                throw new IndexOutOfRangeException("Cannot split on the first or last record");
            if (Pivot < 0)
                throw new IndexOutOfRangeException(string.Format("Pivot ({0}) must be greater than 0", Pivot));
            if (Pivot >= this.Count)
                throw new IndexOutOfRangeException(string.Format("The pivot ({0}) cannot be greater than the element count ({1})", Pivot, this.Count));

            BPlusTreePage p = this.GenerateXPage(PageID, LastPageID, NextPageID);
            for (int i = Pivot; i < this.Count; i++)
            {
                p._Elements.Add(this._Elements[i]);
            }
            this._Elements.RemoveRange(Pivot, this.Count - Pivot);

            // Set the leafness and the parent page id //
            p.IsLeaf = this.IsLeaf;
            p.ParentPageID = this.ParentPageID;

            return p;

        }

        // Branch only methods //
        /// <summary>
        /// Inserts a key into the page
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="PageID"></param>
        public void InsertKey(Record Key, int PageID)
        {

            if (this._WeakMatcher.Compare(Key, this._Elements.Last()) > 0 && !this.IsHighest)
                throw new Exception("Can't insert a record greater the max record unless this is the highest page");

            // InsertKey as usual //
            this.InsertKeyUnsafe(Key, PageID);

        }

        /// <summary>
        /// Inserts a key into the table without checking if it is within the bounds of the tree
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="PageID"></param>
        public void InsertKeyUnsafe(Record Key, int PageID)
        {

            // Find the insertion point //
            Record k = Composite(Key, PageID);
            int idx = this._Elements.BinarySearch(k, this._StrongMatcher);
            if (idx < 0) idx = ~idx;

            // InsertKey as usual //
            this._Elements.Insert(idx, k);

        }

        /// <summary>
        /// Given an element, this finds the page ID of the page it belongs on
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public int GetPageID(Record Key)
        {

            if (this.IsLeaf)
                throw new Exception("Cannot page search a leaf");

            int idx = this._Elements.BinarySearch(Key, this._PageSearchMatcher);
            if (idx < 0)
                idx = ~idx;

            if (idx != this._Elements.Count)
            {
                return this._Elements[idx][this._RefColumn].INT_A;
            }
            else
            {
                throw new Exception();
            }


        }

        /// <summary>
        /// Gets the page ID given an index
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public int GetPageID(int Index)
        {
            return this._Elements[Index][this._RefColumn].INT_A;
        }

        /// <summary>
        /// Gets all the page IDs
        /// </summary>
        /// <returns></returns>
        public List<int> AllPageIDs()
        {

            List<int> ids = new List<int>();
            foreach (Record r in this._Elements)
            {
                int i = r[this._RefColumn].INT_A;
                ids.Add(i);
            }

            return ids;

        }

        /// <summary>
        /// Checks if a Key/PageID exists
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="PageID"></param>
        /// <returns></returns>
        public bool KeyExists(Record Key, int PageID)
        {
            return this._Elements.BinarySearch(Composite(Key, PageID), this._StrongMatcher) >= 0;
        }

        /// <summary>
        /// Checks if a key exists
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool KeyExists(Record Key)
        {
            return this._Elements.BinarySearch(Key, this._WeakMatcher) >= 0;
        }

        public void Delete(Record Element)
        {

            // Key must be the entire data record //
            int idx = this._Elements.BinarySearch(Element, this._StrongMatcher);
            if (idx < 0)
            {
                throw new IndexOutOfRangeException("Key is not in this page");
            }

            this._Elements.RemoveAt(idx);

        }

        public bool LessThanTerminal(Record Key)
        {

            // We want this to be strictly less than the last element, not the case where it may be equal to
            return this._WeakMatcher.Compare(Key, this._Elements.Last()) < 0;

        }

        public Record TerminalKeyOnly
        {
            get { return Record.Split(this._Elements.Last(), this._WeakKeyColumns); }
        }

        // Searches //
        /// <summary>
        /// Finds a page ID given a key
        /// </summary>
        /// <param name="Key">The value to find</param>
        /// <param name="SearchType">The method of search</param>
        /// <returns>A page ID; if the value doesnt exist</returns>
        public int SearchBranch(Record Key, BPlusTreeSearchType SearchType, bool Exact)
        {

            int idx = this._Elements.BinarySearch(Key, this._WeakMatcher);

            // If we didnt find the element, we dont have to search for multiple keys
            if (idx < 0)
            {
                if (!Exact) idx = ~idx;
                return idx;
            }

            // If we really don't care about anything, then return the index //
            if (SearchType == BPlusTreeSearchType.AnyElement)
                return idx;

            // Search Lower //
            int pos = 0;
            if (SearchType == BPlusTreeSearchType.FirstElement)
            {

                while (true)
                {
                    pos = this._Elements.BinarySearch(0, idx, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }
            else
            {

                pos = idx;
                while (true)
                {
                    pos = this._Elements.BinarySearch(pos + 1, this.Count - idx - 1, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }

            return this.GetPageID(idx);

        }

        /// <summary>
        /// Finds a record position in a page
        /// </summary>
        /// <param name="Key">The key to search for</param>
        /// <param name="SearchType">The method of search</param>
        /// <returns>The index of the record on this page</returns>
        public int SearchLeaf(Record Key, BPlusTreeSearchType SearchType, bool Exact)
        {

            int idx = this._Elements.BinarySearch(Key, this._WeakMatcher);

            // If we didnt find the element, we dont have to search for multiple keys
            if (idx < 0)
            {
                if (!Exact) idx = ~idx;
                return idx;
            }

            // If we really don't care about anything, then return the index //
            if (SearchType == BPlusTreeSearchType.AnyElement)
                return idx;

            // Search Lower //
            int pos = 0;
            if (SearchType == BPlusTreeSearchType.FirstElement)
            {

                while (true)
                {
                    pos = this._Elements.BinarySearch(0, idx, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }
            else
            {

                pos = idx;
                while (true)
                {
                    pos = this._Elements.BinarySearch(pos + 1, this.Count - idx - 1, Key, this._WeakMatcher);
                    if (pos < 0) break;
                    idx = pos;
                }

            }

            return idx;

        }

        // Debugs //
        internal string DebugString(int Index, bool NonCluster)
        {

            if (this.IsLeaf)
            {
                if (!NonCluster)
                    this._Elements[Index].ToString(',');
                Cell c = this._Elements[Index]._data.Last();
                return string.Format("{0} <{1},{2}>", this._Elements[Index].ToString(Key.Build(this.FieldCount - 2), ','), c.INT_A, c.INT_B);
            }

            Cell y = this._Elements[Index][this._RefColumn];
            return string.Format("Key [{0}] <{1},{2}>", this._Elements[Index].ToString(this._WeakKeyColumns), y.INT_A, y.INT_B);

        }

        // Statics //
        public static BPlusTreePage Mutate(Page Primitive, Key KeyColumns)
        {

            if (Primitive is BPlusTreePage)
                return Primitive as BPlusTreePage;

            BPlusTreePage x = new BPlusTreePage(Primitive.PageSize, Primitive.PageID, Primitive.LastPageID, Primitive.NextPageID, Primitive.FieldCount, Primitive.DataDiskCost, KeyColumns, Primitive.X1 == 1);
            x._X0 = Primitive.X0;
            x._X1 = Primitive.X1;
            x._X2 = Primitive.X2;
            x._X3 = Primitive.X3;
            x._Elements = Primitive.Cache;

            return x;

        }

        public static Key BranchObjectiveClone(Key KeyColumns, bool Weak)
        {

            Key k = new Key();
            for (int i = 0; i < KeyColumns.Count; i++)
            {
                k.Add(i, KeyColumns.Affinity(i));
            }
            if (!Weak)
                k.Add(k.Count, KeyAffinity.Ascending);
            return k;

        }

        public static Record Composite(Record Key, int PageID)
        {
            Cell[] c = new Cell[Key.Count + 1];
            Array.Copy(Key.BaseArray, 0, c, 0, Key.Count);
            c[c.Length - 1] = new Cell(PageID, 0);
            return new Record(c);
        }

    }


}