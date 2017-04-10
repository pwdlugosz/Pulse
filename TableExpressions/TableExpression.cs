using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.Query;
using Pulse.ScalarExpressions;
using Pulse.Aggregates;
using Pulse.Query.Select;
using Pulse.Query.Aggregate;
using Pulse.Query.Join;
using Pulse.Query.Union;

namespace Pulse.TableExpressions
{

    public abstract class TableExpression
    {

        protected List<TableExpression> _Children;
        protected TableExpression _Parent;
        protected Host _Host;
        protected List<Table> _RecycleBin;
        protected Key _OrderBy;

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
        public List<TableExpression> Children
        {
            get { return this._Children; }
        }

        public IEnumerable<Table> ChildTables
        {
            get { return new TableCollection(this._Children); }
        }

        public TableExpression Parent
        {
            get { return this._Parent; }
        }

        public bool IsRoot
        {
            get { return this._Parent == null; }
        }

        public bool IsTerminal
        {
            get { return this._Children.Count == 0; }
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
        public WriteStream CreateWriter(Table Data)
        {

            // Don't do an order by and a cluster becuase it'll performa pointless sort
            if (this.IsOrdered && !this.IsClustered)
            {
                return new OrderedClusteredWriter(Data, this.OrderBy);
            }
            else if (this.IsDistinct)
            {
                return new DistinctWriteStream(Data);
            }
            else
            {
                return new VanillaWriteStream(Data);
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
        public abstract void Evaluate(WriteStream Writer);

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
            WriteStream ws = this.CreateWriter(t);
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
                this._Host.PageCache.DropTable(t.Key);
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

    public sealed class TableExpressionValue : TableExpression
    {

        private Table _t;

        public TableExpressionValue(Host Host, TableExpression Parent, Table Value)
            :base(Host, Parent)
        {
            this._t = Value;
        }

        public override Schema Columns
        {
            get { return this._t.Columns; }
        }

        public override Table Evaluate()
        {
            return this._t;
        }

        public override void Evaluate(WriteStream Writer)
        {

            Writer.Consume(this._t.OpenReader());

        }

        public override void Recycle()
        {
            // do nothing
        }

    }

    public sealed class TableExpressionSelect : TableExpression
    {

        private ScalarExpressionCollection _Fields;
        private Filter _Where;
        private Query.Select.SelectEngine _Engine;

        public TableExpressionSelect(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, Filter Where, Query.Select.SelectEngine Engine)
            : base(Host, Parent)
        {
            this._Fields = Fields;
            this._Where = Where;
            this._Engine = Engine;
        }

        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        public ScalarExpressionCollection Fields
        {
            get { return this._Fields; }
        }

        public Filter Where
        {
            get { return this._Where; }
        }

        public override void Evaluate(WriteStream Writer)
        {
            this._Engine.Render(this._Host, Writer, this.Children[0].Evaluate(), Fields, Where);
        }

    }

    public sealed class TableExpressionFold : TableExpression
    {

        private ScalarExpressionCollection _Keys;
        private AggregateCollection _Values;
        private Filter _Where;
        private AggregateEngine _Engine;

        public TableExpressionFold(Host Host, TableExpression Parent, ScalarExpressionCollection Keys, AggregateCollection Values, Filter Where, AggregateEngine Engine)
            : base(Host, Parent)
        {
            this._Keys = Keys;
            this._Values = Values;
            this._Where = Where;
            this._Engine = Engine;
        }

        public override Schema Columns
        {
            get { return this._Engine.GetOutputSchema(this._Keys, this._Values); }
        }

        public override void Evaluate(WriteStream Writer)
        {

            this._Engine.Render(this._Host, Writer, this._Children[0].Evaluate(), this._Keys, this._Values, this._Where);

        }

    }

    public sealed class TableExpressionJoin : TableExpression
    {

        private ScalarExpressionCollection _Fields;
        private RecordMatcher _Predicate;
        private Filter _Where;
        private JoinEngine _Engine;
        private JoinType _Type;

        public TableExpressionJoin(Host Host, TableExpression Parent, ScalarExpressionCollection Fields, RecordMatcher Predicate, Filter Where, JoinEngine Engine, JoinType Type)
            : base(Host, Parent)
        {
        }

        public override Schema Columns
        {
            get { return this._Fields.Columns; }
        }

        public override void Evaluate(WriteStream Writer)
        {
            this._Engine.Render(this._Host, Writer, this._Children[0].Evaluate(), this._Children[1].Evaluate(), this._Predicate, this._Fields, this._Where, JoinType.INNER);
        }

    }

    public sealed class TableExpressionUnion : TableExpression
    {

        private UnionEngine _Engine;

        public TableExpressionUnion(Host Host, TableExpression Parent, UnionEngine Engine)
            : base(Host, Parent)
        {
            this._Engine = Engine;
        }

        public override Schema Columns
        {
            get { return this._Children.First().Columns; }
        }

        public override void AddChild(TableExpression Child)
        {

            if (this._Children.Count == 0)
            {
                base.AddChild(Child);
            }
            else if (Child.Columns.Count == this.Columns.Count)
            {
                base.AddChild(Child);
            }
            else
            {
                throw new Exception("Schema of the child node is not compatible with current node");
            }

        }

        public override void Evaluate(WriteStream Writer)
        {
            this._Engine.Render(this._Host, Writer, this.ChildTables);
        }

    }

}
