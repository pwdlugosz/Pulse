using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Tables;

namespace Pulse.Expressions.RecordExpressions
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class ScalarExpressionSet : IColumns
    {

        private Heap<ScalarExpression> _Expressions;
        private Schema _Columns;
        private Host _Host;

        public ScalarExpressionSet(Host Host)
        {
            this._Expressions = new Heap<ScalarExpression>();
            this._Columns = new Schema();
        }

        public ScalarExpressionSet(Host Host, Schema Columns, string Alias)
            : this(Host)
        {

            for (int i = 0; i < Columns.Count; i++)
            {

                ScalarExpressionFieldRef2 e = new ScalarExpressionFieldRef2(this._Host, null, Alias, Columns.ColumnName(i), Columns.ColumnAffinity(i), Columns.ColumnSize(i));
                this.Add(Columns.ColumnName(i), e);
            }

        }

        public ScalarExpressionSet(Host Host, AssociativeRecord Record)
            : this(Host)
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

        public CellAffinity MaxAffinity
        {
            get
            {
                CellAffinity c = CellAffinityHelper.LOWEST_AFFINITY;
                foreach (ScalarExpression se in this._Expressions.Values)
                {
                    c = CellAffinityHelper.Highest(c, se.ReturnAffinity());
                }
                return c;
            }
        }

        public int MaxSize
        {
            get
            {
                int c = -1;
                foreach (ScalarExpression se in this._Expressions.Values)
                {
                    c = Math.Max(c, se.ReturnSize());
                }
                return c;
            }
        }

        public Schema Columns
        {
            get { return this._Columns; }
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
            this._Columns.Add(Alias, Element.ReturnAffinity(), Element.ReturnSize());
        }

        public void Add(ScalarExpressionSet Elements)
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

        public static ScalarExpressionSet operator +(ScalarExpressionSet A, ScalarExpressionSet B)
        {

            ScalarExpressionSet rex = new ScalarExpressionSet(A._Host);

            for (int i = 0; i < A.Count; i++)
                rex.Add(A.Alias(i), A[i]);
            for (int i = 0; i < B.Count; i++)
                rex.Add(B.Alias(i), B[i]);

            return rex;

        }

        public static List<AssociativeRecord> ToAssociativeRecordSet(List<ScalarExpressionSet> Records, FieldResolver Variants)
        {

            List<AssociativeRecord> rexs = new List<AssociativeRecord>();
            foreach (ScalarExpressionSet rex in Records)
                rexs.Add(rex.EvaluateAssociative(Variants));
            return rexs;

        }

        public static List<Record> ToRecordSet(List<ScalarExpressionSet> Records, FieldResolver Variants)
        {

            List<Record> rexs = new List<Record>();
            foreach (ScalarExpressionSet rex in Records)
                rexs.Add(rex.Evaluate(Variants));
            return rexs;

        }

    }

    public abstract class RecordExpression
    {

        protected Host _Host;
        protected RecordExpression _Parent;
        
        public RecordExpression(Host Host, RecordExpression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
        }

        public RecordExpression Parent
        {
            get { return this._Parent; }
        }

        public abstract Schema Columns
        {
            get;
        }

        public abstract RecordExpression CloneOfMe();

        public abstract AssociativeRecord EvaluateAssociative(FieldResolver Variants);

        public virtual Record Evaluate(FieldResolver Variants)
        {
            return this.EvaluateAssociative(Variants);
        }

    }

    public sealed class RecordExpressionStoreRef : RecordExpression
    {

        private string _StoreName;
        private string _ValueName;
        private Schema _Columns;

        public RecordExpressionStoreRef(Host Host, RecordExpression Parent, string StoreName, string ValueName, Schema Columns)
            : base(Host, Parent)
        {
            this._StoreName = StoreName;
            this._ValueName = ValueName;
            this._Columns = Columns;
        }

        public string RecordName
        {
            get { return this._ValueName; }
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override AssociativeRecord EvaluateAssociative(FieldResolver Variants)
        {
            return Variants[this._StoreName].GetRecord(this._ValueName);
        }

        public override RecordExpression CloneOfMe()
        {
            return new RecordExpressionStoreRef(this._Host, this._Parent, this._StoreName, this._ValueName, this._Columns);
        }

    }

    public sealed class RecordExpressionLiteral : RecordExpression
    {

        private Heap<ScalarExpression> _Scalars;

        public RecordExpressionLiteral(Host Host, RecordExpression Parent)
            : base(Host, Parent)
        {
            this._Scalars = new Heap<ScalarExpression>();
        }

        public void Add(ScalarExpression Value, string Alias)
        {
            this._Scalars.Allocate(Alias, Value);
        }

        public void Add(ScalarExpression Value)
        {
            string x = "F" + this._Scalars.Count.ToString();
            this.Add(Value, x);
        }

        public override RecordExpression CloneOfMe()
        {
            RecordExpressionLiteral x = new RecordExpressionLiteral(this._Host, this._Parent);
            foreach (KeyValuePair<string, ScalarExpression> kv in this._Scalars.Entries)
            {
                x.Add(kv.Value.CloneOfMe(), kv.Key);
            }
            return x;
        }

        public override AssociativeRecord EvaluateAssociative(FieldResolver Variants)
        {

            Schema columns = new Schema();
            List<Cell> cells = new List<Cell>();

            foreach (KeyValuePair<string, ScalarExpression> kv in this._Scalars.Entries)
            {
                columns.Add(kv.Key, kv.Value.ReturnAffinity(), kv.Value.ReturnSize());
                cells.Add(kv.Value.Evaluate(Variants));
            }

            return new AssociativeRecord(columns, cells.ToArray());

        }

        public override Schema Columns
        {
            get 
            {

                Schema columns = new Schema();
                foreach (KeyValuePair<string, ScalarExpression> kv in this._Scalars.Entries)
                {
                    columns.Add(kv.Key, kv.Value.ReturnAffinity(), kv.Value.ReturnSize());
                }
                return columns;

            }
        }

    }

    public sealed class RecordExpressionCTOR : RecordExpression
    {

        private Schema _cols;

        public RecordExpressionCTOR(Host Host, RecordExpression Parent, Schema Columns)
            : base(Host, Parent)
        {
            this._cols = Columns;
        }

        public override RecordExpression CloneOfMe()
        {
            return new RecordExpressionCTOR(this._Host, this._Parent, this._cols);
        }

        public override AssociativeRecord EvaluateAssociative(FieldResolver Variants)
        {

            return new AssociativeRecord(this._cols);

        }

        public override Schema Columns
        {
            get 
            {
                return this._cols;
            }
        }


    }

    public abstract class RecordExpressionFunction : RecordExpression
    {

        private string _Name;
        private int _ParamCount = -1;
        private List<Parameter> _Parameters;
        
        public RecordExpressionFunction(Host Host, RecordExpression Parent, string Name, int Parameters)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._ParamCount = Parameters;
            this._Parameters = new List<Parameter>();
        }

        public string FunctionName
        {
            get { return this._Name; }
        }

        public virtual bool IsVolatile
        {
            get { return true; }
        }

        public int ParameterCount
        {
            get { return this._ParamCount; }
        }

        public void AddParameter(Parameter Value)
        {
            this._Parameters.Add(Value);
        }

        public void CheckParameters()
        {
            
            if (this._ParamCount < 0 && this._Parameters.Count > (-this._ParamCount))
            {
                throw new Exception(string.Format("Function '{0}' can have at most '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }
            else if (this._Parameters.Count != this._ParamCount)
            {
                throw new Exception(string.Format("Function '{0}' can have exactly '{1}' parameter(s) but was passed '{2}'", this._Name, -this._ParamCount, this._Parameters.Count));
            }

        }


    }

    public sealed class RecordExpressionPointer : RecordExpression
    {

        private string _Name;
        private Schema _Columns;

        public RecordExpressionPointer(Host Host, RecordExpression Parent, string Name, Schema Columns)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._Columns = Columns;
        }

        public override Schema Columns
        {
            get { return this._Columns; }
        }

        public override Record Evaluate(FieldResolver Variants)
        {
            throw new Exception("Cannot evaluate pointer expressions");
        }

        public override AssociativeRecord EvaluateAssociative(FieldResolver Variants)
        {
            throw new Exception("Cannot evaluate pointer expressions");
        }

        public override RecordExpression CloneOfMe()
        {
            return new RecordExpressionPointer(this._Host, this._Parent, this._Name, this._Columns);
        }

    }


}
