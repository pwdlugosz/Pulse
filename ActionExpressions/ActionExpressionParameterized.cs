using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.TableExpressions;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;

namespace Pulse.ActionExpressions
{
    
    public abstract class ActionExpressionParameterized : ActionExpression
    {

        public enum ParameterType : byte
        {
            Table,
            Matrix,
            Record,
            Scalar
        }

        public sealed class ParameterPointer
        {

            public ParameterPointer(string Name, ParameterType Affinity, bool IsRequired)
            {
                this.Name = Name;
                this.Affinity = Affinity;
                this.IsRequired = IsRequired;
            }

            public string Name 
            { 
                get; 
                private set; 
            }

            public ParameterType Affinity
            {
                get;
                private set;
            }

            public bool IsRequired 
            { 
                get; 
                private set; 
            }

            public override string ToString()
            {
                return string.Format("Name: {0} | Type: {1} | Required: {2}", this.Name, this.Affinity, this.IsRequired);
            }

            public static ParameterPointer Parse(string Text)
            {
                string[] vars = Text.ToUpper().Split('.');
                if (vars.Length != 2 && vars.Length != 3)
                    throw new Exception(string.Format("String passed is invalid: '{0}'", Text));
                string Name = vars[0];
                ParameterType Affinity = ParameterType.Scalar;
                if (vars[1] == "S")
                    Affinity = ParameterType.Scalar;
                else if (vars[1] == "R")
                    Affinity = ParameterType.Record;
                else if (vars[1] == "M")
                    Affinity = ParameterType.Matrix;
                else if (vars[1] == "T")
                    Affinity = ParameterType.Table;
                bool IsRequired = (vars.Length == 2 ? false : vars[2] == "R");
                return new ParameterPointer(Name, Affinity, IsRequired);

            }

            public static Heap<ParameterPointer> ParseMap(string Text)
            {

                Heap<ParameterPointer> map = new Heap<ParameterPointer>();

                foreach (string t in Text.Split(';'))
                {
                    ParameterPointer pp = Parse(t);
                    map.Allocate(pp.Name, pp);
                }

                return map;

            }

        }

        public sealed class Parameter
        {

            public Parameter(ParameterType Affinity)
            {
                this.Affinity = Affinity;
                this.IsNull = true;
            }

            public Parameter(TableExpression Value)
            {
                this.Table = Value;
                this.Affinity = ParameterType.Table;
                this.IsNull = false;
            }

            public Parameter(MatrixExpression Value)
            {
                this.Matrix = Value;
                this.Affinity = ParameterType.Matrix;
                this.IsNull = false;
            }

            public Parameter(ScalarExpressionCollection Value)
            {
                this.Record = Value;
                this.Affinity = ParameterType.Record;
                this.IsNull = false;
            }

            public Parameter(ScalarExpression Value)
            {
                this.Scalar = Value;
                this.Affinity = ParameterType.Scalar;
                this.IsNull = false;
            }

            public bool IsNull
            {
                get;
                private set;
            }

            public int HeapRef
            {
                get;
                set;
            } 

            public ParameterType Affinity
            {
                get;
                private set;
            }

            public TableExpression Table
            {
                get;
                private set;
            }

            public MatrixExpression Matrix
            {
                get;
                private set;
            }

            public ScalarExpressionCollection Record
            {
                get;
                private set;
            }

            public ScalarExpression Scalar
            {
                get;
                private set;
            }

        }

        protected string _Name;
        protected string _InfoString;
        protected Heap<Parameter> _Parameters;
        protected Heap<ParameterPointer> _Map;

        public ActionExpressionParameterized(Host Host, ActionExpression Parent, string Name, string MapString, string InfoString)
            : base(Host, Parent)
        {

            this._Name = Name;
            this._InfoString = InfoString;
            this._Parameters = new Heap<Parameter>();
            this._Map = ParameterPointer.ParseMap(MapString);

        }

        public ActionExpressionParameterized(Host Host, ActionExpression Parent, string Name, string MapString)
            : this(Host, Parent, Name, MapString, Name)
        {
        }

        // Properties //
        public string Name
        {
            get { return this._Name; }
        }

        public string InfoString
        {
            get { return this._InfoString; }
        }

        public string ParameterInfo
        {
            get
            {
                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < this._Map.Count; i++)
                {
                    string t = this._Map[i].ToString();
                    sb.AppendLine(t);
                }
                return sb.ToString();

            }
        }

        // Appendancies //
        public void AddParameter(string Name, Parameter Value)
        {
            this._Parameters.Allocate(Name, Value);
        }

        public void AddParameter(Parameter Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterType.Scalar)
                throw new Exception(string.Format("Parameter '{0}' must be a scalar", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            this._Parameters.Allocate(p.Name, Value);
        }

        public void AddParameter(string Name, ScalarExpression Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(ScalarExpression Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterType.Scalar)
                throw new Exception(string.Format("Parameter '{0}' must be a scalar", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter(ParameterType.Scalar);
            this._Parameters.Allocate(p.Name, x);
        }

        public void AddParameter(string Name, ScalarExpressionCollection Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(ScalarExpressionCollection Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterType.Record)
                throw new Exception(string.Format("Parameter '{0}' must be a record", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter(ParameterType.Record);
            this._Parameters.Allocate(p.Name, x);
        }

        public void AddParameter(string Name, MatrixExpression Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(MatrixExpression Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterType.Matrix)
                throw new Exception(string.Format("Parameter '{0}' must be a matrix", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter(ParameterType.Matrix);
            this._Parameters.Allocate(p.Name, x);
        }

        public void AddParameter(string Name, TableExpression Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(TableExpression Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterType.Table)
                throw new Exception(string.Format("Parameter '{0}' must be a table", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter(ParameterType.Table);
            this._Parameters.Allocate(p.Name, x);
        }

        // Other //
        public void CheckRequired()
        {
            
            for (int i = 0; i < this._Map.Count; i++)
            {
                
                if (this._Map[i].IsRequired && !this._Parameters.Exists(this._Map.Name(i)))
                {
                    throw new Exception(string.Format("'{0}' is missing parameter '{1}'", this.Name, this._Map.Name(i)));
                }
                else if (!this._Map[i].IsRequired && !this._Parameters.Exists(this._Map.Name(i)))
                {
                    ParameterPointer pp = this._Map[i];
                    this._Parameters.Allocate(pp.Name, new Parameter(pp.Affinity));
                }

            }

        }

    }

}
