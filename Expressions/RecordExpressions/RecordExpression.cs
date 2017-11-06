using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Tables;

namespace Pulse.Expressions.RecordExpressions
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class RecordExpression : IColumns
    {

        private Heap<ScalarExpression> _Expressions;

        public RecordExpression()
        {
            this._Expressions = new Heap<ScalarExpression>();
        }

        public RecordExpression(Schema Columns, int InnerHeapRef)
            : this()
        {

            for (int i = 0; i < Columns.Count; i++)
            {
                ScalarExpressionFieldRef e = new ScalarExpressionFieldRef(null, InnerHeapRef, i, Columns.ColumnAffinity(i), Columns.ColumnSize(i));
                this.Add(Columns.ColumnName(i), e);
            }

        }

        public RecordExpression(AssociativeRecord Record)
            : this()
        {

            for (int i = 0; i < Record.Count; i++)
            {
                this.Add(Record.Columns.ColumnName(i), new ScalarExpressionConstant(null, Record[i]));
            }

        }

        public int Count
        {
            get { return this._Expressions.Count; }
        }

        public ScalarExpression this[int IndexOf]
        {
            get { return this._Expressions[IndexOf]; }
        }

        public ScalarExpression this[string Name]
        {
            get { return this._Expressions[Name]; }
        }

        public IEnumerable<ScalarExpression> Expressions
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

        public bool Exists(string Name)
        {
            return this._Expressions.Exists(Name);
        }

        public string Alias(int IndexOf)
        {
            return this._Expressions.Name(IndexOf);
        }

        public void Add(string Alias, ScalarExpression Element)
        {
            Element.Name = Alias;
            this._Expressions.Allocate(Alias, Element);
        }

        public void Add(RecordExpression Elements)
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

        public AssociativeRecord EvaluateAssociative(FieldResolver Variants)
        {
            return new AssociativeRecord(this.Columns, this.Evaluate(Variants));
        }

        public static RecordExpression operator +(RecordExpression A, RecordExpression B)
        {

            RecordExpression rex = new RecordExpression();

            for (int i = 0; i < A.Count; i++)
                rex.Add(A.Alias(i), A[i]);
            for (int i = 0; i < B.Count; i++)
                rex.Add(B.Alias(i), B[i]);

            return rex;

        }

        public static List<AssociativeRecord> ToAssociativeRecordSet(List<RecordExpression> Records, FieldResolver Variants)
        {

            List<AssociativeRecord> rexs = new List<AssociativeRecord>();
            foreach (RecordExpression rex in Records)
                rexs.Add(rex.EvaluateAssociative(Variants));
            return rexs;

        }

        public static List<Record> ToRecordSet(List<RecordExpression> Records, FieldResolver Variants)
        {

            List<Record> rexs = new List<Record>();
            foreach (RecordExpression rex in Records)
                rexs.Add(rex.Evaluate(Variants));
            return rexs;

        }

    }

}
