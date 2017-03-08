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
    /// Represents an envrioment to that core pulse features run off of
    /// </summary>
    public sealed class Host
    {

        /// <summary>
        /// The name of the in memory schema
        /// </summary>
        public const string GLOBAL = "PSY";

        /// <summary>
        /// The name of the disk based temp schema
        /// </summary>
        public const string TEMP = "TEMP";
        
        private const string DIR_MAIN = "Pulse_Projects";
        private const string DIR_TEMP = "Temp";
        private const string DIR_TEST = "Test";
        private const string DIR_LOG = "Log";

        private PageManager _PageCache;
        private Communicator _IO;
        private Heap<Cell> _Scalars;
        private Heap<CellMatrix> _Matrixes;
        private Heap<string> _Connections;
        private RandomCell _RNG;

        public Host()
        {

            Host.CheckDir();

            this._PageCache = new PageManager(this);
            this._IO = new CommandLineCommunicator();

            this._Scalars = new Heap<Cell>();
            this._Matrixes = new Heap<CellMatrix>();
            this._Connections = new Heap<string>();
            this._Connections.Allocate(TEMP, TempDir);

            this._RNG = new RandomCell();
            

        }

        public void ShutDown()
        {
            this.PageCache.ShutDown();
        }

        public PageManager PageCache
        {
            get { return this._PageCache; }
        }

        public Communicator IO
        {
            get { return this._IO; }
        }

        public Heap<Cell> Scalars
        {
            get { return this._Scalars; }
        }

        public Heap<CellMatrix> Matrixes
        {
            get { return this._Matrixes; }
        }

        // Random Number Generator //
        public RandomCell BaseRNG
        {
            get { return this._RNG; }
        }

        public void SetSeed(int Seed)
        {
            this._RNG = new RandomCell(Seed);
        }

        // Connection Support //
        public Heap<string> Connections
        {
            get { return this._Connections; }
        }

        public string TempDB
        {
            get { return this._Connections[TEMP]; }
        }

        public void AddConnection(string Alias, string Connection)
        {
            this._Connections.Allocate(Alias, Connection);
        }

        // Table Support //
        public Table OpenTable(string Key)
        {

            if (this._PageCache.ScribeTableExists(Key))
                return this._PageCache.RequestScribeTable(Key);
            else if (this._PageCache.DreamTableExists(Key))
                return this._PageCache.RequestDreamTable(Key);

            throw new Exception(string.Format("Table does not exist '{0}'", Key));

        }

        public Table OpenTable(string Alias, string Name)
        {

            if (StringComparer.OrdinalIgnoreCase.Compare(Alias, GLOBAL) == 0)
                return this.OpenTable(Name);

            if (this.Connections.Exists(Alias))
                return this.OpenTable(TableHeader.DeriveV1Path(this.Connections[Alias], Name));

            throw new Exception(string.Format("Table does not exist '{0}.{1}'", Alias, Name));

        }

        public ClusteredScribeTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns, ClusterState State, int PageSize)
        {
            ClusteredScribeTable t = new ClusteredScribeTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, State, PageSize);
            return t;
        }

        public ClusteredScribeTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns, ClusterState State)
        {
            ClusteredScribeTable t = new ClusteredScribeTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, State, Page.DEFAULT_SIZE);
            return t;
        }

        public ClusteredScribeTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns)
        {
            ClusteredScribeTable t = new ClusteredScribeTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, ClusterState.Universal, Page.DEFAULT_SIZE);
            return t;
        }

        public HeapScribeTable CreateTable(string Alias, string Name, Schema Columns)
        {
            return new HeapScribeTable(this, Name, this._Connections[Alias], Columns, Page.DEFAULT_SIZE);
        }

        public DictionaryScribeTable CreateTable(string Alias, string Name, Schema KeyColumns, Schema ValueColumns)
        {
            return new DictionaryScribeTable(this, Name, this._Connections[Alias], KeyColumns, ValueColumns, Page.DEFAULT_SIZE);
        }

        public HeapDreamTable Dream(string Name, Schema Columns)
        {
            return new HeapDreamTable(this, Name, Columns);
        }

        public ClusteredDreamTable Dream(string Name, Schema Columns, Key IndexColumns)
        {
            return new ClusteredDreamTable(this, Name, Columns, IndexColumns);
        }

        public DictionaryDreamTable Dream(string Name, Schema KeyColumns, Schema ValueColumns)
        {
            return new DictionaryDreamTable(this, Name, KeyColumns, ValueColumns, Page.DEFAULT_SIZE);
        }

        // Directories //
        public static string MainDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + DIR_MAIN + "\\"; }
        }

        public static string TempDir
        {
            get { return MainDir + DIR_TEMP + "\\"; }
        }

        public static string LogDir
        {
            get { return MainDir + DIR_LOG + "\\"; }
        }

        public static string TestDir
        {
            get { return MainDir + DIR_TEST + "\\"; }
        }

        public static void CheckDir()
        {

            if (!Directory.Exists(MainDir)) 
                Directory.CreateDirectory(MainDir);
            if (!Directory.Exists(TempDir)) 
                Directory.CreateDirectory(TempDir);
            if (!Directory.Exists(LogDir)) 
                Directory.CreateDirectory(LogDir);
            if (!Directory.Exists(TestDir)) 
                Directory.CreateDirectory(TestDir);

        }

        public static string RandomName
        {
            get
            {
                Guid x = Guid.NewGuid();
                return "TEMP_" + x.ToString().Replace("-", "");
            }
        }

    }

}
