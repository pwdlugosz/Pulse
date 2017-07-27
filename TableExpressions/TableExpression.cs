using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Pulse.Data;
using Pulse.Query;
using Pulse.ScalarExpressions;
using Pulse.Aggregates;

namespace Pulse.TableExpressions
{

    public abstract class TableExpression : IBindable
    {

        protected List<TableExpression> _Children;
        protected TableExpression _Parent;
        protected Host _Host;
        protected List<Table> _RecycleBin;
        protected Key _OrderBy;
        protected Stopwatch _Timer;
        
        public TableExpression(Host Host, TableExpression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<TableExpression>();
            this._RecycleBin = new List<Table>();
            this._OrderBy = new Key();
            this.IsClustered = false;
            this.IsDistinct = false;
        }

        // Tree support //
        /// <summary>
        /// All child expressions
        /// </summary>
        public List<TableExpression> Children
        {
            get { return this._Children; }
        }

        /// <summary>
        /// All child tables
        /// </summary>
        public IEnumerable<Table> ChildTables
        {
            get { return new TableCollection(this._Children); }
        }

        /// <summary>
        /// Gets the parent table; null if this is the root node
        /// </summary>
        public TableExpression Parent
        {
            get { return this._Parent; }
        }

        /// <summary>
        /// True if this is the first nod ein the tree
        /// </summary>
        public bool IsRoot
        {
            get { return this._Parent == null; }
        }

        /// <summary>
        /// True if this is the last node in the tree
        /// </summary>
        public bool IsTerminal
        {
            get { return this._Children.Count == 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Alias
        {
            get;
            set;
        }

        // Optimizations //
        /// <summary>
        /// If true, will write only distinct results to the output record set
        /// </summary>
        public bool IsDistinct
        {
            get;
            set;
        }

        /// <summary>
        /// True if the expression requires an order by
        /// </summary>
        public bool IsOrdered
        {
            get { return !this.OrderBy.IsEmpty; }
        }

        /// <summary>
        /// If true, then the expression will be written to a clustered table using the OrderBy key
        /// </summary>
        public bool IsClustered
        {
            get;
            set;
        }

        /// <summary>
        /// If non-empty, then the 
        /// </summary>
        public Key OrderBy
        {
            get { return this._OrderBy; }
            set { this._OrderBy = value; }
        }

        // Output methods //
        /// <summary>
        /// Creates an output stream to write the data to
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public RecordWriter CreateWriter(Table Data)
        {

            // Don't do an order by and a cluster becuase it'll performa pointless sort
            if (this.IsOrdered && !this.IsClustered)
            {
                return new RecordWriterClustered(Data, this.OrderBy);
            }
            else if (this.IsDistinct)
            {
                return new RecordWriterDistinct(Data);
            }
            else
            {
                return new RecordWriterBase(Data);
            }

        }

        /// <summary>
        /// Creates a hard table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Table CreateTable(string DB, string Name)
        {

            if (this.IsClustered)
                return this._Host.CreateTable(DB, Name, this.Columns, this.OrderBy);
            return this._Host.CreateTable(DB, Name, this.Columns);

        }

        /// <summary>
        /// Creates a temp table to output to
        /// </summary>
        /// <returns></returns>
        public Table CreateTempTable()
        {
            return this.CreateTable(Host.TEMP, Host.RandomName);
        }

        /// <summary>
        /// Creates and loads all data from the expression into the table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Table RenderTable(string DB, string Name)
        {
            Table t = this.CreateTable(DB, Name);
            using (RecordWriter w = this.CreateWriter(t))
            {
                this.Evaluate(w);
            }
            return t;
        }

        /// <summary>
        /// Creates a temp table and loads it with the table expression
        /// </summary>
        /// <returns></returns>
        public Table RenderTempTable()
        {
            Table t = this.CreateTempTable();
            using (RecordWriter w = this.CreateWriter(t))
            {
                this.Evaluate(w);
            }
            return t;
        }

        // Methods //
        /// <summary>
        /// Adds a child expression to the set
        /// </summary>
        /// <param name="Child"></param>
        public virtual void AddChild(TableExpression Child)
        {
            Child._Parent = this;
            this._Children.Add(Child);
        }

        /// <summary>
        /// Adds many children to the expression
        /// </summary>
        /// <param name="Children"></param>
        public void AddChildren(List<TableExpression> Children)
        {
            foreach (TableExpression t in Children)
            {
                this.AddChild(t);
            }
        }

        // Abstracts and virtuals //
        /// <summary>
        /// Gets the columns of the output table
        /// </summary>
        public abstract Schema Columns
        {
            get;
        }

        /// <summary>
        /// Writes the value of expression to a table
        /// </summary>
        /// <param name="Writer"></param>
        public abstract void Evaluate(RecordWriter Writer);

        /// <summary>
        /// Returns meta data around the expression
        /// </summary>
        /// <returns></returns>
        //public abstract string MetaData();

        /// <summary>
        /// Evaluates the expression and returns a table
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public virtual Table Evaluate(string DB, string Name)
        {

            Table t = this.CreateTable(DB, Name);
            RecordWriter ws = this.CreateWriter(t);
            this.Evaluate(ws);
            ws.Close();

            return t;

        }

        /// <summary>
        /// Evalutes the table into a temp table
        /// </summary>
        /// <returns></returns>
        public virtual Table Evaluate()
        {
            return this.Evaluate(Host.TEMP, Host.RandomName);
        }

        /// <summary>
        /// Cleans up any resources no longer needed
        /// </summary>
        public virtual void Recycle()
        {

            // Recycle this node //
            foreach (Table t in this._RecycleBin)
            {
                this._Host.Store.DropTable(t.Key);
            }


        }

        /// <summary>
        /// Recycling
        /// </summary>
        public virtual void RecycleAll()
        {

            this.Recycle();

            // Recycle all children //
            foreach (TableExpression te in this._Children)
            {
                te.Recycle();
            }

        }

        /// <summary>
        /// Binds a pointer expression to a value expression
        /// </summary>
        /// <param name="PointerRef"></param>
        /// <param name="Value"></param>
        public virtual void Bind(string PointerRef, ScalarExpression Value)
        {
            this._Children.ForEach((x) => { x.Bind(PointerRef, Value); });
        }

        /// <summary>
        /// Gets an estimate of the records of underlying table
        /// </summary>
        public virtual long EstimatedCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Checks if an expression will yield an indexed table
        /// </summary>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public virtual bool IsIndexedBy(Key IndexColumns)
        {
            return Key.LeftStrong(IndexColumns, this.OrderBy);
        }

        /// <summary>
        /// Returns meta data about the expression rendering
        /// </summary>
        /// <returns></returns>
        public virtual string MetaData()
        {
            return "META_DATA";
        }

        // Internal Classes //
        /// <summary>
        /// Represents a way to enumerate over all the child tables
        /// </summary>
        public sealed class TableCollection : IEnumerable<Table>, IEnumerator<Table>, IEnumerable, IEnumerator, IDisposable
        {

            private int _Position = -1;
            private List<TableExpression> _Nodes;

            public TableCollection(List<TableExpression> Nodes)
            {
                this._Nodes = Nodes;
            }

            public bool MoveNext()
            {
                this._Position++;
                return (this._Position < this._Nodes.Count);
            }

            public void Reset()
            {
                this._Position = -1;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Table Current
            {
                get
                {
                    return this._Nodes[this._Position].Evaluate();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            IEnumerator<Table> IEnumerable<Table>.GetEnumerator()
            {
                return this;
            }

            public void Dispose()
            {
            }

        }

    }

}
