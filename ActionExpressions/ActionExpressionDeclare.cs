﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;
using Pulse.TableExpressions;

namespace Pulse.ActionExpressions
{
    
    public class ActionExpressionDeclareScalar : ActionExpression
    {

        private string _LibName;
        private string _VarName;
        private CellAffinity _Type;
        private ScalarExpression _Value;

        public ActionExpressionDeclareScalar(Host Host, ActionExpression Parent, string LibraryName, string VarName, CellAffinity Type, ScalarExpression Value)
            : base(Host, Parent)
        {
            this._LibName = LibraryName;
            this._VarName = VarName;
            this._Type = Type;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {
            this._Host.Libraries[this._LibName].Values.Reallocate(this._VarName, this._Value.Evaluate(Variant));
        }

    }

    public class ActionExpressionDeclareMatrix : ActionExpression
    {

        private string _LibName;
        private string _VarName;
        private CellAffinity _Type;
        private MatrixExpression _Value;

        public ActionExpressionDeclareMatrix(Host Host, ActionExpression Parent, string LibraryName, string VarName, CellAffinity Type, MatrixExpression Value)
            : base(Host, Parent)
        {
            this._LibName = LibraryName;
            this._VarName = VarName;
            this._Type = Type;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {
            this._Host.Libraries[this._LibName].Matrixes.Reallocate(this._VarName, this._Value.Evaluate(Variant));
        }

    }

    public class ActionExpressionDeclareTable : ActionExpression
    {

        private string _LibName;
        private string _VarName;
        private CellAffinity _Type;
        private MatrixExpression _Value;

        public ActionExpressionDeclareTable(Host Host, ActionExpression Parent, string LibraryName, string VarName, CellAffinity Type, MatrixExpression Value)
            : base(Host, Parent)
        {
            this._LibName = LibraryName;
            this._VarName = VarName;
            this._Type = Type;
            this._Value = Value;
        }

        public override void Invoke(FieldResolver Variant)
        {
            this._Host.Libraries[this._LibName].Matrixes.Reallocate(this._VarName, this._Value.Evaluate(Variant));
        }

    }


}
