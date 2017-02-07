using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
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
            Field,
            Scalar,
            Matrix
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
        private Heap<Heap<Cell>> _Scalars;
        
        /// <summary>
        /// Cell matrix collection
        /// </summary>
        private Heap<Heap<CellMatrix>> _Matrixes;

        public FieldResolver(Host Host)
        {
            this._Host = Host;
            this._Aliases = new Heap<VariantType>();
            this._Columns = new Heap<Schema>();
            this._Records = new Heap<Record>();
            this._Scalars = new Heap<Heap<Cell>>();
            this._Matrixes = new Heap<Heap<CellMatrix>>();
        }

        public bool AliasExists(string Alias)
        {
            return this._Aliases.Exists(Alias);
        }

        // Adds //
        public void AddSchema(string Alias, Schema Columns)
        {
            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Field);
            this._Columns.Allocate(Alias, Columns);
            this._Records.Allocate(Alias, Columns.NullRecord);
        }

        public void AddScalars(string Alias, Heap<Cell> Library)
        {

            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Scalar);
            this._Scalars.Allocate(Alias, Library);

        }

        public void AddMatrixes(string Alias, Heap<CellMatrix> Library)
        {

            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Matrix);
            this._Matrixes.Allocate(Alias, Library);

        }

        // Get fields //
        public Cell GetField(int TablePointer, int ColumnPointer)
        {
            return this._Records[TablePointer][ColumnPointer];
        }

        public Cell GetField(string Alias, string FieldName)
        {
            int tidx = this._Columns.GetPointer(Alias);
            int fidx = this._Columns[tidx].ColumnIndex(Alias);
            return this._Records[tidx][fidx];
        }

        public string GetFieldName(int TablePointer, int ColumnPointer)
        {
            return this._Columns[TablePointer].ColumnName(ColumnPointer);
        }

        public CellAffinity GetFieldAffinity(int TablePointer, int ColumnPointer)
        {
            return this._Columns[TablePointer].ColumnAffinity(ColumnPointer); ;
        }

        public int GetFieldSize(int TablePointer, int ColumnPointer)
        {
            return this._Columns[TablePointer].ColumnSize(ColumnPointer); ;
        }

        // Get scalars //
        public Cell GetScalar(int HeapPointer, int ScalarPointer)
        {
            return this._Scalars[HeapPointer][ScalarPointer];
        }

        public Cell GetScalar(string Alias, string ScalarName)
        {
            int tidx = this._Scalars.GetPointer(Alias);
            int fidx = this._Scalars[tidx].GetPointer(ScalarName);
            return this._Scalars[tidx][fidx];
        }

        // Get matrixes //
        public CellMatrix GetMatrix(int HeapPointer, int MatrixPointer)
        {
            return this._Matrixes[HeapPointer][MatrixPointer];
        }

        public CellMatrix GetMatrix(string Alias, string MatrixName)
        {
            int tidx = this._Scalars.GetPointer(Alias);
            int fidx = this._Scalars[tidx].GetPointer(MatrixName);
            return this._Matrixes[tidx][fidx];
        }


    }

}
