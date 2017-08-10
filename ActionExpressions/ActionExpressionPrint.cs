using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Pulse.Data;
using Pulse.ScalarExpressions;
using Pulse.MatrixExpressions;
using Pulse.TableExpressions;

namespace Pulse.ActionExpressions
{

    public class ActionExpressionPrintConsole : ActionExpression
    {

        private TableExpression _t;
        private ScalarExpressionCollection _s;
        private MatrixExpression _m;

        public ActionExpressionPrintConsole(Host Host, ActionExpression Parent, TableExpression Expression)
            : base(Host, Parent)
        {
            this._t = Expression;
            this._s = null;
            this._m = null;
        }

        public ActionExpressionPrintConsole(Host Host, ActionExpression Parent, MatrixExpression Expression)
            : base(Host, Parent)
        {
            this._t = null;
            this._s = null;
            this._m = Expression;
        }

        public ActionExpressionPrintConsole(Host Host, ActionExpression Parent, ScalarExpressionCollection Expression)
            : base(Host, Parent)
        {
            this._t = null;
            this._s = Expression;
            this._m = null;
        }

        public override void Invoke(FieldResolver Variant)
        {
            
            if (this._t != null)
            {

                using (RecordReader rr = this._t.Evaluate(Variant).OpenReader())
                {
                    this._Host.IO.WriteLine(rr.Columns.ToNameString(','));
                    while (rr.CanAdvance)
                    {
                        this._Host.IO.WriteLine(rr.ReadNext().ToString(','));
                    }
                }

            }
            else if (this._s != null)
            {
                //this._Host.IO.WriteLine(this._s.Columns.ToNameString(','));
                this._Host.IO.WriteLine(this._s.Evaluate(Variant).ToString(','));
            }
            else if (this._m != null)
            {
                this._Host.IO.WriteLine(this._m.Evaluate(Variant).ToString());
            }
            else
            {
                throw new Exception("Invalid print logic");
            }


        }

    }

    public class ActionExpressionPrintFile : ActionExpression
    {

        private TableExpression _t;
        private ScalarExpressionCollection _s;
        private MatrixExpression _m;
        private Heap<StreamWriter> _sw;
        private ScalarExpression _path;

        public ActionExpressionPrintFile(Host Host, ActionExpression Parent, TableExpression Expression, Heap<StreamWriter> Heap, ScalarExpression Path)
            : base(Host, Parent)
        {
            this._t = Expression;
            this._s = null;
            this._m = null;
            this._sw = Heap;
            this._path = Path;
        }

        public ActionExpressionPrintFile(Host Host, ActionExpression Parent, MatrixExpression Expression, Heap<StreamWriter> Heap, ScalarExpression Path)
            : base(Host, Parent)
        {
            this._t = null;
            this._s = null;
            this._m = Expression;
            this._sw = Heap;
            this._path = Path;
        }

        public ActionExpressionPrintFile(Host Host, ActionExpression Parent, ScalarExpressionCollection Expression, Heap<StreamWriter> Heap, ScalarExpression Path)
            : base(Host, Parent)
        {
            this._t = null;
            this._s = Expression;
            this._m = null;
            this._sw = Heap;
            this._path = Path;
        }

        public override void Invoke(FieldResolver Variant)
        {

            // Get the steam //
            StreamWriter sw = null;
            string key = this._path.Evaluate(Variant).ToString();
            if (this._sw.Exists(key))
            {
                sw = this._sw[key];
            }
            else
            {
                sw = new StreamWriter(key);
                this._sw.Allocate(key, sw);
            }

            // Print //
            if (this._t != null)
            {

                using (RecordReader rr = this._t.Evaluate(Variant).OpenReader())
                {
                    sw.WriteLine(rr.Columns.ToNameString(','));
                    while (rr.CanAdvance)
                    {
                        sw.WriteLine(rr.ReadNext().ToString(','));
                    }
                }

            }
            else if (this._s != null)
            {
                //sw.WriteLine(this._s.Columns.ToNameString(','));
                sw.WriteLine(this._s.Evaluate(Variant).ToString(','));
            }
            else if (this._m != null)
            {
                sw.WriteLine(this._m.Evaluate(Variant).ToString());
            }
            else
            {
                throw new Exception("Invalid print logic");
            }
            sw.Flush();

        }

        public override void EndInvoke(FieldResolver Variant)
        {

            //foreach (KeyValuePair<string, StreamWriter> kv in this._sw.Entries)
            //{
            //    if (kv.Value != null)
            //    {
            //        kv.Value.Flush();
            //        kv.Value.Close();
            //        this._sw[kv.Key] = null;
            //    }
            //}

        }

    }

}
