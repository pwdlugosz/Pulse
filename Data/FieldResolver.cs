using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Libraries;

namespace Pulse.Data
{
    
    /// <summary>
    /// Represents a collection of fields, scalars, and matrixes
    /// </summary>
    public sealed class FieldResolver
    {

        /// <summary>
        /// The type of variable stored
        /// </summary>
        public enum VariantType
        {
            Schema,
            Library
        }

        /// <summary>
        /// Base host
        /// </summary>
        private Host _Host;

        /// <summary>
        /// Master list of all aliases
        /// </summary>
        private Heap<VariantType> _Aliases;
        
        /// <summary>
        /// Schema collection
        /// </summary>
        private Heap<Schema> _Columns;
        
        /// <summary>
        /// Record collection
        /// </summary>
        private Heap<Record> _Records;

        /// <summary>
        /// Scalars collection
        /// </summary>
        private Heap<Library> _Libraries;
        
        /// <summary>
        /// Creates a field resolver
        /// </summary>
        /// <param name="Host"></param>
        public FieldResolver(Host Host)
        {

            this._Host = Host;
            this._Aliases = new Heap<VariantType>();
            this._Columns = new Heap<Schema>();
            this._Records = new Heap<Record>();
            this._Libraries = new Heap<Library>();

            if (this._Host != null)
            {
                this.AddLibrary(Host.GLOBAL, Host.BaseLibrary);
            }

        }

        /// <summary>
        /// Checks if an alias exists in the resolver already
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public bool AliasExists(string Alias)
        {
            return this._Aliases.Exists(Alias);
        }

        // Adds //
        /// <summary>
        /// Adds a schema to the resolver
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Columns"></param>
        public void AddSchema(string Alias, Schema Columns)
        {
            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Library);
            this._Columns.Allocate(Alias, Columns);
            this._Records.Allocate(Alias, Columns.NullRecord);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Library"></param>
        public void AddLibrary(string Alias, Library Library)
        {
            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Library);
            this._Libraries.Allocate(Alias, Library);
        }

        /// <summary>
        /// Imports the structure of one resolver to another
        /// </summary>
        /// <param name="Variants"></param>
        public void Import(FieldResolver Variants)
        {
            
            for (int i = 0; i < Variants._Columns.Count; i++)
            {
                this.AddSchema(Variants._Columns.Name(i), Variants._Columns[i]);
            }

            for (int i = 0; i < Variants._Libraries.Count; i++)
            {
                if (Variants._Libraries.Name(i) != Host.GLOBAL)
                    this.AddLibrary(Variants._Libraries.Name(i), Variants._Libraries[i]);
            }

        }

        // Get fields //
        /// <summary>
        /// Gets a field from the resolver
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <param name="ColumnPointer"></param>
        /// <returns></returns>
        public Cell GetField(int TablePointer, int ColumnPointer)
        {
            return this._Records[TablePointer][ColumnPointer];
        }

        /// <summary>
        /// Gets a field
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="FieldName"></param>
        /// <returns></returns>
        public Cell GetField(string Alias, string FieldName)
        {
            int tidx = this._Columns.GetPointer(Alias);
            int fidx = this._Columns[tidx].ColumnIndex(Alias);
            return this._Records[tidx][fidx];
        }

        /// <summary>
        /// Gets a field name
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <param name="ColumnPointer"></param>
        /// <returns></returns>
        public string GetFieldName(int TablePointer, int ColumnPointer)
        {
            return this._Columns[TablePointer].ColumnName(ColumnPointer);
        }

        /// <summary>
        /// Gets the cell affinity
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <param name="ColumnPointer"></param>
        /// <returns></returns>
        public CellAffinity GetFieldAffinity(int TablePointer, int ColumnPointer)
        {
            return this._Columns[TablePointer].ColumnAffinity(ColumnPointer); ;
        }

        /// <summary>
        /// Gets the field size
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <param name="ColumnPointer"></param>
        /// <returns></returns>
        public int GetFieldSize(int TablePointer, int ColumnPointer)
        {
            return this._Columns[TablePointer].ColumnSize(ColumnPointer); ;
        }

        /// <summary>
        /// Sets a record value
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <param name="Value"></param>
        public void SetValue(int TablePointer, Record Value)
        {
            this._Records[TablePointer] = Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <returns></returns>
        public Record GetRecord(int TablePointer)
        {
            return this._Records[TablePointer];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public Record GetRecord(string Alias)
        {
            return this._Records[Alias];
        }

        /// <summary>
        /// Sets all the record values in one resolver to another
        /// </summary>
        /// <param name="Variants"></param>
        public void SetValues(FieldResolver Variants)
        {

            for (int i = 0; i < Variants._Records.Count; i++)
            {
                this._Records[i] = Variants._Records[i];
            }

        }

        /// <summary>
        /// Removes a schema 
        /// </summary>
        /// <param name="Alias"></param>
        public void RemoveSchema(string Alias)
        {
            this._Aliases.Deallocate(Alias);
            this._Columns.Deallocate(Alias);
            this._Records.Deallocate(Alias);
        }

        /// <summary>
        /// Using an existing pointer, points the data to a different schema
        /// </summary>
        /// <param name="TablePointer"></param>
        /// <param name="NewSchema"></param>
        public void Reclaim(int TablePointer, Schema NewSchema)
        {

            string Alias = this._Aliases.Name(TablePointer);

            this._Aliases.Deallocate(Alias);
            this._Columns.Deallocate(Alias);
            this._Records.Deallocate(Alias);

            this._Aliases.Collide(Alias, VariantType.Schema, TablePointer);
            this._Columns.Collide(Alias, NewSchema, TablePointer);
            this._Records.Collide(Alias, NewSchema.NullRecord, TablePointer);

        }

        // Get library //
        /// <summary>
        /// Gets a library value
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public Library GetLibrary(int HeapPointer)
        {
            return this._Libraries[HeapPointer];
        }

        /// <summary>
        /// Gets a library value
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="ScalarName"></param>
        /// <returns></returns>
        public Library GetLibrary(string Alias)
        {
            int tidx = this._Libraries.GetPointer(Alias);
            return this._Libraries[tidx];
        }

        /// <summary>
        /// Gets a library's name
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public string GetLibraryName(int HeapPointer)
        {
            return this._Libraries[HeapPointer].Name;
        }

        // Get scalars //
        /// <summary>
        /// Gets a scalar value
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public Cell GetScalar(int HeapPointer, int ScalarPointer)
        {
            return this._Libraries[HeapPointer].Values[ScalarPointer];
        }

        /// <summary>
        /// Gets a scalar value
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="ScalarName"></param>
        /// <returns></returns>
        public Cell GetScalar(string Alias, string ScalarName)
        {
            int tidx = this._Libraries.GetPointer(Alias);
            int fidx = this._Libraries[tidx].Values.GetPointer(ScalarName);
            return this._Libraries[tidx].Values[fidx];
        }

        /// <summary>
        /// Gets a scalar's name
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public string GetScalarName(int HeapPointer, int ScalarPointer)
        {
            return this._Libraries[HeapPointer].Values.Name(ScalarPointer);
        }

        // Get matrixes //
        /// <summary>
        /// 
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="MatrixPointer"></param>
        /// <returns></returns>
        public CellMatrix GetMatrix(int HeapPointer, int MatrixPointer)
        {
            return this._Libraries[HeapPointer].Matrixes[MatrixPointer];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="MatrixName"></param>
        /// <returns></returns>
        public CellMatrix GetMatrix(string Alias, string MatrixName)
        {
            int tidx = this._Libraries.GetPointer(Alias);
            int fidx = this._Libraries[tidx].Matrixes.GetPointer(MatrixName);
            return this._Libraries[tidx].Matrixes[fidx];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public string GetMatrixName(int HeapPointer, int ScalarPointer)
        {
            return this._Libraries[HeapPointer].Matrixes.Name(ScalarPointer);
        }

        // Signitures //
        /// <summary>
        /// Checks if both resolvers have the same count of schema and each schema has the same column count; DOES NOT check for field size or data type similarities
        /// </summary>
        /// <param name="Variants"></param>
        /// <returns></returns>
        public bool CheckColumnSignitures(FieldResolver Variants)
        {

            if (Variants._Columns.Count != this._Columns.Count)
                return false;
            for (int i = 0; i < this._Columns.Count; i++)
            {
                if (this._Columns[i].Count != Variants._Columns[i].Count)
                    return false;
            }
            return true;

        }

        // Other //
        public FieldResolver CloneOfMe()
        {

            FieldResolver f = new FieldResolver(this._Host);

            foreach (KeyValuePair<string, Schema> x in this._Columns.Entries)
            {
                f.AddSchema(x.Key, x.Value);
            }

            foreach (KeyValuePair<string, Library> x in this._Libraries.Entries)
            {
                if (!f._Libraries.Exists(x.Key)) f.AddLibrary(x.Key, x.Value); // In case we collide with 'GLOBAL'
            }

            return f;

        }

        // Statics //
        public static FieldResolver Build(Host Host, params Table[] Tables)
        {
            FieldResolver fr = new FieldResolver(Host);
            foreach (Table t in Tables)
            {
                fr.AddSchema(t.Name, t.Columns);
            }
            return fr;

        }

        public static FieldResolver Union(FieldResolver A, FieldResolver B)
        {

            FieldResolver f = new FieldResolver(A._Host);

            foreach (KeyValuePair<string, Schema> x in A._Columns.Entries)
            {
                f.AddSchema(x.Key, x.Value);
            }

            foreach (KeyValuePair<string, Schema> x in B._Columns.Entries)
            {
                if (!f._Columns.Exists(x.Key)) f.AddSchema(x.Key, x.Value);
            }

            foreach (KeyValuePair<string, Library> x in A._Libraries.Entries)
            {
                if (!f._Libraries.Exists(x.Key)) f.AddLibrary(x.Key, x.Value); // In case we collide with 'GLOBAL'
            }

            foreach (KeyValuePair<string, Library> x in B._Libraries.Entries)
            {
                if (!f._Libraries.Exists(x.Key)) f.AddLibrary(x.Key, x.Value);
            }

            return f;

        }


    }

}
