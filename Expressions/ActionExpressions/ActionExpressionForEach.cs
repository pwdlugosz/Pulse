using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Elements;
using Pulse.Tables;
using Pulse.Expressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.TableExpressions;

namespace Pulse.Expressions.ActionExpressions
{
    
    public class ActionExpressionForEachTable : ActionExpression
    {

        private TableExpression _t;
        private string _a;

        public ActionExpressionForEachTable(Host Host, ActionExpression Parent, TableExpression Data, string Alias)
            : base(Host, Parent)
        {
            this._t = Data;
            this._a = Alias;
        }

        public override void BeginInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => x.BeginInvoke(Variant));
        }

        public override void EndInvoke(FieldResolver Variant)
        {
            this._Children.ForEach((x) => x.EndInvoke(Variant));
        }

        public override void Invoke(FieldResolver Variant)
        {

            Table t = this._t.Select(Variant);
            RecordReader rr = t.OpenReader();

            // Set up the resolver //
            if (!Variant.Local.ExistsRecord(this._a))
                Variant.Local.DeclareRecord(this._a, new AssociativeRecord(t.Columns));

            while (rr.CanAdvance)
            {
                Variant.SetRecord(FieldResolver.LOCAL, this._a, new AssociativeRecord(t.Columns, rr.ReadNext())); 
                this._Children.ForEach((x) => { x.Invoke(Variant); });
            }

            if (this._Host.IsSystemTemp(t))
                this._Host.TableStore.DropTable(t.Key);

        }

        public override FieldResolver CreateResolver()
        {
            FieldResolver f = base.CreateResolver();
            return f;
        }

    }

    public class ActionExpressionForEachMatrixExpression : ActionExpression
    {

        private MatrixExpression _Val;
        private string _Lib;
        private string _Name;
        private bool _Trigger = false;

        public ActionExpressionForEachMatrixExpression(Host Host, ActionExpression Parent, MatrixExpression Enumerator, string VarLib, string VarName)
            : base(Host, Parent)
        {
            this._Val = Enumerator;
            this._Lib = VarLib;
            this._Name = VarName;
        }

        public override void BeginInvoke(FieldResolver Variant)
        {

            if (!Variant.Stores.Exists(this._Lib))
            {
                throw new Exception(string.Format("Object store '{0}' does not exist", this._Lib));
            }
            if (!Variant.Stores[this._Lib].Scalars.Exists(this._Name))
            {
                Variant.Stores[this._Lib].Scalars.Allocate(this._Name, new Cell(this._Val.ReturnAffinity()));
                this._Trigger = true;
            }

        }

        public override void Invoke(FieldResolver Variant)
        {

            CellMatrix m = this._Val.Evaluate(Variant);

            foreach (Cell c in m)
            {

                Variant.Stores[this._Lib].Scalars[this._Name] = c;

                this._Children.ForEach((x) => { x.Invoke(Variant); });

            }

        }

        public override void EndInvoke(FieldResolver Variant)
        {
            if (this._Trigger)
            {
                Variant.Stores[this._Lib].Scalars.Deallocate(this._Name);
                this._Trigger = false;
            }
        }

    }

    public class ActionExpressionForEachMatrix : ActionExpression
    {

        private string _MatLib;
        private string _MatName;
        private string _VarLib;
        private string _VarName;
        private bool _Trigger = false;

        public ActionExpressionForEachMatrix(Host Host, ActionExpression Parent, string MatrixLib, string MatrixName, string VarLib, string VarName)
            : base(Host, Parent)
        {
            this._MatLib = MatrixLib;
            this._MatName = MatrixName;
            this._VarLib = VarLib;
            this._VarName = VarName;
        }

        public override void BeginInvoke(FieldResolver Variant)
        {

            if (!Variant.Stores.Exists(this._VarLib))
            {
                throw new Exception(string.Format("Object store '{0}' does not exist", this._VarLib));
            }
            if (!Variant.Stores[this._VarLib].Scalars.Exists(this._VarName))
            {
                Variant.Stores[this._VarLib].Scalars.Allocate(this._VarName, new Cell(Variant.Stores[this._MatLib].Matrixes[this._MatName].Affinity));
                this._Trigger = true;
            }


        }

        public override void Invoke(FieldResolver Variant)
        {

            CellMatrix m = Variant[this._MatLib].Matrixes[this._MatName];

            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    Cell c = m[i, j];
                    Variant.Stores[this._VarLib].Scalars[this._VarName] = c;
                    this._Children.ForEach((x) => { x.Invoke(Variant); });
                    m[i, j] = Variant.Stores[this._VarLib].Scalars[this._VarName];
                }
            }
            
        }

        public override void EndInvoke(FieldResolver Variant)
        {
            if (this._Trigger)
            {
                Variant.Stores[this._VarLib].Scalars.Deallocate(this._VarName);
                this._Trigger = false;
            }
        }

    }

    //public class ActionExpressionForEachByteInBinary : ActionExpression
    //{
    //}

    //public class ActionExpressionForEachBStringInBString : ActionExpression
    //{
    //}

    //public class ActionExpressionForEachCStringInCString : ActionExpression
    //{
    //}

}
