﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this._Scalars = new Heap<Heap<Cell>>();
            this._Matrixes = new Heap<Heap<CellMatrix>>();

            if (this._Host != null)
            {
                this._Scalars.Allocate(Host.GLOBAL, this._Host.Scalars);
                this._Matrixes.Allocate(Host.GLOBAL, this._Host.Matrixes);
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
            this._Aliases.Allocate(Alias, VariantType.Field);
            this._Columns.Allocate(Alias, Columns);
            this._Records.Allocate(Alias, Columns.NullRecord);
        }

        /// <summary>
        /// Adds a scalar heap to the resolver
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Library"></param>
        public void AddScalars(string Alias, Heap<Cell> Library)
        {

            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Scalar);
            this._Scalars.Allocate(Alias, Library);

        }

        /// <summary>
        /// Adds a matrix heap to the resolver
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Library"></param>
        public void AddMatrixes(string Alias, Heap<CellMatrix> Library)
        {

            if (this.AliasExists(Alias))
                throw new Exception(string.Format("Alias '{0}' already exists"));
            this._Aliases.Allocate(Alias, VariantType.Matrix);
            this._Matrixes.Allocate(Alias, Library);

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

        // Get scalars //
        /// <summary>
        /// Gets a scalar value
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public Cell GetScalar(int HeapPointer, int ScalarPointer)
        {
            return this._Scalars[HeapPointer][ScalarPointer];
        }

        /// <summary>
        /// Gets a scalar value
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="ScalarName"></param>
        /// <returns></returns>
        public Cell GetScalar(string Alias, string ScalarName)
        {
            int tidx = this._Scalars.GetPointer(Alias);
            int fidx = this._Scalars[tidx].GetPointer(ScalarName);
            return this._Scalars[tidx][fidx];
        }

        /// <summary>
        /// Gets a scalar's name
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public string GetScalarName(int HeapPointer, int ScalarPointer)
        {
            return this._Scalars[HeapPointer].Name(ScalarPointer);
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
            return this._Matrixes[HeapPointer][MatrixPointer];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="MatrixName"></param>
        /// <returns></returns>
        public CellMatrix GetMatrix(string Alias, string MatrixName)
        {
            int tidx = this._Scalars.GetPointer(Alias);
            int fidx = this._Scalars[tidx].GetPointer(MatrixName);
            return this._Matrixes[tidx][fidx];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HeapPointer"></param>
        /// <param name="ScalarPointer"></param>
        /// <returns></returns>
        public string GetMatrixName(int HeapPointer, int ScalarPointer)
        {
            return this._Matrixes[HeapPointer].Name(ScalarPointer);
        }


    }

}