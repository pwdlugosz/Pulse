﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions;

namespace Pulse.Expressions.ActionExpressions
{

    public abstract class ActionExpressionParameterized : ActionExpression
    {

        public sealed class ParameterPointer
        {

            public ParameterPointer(string Name, ParameterAffinity Affinity, bool IsRequired)
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

            public ParameterAffinity Affinity
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
                ParameterAffinity Affinity = ParameterAffinity.Scalar;
                if (vars[1] == "S")
                    Affinity = ParameterAffinity.Scalar;
                else if (vars[1] == "R")
                    Affinity = ParameterAffinity.Record;
                else if (vars[1] == "M")
                    Affinity = ParameterAffinity.Matrix;
                else if (vars[1] == "T")
                    Affinity = ParameterAffinity.Table;
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
            if (p.Affinity != Value.Affinity)
                throw new Exception(string.Format("Parameter '{0}' must be a '{1}'", p.Name, p.Affinity));
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
            if (p.Affinity != ParameterAffinity.Scalar)
                throw new Exception(string.Format("Parameter '{0}' must be a scalar", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter();
            this._Parameters.Allocate(p.Name, x);
        }

        public void AddParameter(string Name, RecordExpression Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(RecordExpression Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterAffinity.Record)
                throw new Exception(string.Format("Parameter '{0}' must be a record", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter();
            this._Parameters.Allocate(p.Name, x);
        }

        public void AddParameter(string Name, MatrixExpression Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(MatrixExpression Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterAffinity.Matrix)
                throw new Exception(string.Format("Parameter '{0}' must be a matrix", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter();
            this._Parameters.Allocate(p.Name, x);
        }

        public void AddParameter(string Name, TableExpression Value)
        {
            this._Parameters.Allocate(Name, new Parameter(Value));
        }

        public void AddParameter(TableExpression Value)
        {
            ParameterPointer p = this._Map[this._Parameters.Count];
            if (p.Affinity != ParameterAffinity.Table)
                throw new Exception(string.Format("Parameter '{0}' must be a table", p.Name));
            if (Value == null && p.IsRequired)
                throw new Exception(string.Format("Parameter '{0}' is required", p.Name));
            Parameter x = new Parameter(Value);
            if (Value == null)
                x = new Parameter();
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
                    this._Parameters.Allocate(pp.Name, new Parameter());
                }

            }

        }

    }

}
