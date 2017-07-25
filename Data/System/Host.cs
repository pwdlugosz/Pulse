using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Pulse.Libraries;

namespace Pulse.Data
{

    /// <summary>
    /// Represents an envrioment to that core pulse features run off of
    /// </summary>
    public sealed class Host
    {

        public const int DEBUG_STATE = 1;
        private StreamWriter _DebugWriter;

        public const string HOST_NAME = "PULSE";
        public const string HOST_VERSION = "0.3.0";

        /// <summary>
        /// The name of the disk based temp schema
        /// </summary>
        public const string TEMP = "TEMP";
        public const string GLOBAL = "GLOBAL";
        
        private const string DIR_MAIN = "Pulse_Projects";
        private const string DIR_TEMP = "Temp";
        private const string DIR_TEST = "Test";
        private const string DIR_LOG = "Log";

        //private PageManager _PageCache;
        private TableStore _Cache;
        private Communicator _IO;
        private Heap<Cell> _Scalars;
        private Heap<CellMatrix> _Matrixes;
        private Heap<string> _Connections;
        private RandomCell _RNG;
        private Stopwatch _Timer;
        private Heap<Library> _Libraries;
        private Library _Base;

        /// <summary>
        /// Creates a host
        /// </summary>
        public Host()
        {

            Host.CheckDir();

            this._Cache = new TableStore(this, TableStore.DEFAULT_MAX_MEMORY);
            this._IO = new CommandLineCommunicator();

            this._Scalars = new Heap<Cell>();
            this._Matrixes = new Heap<CellMatrix>();
            this._Connections = new Heap<string>();
            this._Connections.Allocate(TEMP, TempDir);

            this._RNG = new RandomCell();
            this._Timer = Stopwatch.StartNew();
            this._Libraries = new Heap<Library>();
            this._Base = new Libraries.BaseLibrary(this);

            // Add the base library to the library collection //
            this._Libraries.Allocate("GLOBAL", this._Base);

            // Possibly create a log writer //
            if (DEBUG_STATE == 1)
            {
                this._DebugWriter = new StreamWriter(DebugLogPath);
            }

        }

        /// <summary>
        /// Shuts down the host
        /// </summary>
        public void ShutDown()
        {
            
            this.Store.ShutDown();

            if (DEBUG_STATE == 1)
            {
                this._DebugWriter.Flush();
                this._DebugWriter.Close();
                this._DebugWriter = null;
            }

        }

        /// <summary>
        /// Internal table store
        /// </summary>
        public TableStore Store
        {
            get { return this._Cache; }
        }

        /// <summary>
        /// Internal non-debugger log
        /// </summary>
        public Communicator IO
        {
            get { return this._IO; }
        }

        /// <summary>
        /// In memory scalar collection
        /// </summary>
        public Heap<Cell> Scalars
        {
            get { return this._Scalars; }
        }

        /// <summary>
        /// In memory matrix store
        /// </summary>
        public Heap<CellMatrix> Matrixes
        {
            get { return this._Matrixes; }
        }

        /// <summary>
        /// Gets the elapsed time since the host was launched
        /// </summary>
        public long Elapsed
        {
            get { return this._Timer.ElapsedMilliseconds; }
        }

        // Random Number Generator //
        /// <summary>
        /// Base random number generator
        /// </summary>
        public RandomCell BaseRNG
        {
            get { return this._RNG; }
        }
        
        /// <summary>
        /// Sets the RNG seed
        /// </summary>
        /// <param name="Seed"></param>
        public void SetSeed(int Seed)
        {
            this._RNG = new RandomCell(Seed);
        }

        // Connection Support //
        /// <summary>
        /// Connection store
        /// </summary>
        public Heap<string> Connections
        {
            get { return this._Connections; }
        }

        /// <summary>
        /// The alias of the TEMP database
        /// </summary>
        public string TempDB
        {
            get { return this._Connections[TEMP]; }
        }

        /// <summary>
        /// Adds a database connection
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Connection"></param>
        public void AddConnection(string Alias, string Connection)
        {
            this._Connections.Allocate(Alias, Connection);
        }

        // Library //
        /// <summary>
        /// Collection of libraries; this does NOT include the base library
        /// </summary>
        public Heap<Library> Libraries
        {
            get { return this._Libraries; }
        }

        /// <summary>
        /// Base library, which includes the base function library, the base action library and the scalar / matrix heap the user's can alter
        /// </summary>
        public Library BaseLibrary
        {
            get { return this._Base; }
        }

        // Table Support //
        /// <summary>
        /// Opens a table given a path
        /// </summary>
        /// <param name="Key">The path to the file</param>
        /// <returns></returns>
        public Table OpenTable(string Key)
        {
            return this._Cache.RequestTable(Key);
        }

        /// <summary>
        /// Opens a table given an aliast and a name
        /// </summary>
        /// <param name="Alias">The alias; the alias must be an active connection</param>
        /// <param name="Name">The name of the table</param>
        /// <returns></returns>
        public Table OpenTable(string Alias, string Name)
        {
            if (this.Connections.Exists(Alias))
                return this.OpenTable(TableHeader.DeriveV1Path(this.Connections[Alias], Name));
            throw new Exception(string.Format("Connection '{0}' does not exist", Alias));
        }

        /// <summary>
        /// Creates a table with a cluster index
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="State"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public ClusteredTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns, ClusterState State, int PageSize)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            ClusteredTable t = new ClusteredTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, State, PageSize);
            return t;
        }

        /// <summary>
        /// Creates a table with a given index; the page size is defaulted
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public ClusteredTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns, ClusterState State)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name)); 
            ClusteredTable t = new ClusteredTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, State, Page.DEFAULT_SIZE);
            return t;
        }

        /// <summary>
        /// Creates a table with a clustered index; this assumes a default page size and a universal index
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <returns></returns>
        public ClusteredTable CreateTable(string Alias, string Name, Schema Columns, Key ClusterColumns)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name)); 
            ClusteredTable t = new ClusteredTable(this, Name, this._Connections[Alias], Columns, ClusterColumns, ClusterState.Universal, Page.DEFAULT_SIZE);
            return t;
        }

        /// <summary>
        /// Creates a heap table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public HeapTable CreateTable(string Alias, string Name, Schema Columns)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
            return new HeapTable(this, Name, this._Connections[Alias], Columns, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a dictionary table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <param name="KeyColumns"></param>
        /// <param name="ValueColumns"></param>
        /// <returns></returns>
        public DictionaryTable CreateTable(string Alias, string Name, Schema KeyColumns, Schema ValueColumns)
        {
            this._Cache.DropTable(TableHeader.DeriveV1Path(this._Connections[Alias], Name)); 
            return new DictionaryTable(this, Name, this._Connections[Alias], KeyColumns, ValueColumns, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a temporary table and adds to the recycle bin
        /// </summary>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public HeapTable CreateTempTable(Schema Columns)
        {
            HeapTable t = this.CreateTable(TEMP, Host.RandomName, Columns);
            this._Cache.PlaceInRecycleBin(t.Key);
            return t;
        }

        /// <summary>
        /// Checks if a table exists
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool TableExists(string Alias, string Name)
        {
            if (!this._Connections.Exists(Alias))
                return false;
            return this._Cache.TableExists(TableHeader.DeriveV1Path(this._Connections[Alias], Name));
        }

        // Debugging //
        public void DebugPrint(string Text, params object[] Obj)
        {

            if (DEBUG_STATE == 0)
                return;

            string x = new string('\t', Math.Max(0, this.DebugDepth));
            this._DebugWriter.WriteLine(string.Format(x + Text, Obj));

        }

        public int DebugDepth
        {
            get;
            set;
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

        public static string LongDateString
        {
            
            get
            {
                DateTime dt = DateTime.Now;
                return string.Format("{0}{1}{2}_{3}{4}{5}{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            }

        }

        internal static string DebugLogPath
        {

            get
            {
                return LogDir + "_DEBUG_LOG_" + LongDateString + ".txt";
            }

        }

    }

}
