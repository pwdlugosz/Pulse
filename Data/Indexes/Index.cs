using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Data
{

    /// <summary>
    /// Represents a non-clustered index using a b+tree
    /// </summary>
    public class Index
    {

        /// <summary>
        /// Represents the base b+tree object
        /// </summary>
        private BPlusTree _Tree;

        /// <summary>
        /// Represents the table where the index will be stored
        /// </summary>
        private Table _Storage;

        /// <summary>
        /// Represents the table that the index is
        /// </summary>
        private Table _Parent;

        /// <summary>
        /// 
        /// </summary>
        private Key _IndexColumns;

        /// <summary>
        /// 
        /// </summary>
        private IndexHeader _Header;

        /// <summary>
        /// Opens an existing index
        /// </summary>
        /// <param name="Storage"></param>
        /// <param name="Parent"></param>
        /// <param name="Header"></param>
        public Index(Table Storage, Table Parent, IndexHeader Header)
        {

            this._Storage = Storage;
            this._Parent = Parent;
            this._Header = Header;
            this._IndexColumns = Header.IndexColumns;
            BPlusTreePage root = BPlusTreePage.Mutate(this._Storage.GetPage(Header.RootPageID), Header.IndexColumns);
            Schema s = BPlusTree.NonClusteredIndexColumns(this._Parent.Columns, Header.IndexColumns);
            this._Tree = new BPlusTree(Storage, s, Key.Build(this._IndexColumns.Count), root, Header);

        }

        /// <summary>
        /// Creates a new index
        /// </summary>
        /// <param name="Storage"></param>
        /// <param name="Parent"></param>
        /// <param name="IndexColumns"></param>
        public Index(Table Storage, Table Parent, string Name, Key IndexColumns)
        {

            this._Header = new IndexHeader(Name, -1, -1, -1, 0, 0, IndexColumns);
            this._Storage = Storage;
            this._Parent = Parent;
            this._IndexColumns = IndexColumns;
            Schema s = BPlusTree.NonClusteredIndexColumns(this._Parent.Columns, this._IndexColumns);
            this._Tree = new BPlusTree(this._Storage, s, this._IndexColumns, null, this._Header);

        }

        // Properties //
        public Table Storage
        {
            get { return this._Storage; }
        }

        public Table Parent
        {
            get { return this._Parent; }
        }

        public Key IndexColumns
        {
            get { return this._IndexColumns; }
        }

        public IndexHeader Header
        {
            get { return this._Header; }
        }

        public BPlusTree Tree
        {
            get { return this._Tree; }
        }

        // Methods //
        public void Insert(Record Element, RecordKey Key)
        {
            Record x = Index.GetIndexElement(Element, Key, this._IndexColumns);
            this._Tree.Insert(x);
        }

        public ReadStream OpenReader()
        {
            return new IndexDataReadStream(this._Header, this._Storage, this._Parent);
        }

        public ReadStream OpenReader(Record Key)
        {
            RecordKey l = this._Tree.SeekFirst(Key);
            RecordKey u = this._Tree.SeekLast(Key);
            return new IndexDataReadStream(this._Header, this._Storage, this._Parent, l, u);
        }

        public ReadStream OpenReader(Record LKey, Record UKey)
        {
            RecordKey l = this._Tree.SeekFirst(LKey);
            RecordKey u = this._Tree.SeekLast(UKey);
            return new IndexDataReadStream(this._Header, this._Storage, this._Parent, l, u);
        }

        public void Calibrate()
        {

            if (this._Header.RecordCount != 0 || this._Parent.RecordCount == 0)
                return;

            ReadStream stream = this._Parent.OpenReader();
            while (stream.CanAdvance)
            {

                RecordKey rk = stream.PositionKey;
                Record rec = stream.ReadNext();

                this.Insert(rec, rk);

            }

        }

        // Statics //
        public static Record GetIndexElement(Record Element, RecordKey Pointer, Key IndexColumns)
        {

            Cell[] c = new Cell[IndexColumns.Count + 1];
            for (int i = 0; i < IndexColumns.Count; i++)
            {
                c[i] = Element[IndexColumns[i]];
            }
            c[c.Length - 1] = Pointer.Element;
            return new Record(c);

        }

        public static Index CreateExternalIndex(Table Parent, Key IndexColumns)
        {

            Schema columns = BPlusTree.NonClusteredIndexColumns(Parent.Columns, IndexColumns);
            ScribeShellTable storage = new ScribeShellTable(Parent.Host, Host.RandomName, Parent.Host.TempDB, columns, Page.DEFAULT_SIZE);
            return new Index(storage, Parent, Host.RandomName, IndexColumns);

        }

    }

}
