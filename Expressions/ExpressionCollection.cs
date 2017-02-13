using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;

namespace Pulse.Expressions
{

    public sealed class ExpressionCollection : IColumns
    {

        private Heap<Expression> _Expressions;

        public ExpressionCollection()
        {
            this._Expressions = new Heap<Expression>();
        }

        public ExpressionCollection(Schema Columns, int InnerHeapRef)
            : this()
        {

            for (int i = 0; i < this._Expressions.Count; i++)
            {
                ExpressionFieldRef e = new ExpressionFieldRef(null, InnerHeapRef, i, Columns.ColumnAffinity(i), Columns.ColumnSize(i));
                this.Add(Columns.ColumnName(i), e);
            }

        }

        public int Count
        {
            get { return this._Expressions.Count; }
        }

        public Expression this[int IndexOf]
        {
            get { return this._Expressions[IndexOf]; }
        }

        public Expression this[string Name]
        {
            get { return this._Expressions[Name]; }
        }

        public IEnumerable<Expression> Expressions
        {
            get { return this._Expressions.Values; }
        }

        public Schema Columns
        {
            get
            {

                Schema s = new Schema();
                for (int i = 0; i < this._Expressions.Count; i++)
                {
                    s.Add(this._Expressions.Name(i), this._Expressions[i].ExpressionReturnAffinity(), this._Expressions[i].ExpressionSize());
                }
                return s;

            }
        }

        public string Alias(int IndexOf)
        {
            return this._Expressions.Name(IndexOf);
        }

        public void Add(string Alias, Expression Element)
        {
            Element.Name = Alias;
            this._Expressions.Allocate(Alias, Element);
        }

        public void Add(ExpressionCollection Elements)
        {

            for (int i = 0; i < Elements.Count; i++)
            {
                this.Add(Elements.Alias(i), Elements[i]);
            }

        }

        public Record Evaluate(FieldResolver Variants)
        {
            Cell[] c = new Cell[this.Count];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = this._Expressions[i].Evaluate(Variants);
            }
            return new Record(c);
        }

    }

}
