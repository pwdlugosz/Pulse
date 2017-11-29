using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Libraries;
using Pulse.Elements;
using Pulse.Tables;

namespace Pulse.Expressions
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class FieldResolver
    {

        public const string GLOBAL = Host.GLOBAL;
        public const string LOCAL = "LOCAL";

        private Host _Host;
        private Heap<ObjectStore> _Stores;

        public FieldResolver(Host Host)
        {
            this._Host = Host;
            this._Stores = new Heap<ObjectStore>();
            this._Stores.Allocate(Host.GLOBAL, Host.Store);
            this._Stores.Allocate(LOCAL, new ObjectStore(this._Host));
        }

        // Stores //
        public Heap<ObjectStore> Stores
        {
            get { return this._Stores; }
        }

        public ObjectStore this[int Index]
        {
            get { return this._Stores[Index]; }
        }

        public ObjectStore this[string Name]
        {
            get { return this._Stores[Name]; }
        }

        public bool StoreExists(string Name)
        {
            return this._Stores.Exists(Name);
        }

        public ObjectStore Global
        {
            get { return this._Stores[GLOBAL]; }
        }

        public ObjectStore Local
        {
            get { return this._Stores[LOCAL]; }
        }

        // Scalars //
        public Cell GetScalar(string Major, string Minor)
        {
            return this._Stores[Major].GetScalar(Minor);
        }
        
        public void DeclareScalar(string Major, string Minor, Cell Value)
        {
            this._Stores[Major].Scalars.Allocate(Minor, Value);
        }

        public void SetScalar(string Major, string Minor, Cell Value)
        {
            this._Stores[Major].Scalars[Minor] = Value;
        }

        public Cell GetScalar(int Major, int Minor)
        {
            return this._Stores[Major].Scalars[Minor];
        }
        
        public void SetScalar(int Major, int Minor, Cell Value)
        {
            this._Stores[Major].Scalars[Minor] = Value;
        }

        // Matrixes //
        public CellMatrix GetMatrix(string Major, string Minor)
        {
            return this._Stores[Major].Matrixes[Minor];
        }
        
        public void DeclareMatrix(string Major, string Minor, CellMatrix Value)
        {
            this._Stores[Major].Matrixes.Allocate(Minor, Value);
        }

        public void SetMatrix(string Major, string Minor, CellMatrix Value)
        {
            this._Stores[Major].Matrixes[Minor] = Value;
        }
        
        public CellMatrix GetMatrix(int Major, int Minor)
        {
            return this._Stores[Major].Matrixes[Minor];
        }
        
        public void SetMatrix(int Major, int Minor, CellMatrix Value)
        {
            this._Stores[Major].Matrixes[Minor] = Value;
        }

        // Records //
        public AssociativeRecord GetRecord(string Major, string Minor)
        {
            return this._Stores[Major].Records[Minor];
        }

        public AssociativeRecord GetRecord(int Major, int Minor)
        {
            return this._Stores[Major].Records[Minor];
        }
        
        public Schema GetSchema(string Major, string Minor)
        {
            return this._Stores[Major].Records[Minor].Columns;
        }

        public Schema GetSchema(int Major, int Minor)
        {
            return this._Stores[Major].Records[Minor].Columns;
        }
        
        public void DeclareRecord(string Major, string Minor, AssociativeRecord Value)
        {
            this._Stores[Major].Records.Allocate(Minor, Value);
        }

        public void DeclareRecord(string Major, string Minor, Schema Value)
        {
            this._Stores[Major].Records.Allocate(Minor, new AssociativeRecord(Value, Value.NullRecord));
        }

        public void SetRecord(string Major, string Minor, AssociativeRecord Value)
        {
            this._Stores[Major].Records[Minor] = Value;
        }
        
        public void SetRecord(int Major, int Minor, AssociativeRecord Value)
        {
            this._Stores[Major].Records[Minor] = Value;
        }

        public void RemoveRecord(string Major, string Minor)
        {
            this._Stores[Major].RemoveRecord(Minor);
        }

    }
    
    /// <summary>
    /// Represents a collection of fields, scalars, and matrixes
    /// </summary>
    //public sealed class FieldResolver2
    //{

    //    /// <summary>
    //    /// Base host
    //    /// </summary>
    //    private Host _Host;

    //    /// <summary>
    //    /// Schema collection
    //    /// </summary>
    //    private Heap<Schema> _Columns;
        
    //    /// <summary>
    //    /// Record collection
    //    /// </summary>
    //    private Heap<Record> _TableRecords;

    //    /// <summary>
    //    /// Creates a field resolver
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    public FieldResolver2(Host Host)
    //    {

    //        this._Host = Host;
    //        this._Columns = new Heap<Schema>();
    //        this._TableRecords = new Heap<Record>();
    //        this.XID = Host.GetXID();

    //    }

    //    /// <summary>
    //    /// Gets the XID in the field resolver
    //    /// </summary>
    //    public long XID
    //    {
    //        get;
    //        protected set;
    //    }

    //    // Adds //
    //    /// <summary>
    //    /// Adds a schema to the resolver
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="RecordRef">The element that holds the index of the ref added</param>
    //    public void AddSchema(string Alias, Schema Columns, out int RecordRef)
    //    {
    //        if (this._Columns.Exists(Alias))
    //            throw new Exception(string.Format("Alias '{0}' already exists"));
            
    //        this._Columns.Allocate(Alias, Columns);
    //        this._TableRecords.Allocate(Alias, Columns.NullRecord);
    //        RecordRef = this._Columns.Count - 1;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    /// <param name="Columns"></param>
    //    public void AddSchema(string Alias, Schema Columns)
    //    {
    //        int x = 0;
    //        this.AddSchema(Alias, Columns, out x);
    //    }

    //    /// <summary>
    //    /// Imports the structure of one resolver to another
    //    /// </summary>
    //    /// <param name="Variants"></param>
    //    public void Import(FieldResolver Variants)
    //    {
            
    //        for (int i = 0; i < Variants._Columns.Count; i++)
    //        {
    //            this.AddSchema(Variants._Columns.Name(i), Variants._Columns[i]);
    //        }

    //    }

    //    // Get fields //
    //    /// <summary>
    //    /// Gets a field from the resolver
    //    /// </summary>
    //    /// <param name="TablePointer"></param>
    //    /// <param name="ColumnPointer"></param>
    //    /// <returns></returns>
    //    public Cell GetField(int TablePointer, int ColumnPointer)
    //    {
    //        return this._TableRecords[TablePointer][ColumnPointer];
    //    }

    //    /// <summary>
    //    /// Gets a field
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    /// <param name="FieldName"></param>
    //    /// <returns></returns>
    //    public Cell GetField(string Alias, string FieldName)
    //    {
    //        int tidx = this._Columns.GetPointer(Alias);
    //        int fidx = this._Columns[tidx].ColumnIndex(Alias);
    //        return this._TableRecords[tidx][fidx];
    //    }

    //    public Record GetRecord(int TablePointer)
    //    {
    //        return this._TableRecords[TablePointer];
    //    }

    //    public Record GetRecord(string Alias)
    //    {
    //        return this._TableRecords[Alias];
    //    }

    //    /// <summary>
    //    /// Gets a field name
    //    /// </summary>
    //    /// <param name="TablePointer"></param>
    //    /// <param name="ColumnPointer"></param>
    //    /// <returns></returns>
    //    public string GetFieldName(int TablePointer, int ColumnPointer)
    //    {
    //        return this._Columns[TablePointer].ColumnName(ColumnPointer);
    //    }

    //    /// <summary>
    //    /// Gets the cell affinity
    //    /// </summary>
    //    /// <param name="TablePointer"></param>
    //    /// <param name="ColumnPointer"></param>
    //    /// <returns></returns>
    //    public CellAffinity GetFieldAffinity(int TablePointer, int ColumnPointer)
    //    {
    //        return this._Columns[TablePointer].ColumnAffinity(ColumnPointer); ;
    //    }

    //    /// <summary>
    //    /// Gets the field size
    //    /// </summary>
    //    /// <param name="TablePointer"></param>
    //    /// <param name="ColumnPointer"></param>
    //    /// <returns></returns>
    //    public int GetFieldSize(int TablePointer, int ColumnPointer)
    //    {
    //        return this._Columns[TablePointer].ColumnSize(ColumnPointer); ;
    //    }

    //    /// <summary>
    //    /// Sets a record value
    //    /// </summary>
    //    /// <param name="TablePointer"></param>
    //    /// <param name="Value"></param>
    //    public void SetValue(int TablePointer, Record Value)
    //    {
    //        this._TableRecords[TablePointer] = Value;
    //    }

    //    /// <summary>
    //    /// Sets all the record values in one resolver to another
    //    /// </summary>
    //    /// <param name="Variants"></param>
    //    public void SetValues(FieldResolver Variants)
    //    {

    //        for (int i = 0; i < Variants._TableRecords.Count; i++)
    //        {
    //            this._TableRecords[i] = Variants._TableRecords[i];
    //        }

    //    }

    //    /// <summary>
    //    /// Removes a schema 
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    public void RemoveSchema(string Alias)
    //    {
    //        this._Columns.Deallocate(Alias);
    //        this._TableRecords.Deallocate(Alias);
    //    }

    //    // Signitures //
    //    /// <summary>
    //    /// Checks if both resolvers have the same count of schema and each schema has the same column count; DOES NOT check for field size or data type similarities
    //    /// </summary>
    //    /// <param name="Variants"></param>
    //    /// <returns></returns>
    //    public bool CheckColumnSignitures(FieldResolver Variants)
    //    {

    //        if (Variants._Columns.Count != this._Columns.Count)
    //            return false;
    //        for (int i = 0; i < this._Columns.Count; i++)
    //        {
    //            if (this._Columns[i].Count != Variants._Columns[i].Count)
    //                return false;
    //        }
    //        return true;

    //    }

    //    // Other //
    //    public FieldResolver2 CloneOfMe()
    //    {

    //        FieldResolver2 f = new FieldResolver2(this._Host);

    //        foreach (KeyValuePair<string, Schema> x in this._Columns.Entries)
    //        {
    //            f.AddSchema(x.Key, x.Value);
    //        }

    //        return f;

    //    }

    //    public FieldResolver2 CloneOfMeFull()
    //    {

    //        FieldResolver2 f = this.CloneOfMe();

    //        for (int i = 0; i < this._TableRecords.Count; i++)
    //        {
    //            f._TableRecords[i] = this._TableRecords[i];
    //        }

    //        return f;

    //    }

    //    // Statics //
    //    public static FieldResolver2 Build(Host Host, params Table[] Tables)
    //    {
    //        FieldResolver2 fr = new FieldResolver2(Host);
    //        foreach (Table t in Tables)
    //        {
    //            fr.AddSchema(t.Name, t.Columns);
    //        }
    //        return fr;

    //    }

    //    public static FieldResolver2 Union(FieldResolver2 A, FieldResolver2 B)
    //    {

    //        FieldResolver2 f = new FieldResolver2(A._Host);

    //        foreach (KeyValuePair<string, Schema> x in A._Columns.Entries)
    //        {
    //            f.AddSchema(x.Key, x.Value);
    //        }

    //        foreach (KeyValuePair<string, Schema> x in B._Columns.Entries)
    //        {
    //            if (!f._Columns.Exists(x.Key)) f.AddSchema(x.Key, x.Value);
    //        }

    //        return f;

    //    }


    //}

}
