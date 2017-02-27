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
        protected BPlusTree _Tree;

        /// <summary>
        /// Represents the table where the index will be stored
        /// </summary>
        protected Table _Storage;

        /// <summary>
        /// Represents the table that the index is
        /// </summary>
        protected Table _Parent;

        /// <summary>
        /// 
        /// </summary>
        protected Key _IndexColumns;

        /// <summary>
        /// 
        /// </summary>
        protected IndexHeader _Header;

        /// <summary>
        /// Used only by inherited classes
        /// </summary>
        protected Index()
        {
        }

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
            this._Tree = new BPlusTree(Storage, s, Key.Build(this._IndexColumns.Count), root, Header, false);

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
            this._Tree = new BPlusTree(this._Storage, s, this._IndexColumns, null, this._Header, false);

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
        public virtual void Insert(Record Element, RecordKey Key)
        {
            Record x = Index.GetIndexElement(Element, Key, this._IndexColumns);
            this._Tree.Insert(x);
        }

        public virtual ReadStream OpenReader()
        {
            return new IndexDataReadStream(this._Header, this._Storage, this._Parent);
        }

        public virtual ReadStream OpenReader(Record Key)
        {
            RecordKey l = this._Tree.SeekFirst(Key, false);
            RecordKey u = this._Tree.SeekLast(Key, false);
            return new IndexDataReadStream(this._Header, this._Storage, this._Parent, l, u);
        }

        public virtual ReadStream OpenReader(Record LKey, Record UKey)
        {
            RecordKey l = this._Tree.SeekFirst(LKey, false);
            RecordKey u = this._Tree.SeekLast(UKey, false);
            return new IndexDataReadStream(this._Header, this._Storage, this._Parent, l, u);
        }

        public virtual void Calibrate()
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
            ShellScribeTable storage = new ShellScribeTable(Parent.Host, Host.RandomName, Parent.Host.TempDB, columns, Page.DEFAULT_SIZE);
            return new Index(storage, Parent, Host.RandomName, IndexColumns);

        }

    }

}
