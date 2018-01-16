using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pulse.Expressions.ScalarExpressions;
using Pulse.Expressions.MatrixExpressions;
using Pulse.Expressions.RecordExpressions;
using Pulse.Expressions.TableExpressions;
using Pulse.Expressions.ActionExpressions;
using Pulse.Expressions;
using Pulse.Tables;
using Pulse.Elements;
using Pulse.Elements.Structures;

namespace Pulse.Libraries
{

    /// <summary>
    /// Represents a base class for all libraries
    /// </summary>
    public abstract class Library 
    {

        protected Host _Host;
        protected string _Name;

        public Library(Host Host, string Name)
        {
            this._Host = Host;
            this._Name = Name;
        }

        /// <summary>
        /// The name of the library
        /// </summary>
        public string Name
        {
            get { return this._Name; }
        }

        /// <summary>
        /// The library's host
        /// </summary>
        public Host Host
        {
            get { return this._Host; }
        }

        /// <summary>
        /// Shuts down the library
        /// </summary>
        public virtual void ShutDown()
        {
            // do something
        }

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool ScalarFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract ScalarExpressionFunction ScalarFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool MatrixFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract MatrixExpressionFunction MatrixFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool RecordFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract RecordExpressionFunction RecordFunctionLookup(string Name);

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool TableFunctionExists(string Name);

        /// <summary>
        /// Gets a function
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract TableExpressionFunction TableFunctionLookup(string Name);

        /// <summary>
        /// Checks if an action exists
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract bool ActionExists(string Name);

        /// <summary>
        /// Gets an action
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public abstract ActionExpressionParameterized ActionLookup(string Name);

        /// <summary>
        /// Represents the base library
        /// </summary>
        //public sealed class BaseLibrary : Library
        //{

        //    private ScalarExpressionFunction.BaseLibraryFunctions _x;

        //    public BaseLibrary(Host Host)
        //        :base(Host, Host.GLOBAL)
        //    {
        //        this._x = new ScalarExpressionFunction.BaseLibraryFunctions(Host);
        //    }

        //    public override bool ActionExists(string Name)
        //    {
        //        return false;
        //    }

        //    public override ActionExpressionParameterized ActionLookup(string Name)
        //    {
        //        return null;
        //    }

        //    public override bool ScalarFunctionExists(string Name)
        //    {
        //        return this._x.Exists(Name);
        //    }

        //    public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        //    {
        //        return this._x.Lookup(Name);
        //    }

        //}

    }

    public sealed class BaseLibrary : Library
    {

        public const string SUBSTR = "SUBSTR";
        public const string REPLACE = "REPLACE";
        public const string FIND = "FIND";
        public const string TRIM = "TRIM";
        public const string LENGTHOF = "LENGTHOF";
        public const string TYPEOF = "TYPEOF";
        public const string SIZEOF = "SIZEOF";
        public const string NAMEOF = "NAMEOF";
        public const string ROW_COUNT = "ROW_COUNT";
        public const string COLUMN_COUNT = "COLUMN_COUNT";
        public const string GUID = "GUID";
        public const string SSUM = "SSUM";
        public const string SMIN = "SMIN";
        public const string SMAX = "SMAX";
        public const string COALESCE = "COALESCE";
        public const string DENSEA = "DENSEA";
        public const string DENSEB = "DENSEB";
        public const string DENSE = "DENSE";
        public const string MATCH = "MATCH";
        private static readonly string[] ScalarFunctionNames = { SUBSTR, REPLACE, FIND, TRIM, LENGTHOF, TYPEOF, SIZEOF, NAMEOF, ROW_COUNT, COLUMN_COUNT, GUID, SSUM, SMIN, SMAX, COALESCE, DENSEA, DENSEB, DENSE, MATCH };

        public const string SPLIT = "SPLIT";
        public const string SPLICE = "SPLICE";
        public const string BYTES = "BYTES";
        public const string CHARS = "CHARS";
        private static readonly string[] MatrixFunctionNames = { SPLIT, SPLICE, BYTES, CHARS };

        private static readonly string[] RecordFunctionNames = { };

        private static readonly string[] TableFunctionNames = { };

        private static readonly string[] ActionNames = { };

        public BaseLibrary(Host Host)
            : base(Host, Host.GLOBAL)
        {
        }

        // Methods //
        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool RecordFunctionExists(string Name)
        {
            return RecordFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TableFunctionExists(string Name)
        {
            return TableFunctionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool ActionExists(string Name)
        {
            return ActionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case SUBSTR: return new sfSUBSTR(this._Host);
                case REPLACE: return new sfREPLACE(this._Host);
                case FIND: return new sfFIND(this._Host);
                case TRIM: return new sfTRIM(this._Host);
                case LENGTHOF: return new sfLENGTHOF(this._Host);
                case TYPEOF: return new sfTYPEOF(this._Host);
                case SIZEOF: return new sfSIZEOF(this._Host);
                case NAMEOF: return new sfNAMEOF(this._Host);
                case ROW_COUNT: return new sfROW_COUNT(this._Host);
                case COLUMN_COUNT: return new sfCOLUMN_COUNT(this._Host);
                case GUID: return new sfGUID(this._Host);
                case SSUM: return new sfSSUM(this._Host);
                case SMIN: return new sfSMIN(this._Host);
                case SMAX: return new sfSMAX(this._Host);
                case COALESCE: return new sfCOALESCE(this._Host);
                case DENSEA: return new sfDENSEA(this._Host);
                case DENSEB: return new sfDENSEB(this._Host);
                case DENSE: return new sfDENSE(this._Host);
                case MATCH: return new sfMATCH(this._Host);
            }

            throw new Exception(string.Format("Scalar function '{0}' does not exist", Name));
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case SPLIT:
                    return new mfSPLIT();
                case SPLICE:
                    return new mfSPLICE();
                case BYTES:
                    return new mfBYTES();
                case CHARS:
                    return new mfCHARS();
            }
            throw new Exception(string.Format("Matrix function '{0}' does not exist", Name));

        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Record function '{0}' does not exist", Name));
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Table function '{0}' does not exist", Name));
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new Exception(string.Format("Action '{0}' does not exist", Name));
        }

        public override void ShutDown()
        {
            base.ShutDown();
        }

        // Matrix Functions //
        public sealed class mfSPLIT : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfSPLIT()
                : base(null, SPLIT, -2, CellAffinity.CSTRING)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfSPLIT();
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {

                this.CheckParameters();

                bool x = this._Parameters.TrueForAll((y) => { return y.Affinity == ParameterAffinity.Scalar; });
                if (!x)
                    throw new Exception("This function requires all scalars");

                string val = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                string delims = this._Parameters[1].Scalar.Evaluate(Variant).valueCSTRING;
                char escape = (this._Parameters.Count > 2 ? this._Parameters[2].Scalar.Evaluate(Variant).valueCSTRING.FirstOrDefault() : char.MaxValue);

                string[] s = Util.StringUtil.Split(val, delims.ToCharArray(), escape);
                this._Size = val.Length;
                CellMatrix c = new CellMatrix(s.Length, 1, CellAffinity.CSTRING, s.Max((t) => { return t.Length;}));
                for (int i = 0; i < s.Length; i++)
                {
                    c[i, 0] = new Cell(s[i], false);
                }

                return c;

            }

        }

        public sealed class mfSPLICE : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfSPLICE()
                : base(null, SPLICE, 2, CellAffinity.CSTRING)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfSPLICE();
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {

                this.CheckParameters();

                if (this._Parameters[0].Affinity != ParameterAffinity.Scalar || this._Parameters[1].Affinity != ParameterAffinity.Matrix)
                    throw new Exception("This functions requires a scalar and a matrix");

                string s = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING;
                CellMatrix m = this._Parameters[1].Matrix.Evaluate(Variant);
                List<int> indexes = new List<int>();
                foreach (Cell c in m)
                {
                    indexes.Add(c.valueINT);
                }


                string[] q = Util.StringUtil.Splice(s, indexes.ToArray());
                this._Size = s.Length;
                CellMatrix v = new CellMatrix(q.Length, 1, CellAffinity.CSTRING, q.Max((t) => { return t.Length; }));
                for (int i = 0; i < s.Length; i++)
                {
                    v[i, 0] = new Cell(s[i]);
                }

                return v;

            }

        }

        public sealed class mfBYTES : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfBYTES()
                : base(null, BYTES, 1, CellAffinity.BYTE)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfBYTES();
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {

                this.CheckParameters();

                if (this._Parameters[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception("This functions requires a scalar");

                byte[] b = this._Parameters[0].Scalar.Evaluate(Variant).valueBINARY;
                CellMatrix m = new CellMatrix(b.Length, 1, CellAffinity.BYTE, CellSerializer.BYTE_SIZE);
                for (int i = 0; i < b.Length; i++)
                {
                    m[i, 0] = new Cell(b[i]);
                }

                return m;

            }

        }

        public sealed class mfCHARS : MatrixExpressionFunction
        {

            private int _Size = 0;

            public mfCHARS()
                : base(null, CHARS, 1, CellAffinity.CSTRING)
            {

            }

            public override int ReturnSize()
            {
                return this._Size;
            }

            public override MatrixExpression CloneOfMe()
            {
                return new mfCHARS();
            }

            public override CellMatrix Evaluate(FieldResolver Variant)
            {

                this.CheckParameters();

                if (this._Parameters[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception("This functions requires a scalar");

                char[] c = this._Parameters[0].Scalar.Evaluate(Variant).valueCSTRING.ToCharArray();
                CellMatrix m = new CellMatrix(c.Length, 1, CellAffinity.CSTRING, 1);
                for (int i = 0; i < c.Length; i++)
                {
                    m[i, 0] = new Cell(c[i].ToString());
                }

                return m;

            }

        }

        // Scalar Functions //
        public sealed class sfSUBSTR : ScalarExpressionFunction
        {

            public sfSUBSTR(Host Host)
                : base(Host, null, SUBSTR, 3)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSUBSTR(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                if (!(this._Params[0].Affinity == ParameterAffinity.Scalar && this._Params[1].Affinity == ParameterAffinity.Scalar && this._Params[2].Affinity == ParameterAffinity.Scalar))
                    throw new Exception("This function requires all scalars");

                Cell value = this._Params[0].Scalar.Evaluate(Variants);
                Cell start = this._Params[1].Scalar.Evaluate(Variants);
                Cell length = this._Params[2].Scalar.Evaluate(Variants);

                return CellFunctions.Substring(value, start.valueLONG, length.valueLONG);

            }

        }

        public class sfREPLACE : ScalarExpressionFunction
        {

            public sfREPLACE(Host Host)
                : base(Host, null, REPLACE, 3)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfREPLACE(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != this._MaxParamterCount)
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                if (!(this._Params[0].Affinity == ParameterAffinity.Scalar 
                    && (this._Params[1].Affinity == ParameterAffinity.Scalar || this._Params[1].Affinity == ParameterAffinity.Matrix)
                    && this._Params[2].Affinity == ParameterAffinity.Scalar))
                    throw new Exception("This function requires all scalars");

                if (this._Params[1].Affinity == ParameterAffinity.Scalar)
                {
                    Cell value = this._Params[0].Scalar.Evaluate(Variants);
                    Cell pattern = this._Params[1].Scalar.Evaluate(Variants);
                    Cell new_pattern = this._Params[2].Scalar.Evaluate(Variants);
                    return CellFunctions.Replace(value, pattern, new_pattern);
                }
                else
                {
                    Cell value = this._Params[0].Scalar.Evaluate(Variants);
                    CellMatrix pattern = this._Params[1].Matrix.Evaluate(Variants);
                    Cell new_pattern = this._Params[2].Scalar.Evaluate(Variants);
                    
                    foreach(Cell c in pattern)
                    {
                        value = CellFunctions.Replace(value, c, new_pattern);
                    }
                    return value;

                }

            }

        }

        public class sfFIND : ScalarExpressionFunction
        {

            public sfFIND(Host Host)
                : base(Host, null, FIND, -2, CellAffinity.LONG)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfFIND(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count < Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));
                
                if (!(this._Params[0].Affinity == ParameterAffinity.Scalar && this._Params[1].Affinity == ParameterAffinity.Scalar))
                    throw new Exception("This function requires all scalars");

                Cell value = this._Params[0].Scalar.Evaluate(Variants);
                Cell pattern = this._Params[1].Scalar.Evaluate(Variants);
                int start = (this._Params.Count >= 3 ? (int)this._Params[2].Scalar.Evaluate(Variants).valueLONG : 0);

                return CellFunctions.Position(value, pattern, start);

            }

        }

        public class sfTRIM : ScalarExpressionFunction
        {

            public sfTRIM(Host Host)
                : base(Host, null, TRIM, 1)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfTRIM(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));
                if (this._Params[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception("This function requires all scalars");

                Cell x = this._Params[0].Scalar.Evaluate(Variants);
                if (x.IsNull)
                    return new Cell(this.ReturnAffinity());

                return CellFunctions.Trim(x);

            }

        }

        public class sfLENGTHOF : ScalarExpressionFunction
        {

            public sfLENGTHOF(Host Host)
                : base(Host, null, LENGTHOF, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfLENGTHOF(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    return CellSerializer.Length(this._Params[0].Scalar.Evaluate(Variants));
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    return new Cell(this._Params[0].Matrix.Evaluate(Variants).Size);
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    return new Cell(this._Params[0].Record.Columns.Count);
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    return new Cell(this._Params[0].Table.Columns.Count);
                }

                return CellValues.NullINT;

            }

        }

        public class sfTYPEOF : ScalarExpressionFunction
        {

            public sfTYPEOF(Host Host)
                : base(Host, null, SIZEOF, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfTYPEOF(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                int t = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    t = (int)this._Params[0].Scalar.ReturnAffinity();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    t = (int)this._Params[0].Matrix.ReturnAffinity();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    t = 100;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    t = 200;
                }

                return new Cell(t);

            }

        }

        public class sfSIZEOF : ScalarExpressionFunction
        {

            public sfSIZEOF(Host Host)
                : base(Host, null, SIZEOF, 1, CellAffinity.LONG)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSIZEOF(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                long t = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    t = CellSerializer.DiskSize((long)this._Params[0].Scalar.Evaluate(Variants));
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    CellMatrix m = this._Params[0].Matrix.Evaluate(Variants);
                    t = m.Size * m.RowCount * m.ColumnCount;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    t = 100;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    Table q = this._Params[0].Table.Select(Variants);
                    t = q.PageCount * q.PageSize + TableHeader.SIZE;
                }

                return new Cell(t);

            }

        }

        public class sfNAMEOF : ScalarExpressionFunction
        {

            public sfNAMEOF(Host Host)
                : base(Host, null, NAMEOF, 1, CellAffinity.BSTRING)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfNAMEOF(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                string n = null;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    if (this._Params[0].Scalar is ScalarExpressionFunction)
                        n = (this._Params[0].Scalar as ScalarExpressionFunction).Name;
                    else
                        n = this._Params[0].Scalar.BuildAlias();
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    if (this._Params[0].Matrix is MatrixExpressionFunction)
                        n = (this._Params[0].Matrix as MatrixExpressionFunction).FunctionName;
                    else if (this._Params[0].Matrix is MatrixExpressionStoreRef)
                        n = (this._Params[0].Matrix as MatrixExpressionStoreRef).MatrixName;
                    else
                        n = "$UNKNOWN";
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    if (this._Params[0].Record is RecordExpressionStoreRef)
                        n = (this._Params[0].Record as RecordExpressionStoreRef).RecordName;
                    else if (this._Params[0].Record is RecordExpressionFunction)
                        n = (this._Params[0].Record as RecordExpressionFunction).FunctionName;
                    else
                        n = "@UNKNOWN";
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    n = this._Params[0].Table.Alias;
                }

                return new Cell(n);

            }

        }

        public class sfROW_COUNT : ScalarExpressionFunction
        {

            public sfROW_COUNT(Host Host)
                : base(Host, null, ROW_COUNT, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfROW_COUNT(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                long n = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar || this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    n = 1;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    n = this._Params[0].Matrix.Evaluate(Variants).RowCount;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    n = this._Params[0].Table.EstimatedCount;
                }

                return new Cell(n);

            }

        }

        public class sfCOLUMN_COUNT : ScalarExpressionFunction
        {

            public sfCOLUMN_COUNT(Host Host)
                : base(Host, null, COLUMN_COUNT, 1, CellAffinity.INT)
            {
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfCOLUMN_COUNT(this._Host);
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count != Math.Abs(this._MaxParamterCount))
                    throw new ArgumentException(string.Format("'{0}' requires {1} parameters", this.Name, this._MaxParamterCount));

                long n = -1;
                if (this._Params[0].Affinity == ParameterAffinity.Scalar)
                {
                    n = 1;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Matrix)
                {
                    n = this._Params[0].Matrix.Evaluate(Variants).ColumnCount;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Record)
                {
                    n = this._Params[0].Record.Columns.Count;
                }
                else if (this._Params[0].Affinity == ParameterAffinity.Table)
                {
                    n = this._Params[0].Table.Columns.Count;
                }

                return new Cell(n);

            }

        }

        public sealed class sfGUID : ScalarExpressionFunction
        {

            public sfGUID(Host Host)
                : base(Host, null, GUID, 0, CellAffinity.BINARY)
            {
            }

            public override int ReturnSize()
            {
                return 16;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfGUID(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {
                return new Cell(Guid.NewGuid().ToByteArray());
            }

        }

        public sealed class sfSSUM : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfSSUM(Host Host)
                : base(Host, null, SSUM, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSSUM(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Zero(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c += p.Scalar.Evaluate(Variants);
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c += CellMatrix.Sum(p.Matrix.Evaluate(Variants));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfSMIN : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfSMIN(Host Host)
                : base(Host, null, SMIN, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSSUM(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Max(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c = CellFunctions.Min(c, p.Scalar.Evaluate(Variants));
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c = CellFunctions.Min(c, CellMatrix.Min(p.Matrix.Evaluate(Variants)));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfSMAX : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfSMAX(Host Host)
                : base(Host, null, SMAX, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSMAX(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Min(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c = CellFunctions.Max(c, p.Scalar.Evaluate(Variants));
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c = CellFunctions.Max(c, CellMatrix.Min(p.Matrix.Evaluate(Variants)));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfCOALESCE : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfCOALESCE(Host Host)
                : base(Host, null, COALESCE, -127)
            {
            }

            public override int ReturnSize()
            {
                this.SetUpMetaData();
                return this._size;
            }

            public override CellAffinity ReturnAffinity()
            {
                this.SetUpMetaData();
                return base.ReturnAffinity();
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfSMAX(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                this.SetUpMetaData();

                if (this._Params.Count == 0)
                {
                    return CellValues.Null(this.ReturnAffinity());
                }

                Cell c = CellValues.Null(this.ReturnAffinity());

                foreach (Parameter p in this._Params)
                {
                    if (p.Affinity == ParameterAffinity.Scalar)
                        c = CellFunctions.Coalesce(c, p.Scalar.Evaluate(Variants));
                    else if (p.Affinity == ParameterAffinity.Matrix)
                        c = CellFunctions.Coalesce(c, CellMatrix.Min(p.Matrix.Evaluate(Variants)));
                    else
                        throw new Exception("This function requires a scalar or a matrix");
                }

                return c;


            }

            private void SetUpMetaData()
            {

                if (!this._IsRun)
                    return;

                foreach (Parameter p in this._Params)
                {

                    if (p.Affinity == ParameterAffinity.Scalar)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Scalar.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Scalar.ReturnSize());
                        this._IsVolitile = p.Scalar.IsVolatile | this._IsVolitile;
                    }
                    else if (p.Affinity == ParameterAffinity.Matrix)
                    {
                        this._ReturnAffinity = CellAffinityHelper.Highest(this._ReturnAffinity, p.Matrix.ReturnAffinity());
                        this._size = Math.Max(this._size, p.Matrix.ReturnSize());
                    }
                    else
                    {
                        throw new Exception("This function requires a scalar or a matrix");
                    }

                }
                this._IsRun = true;

            }

        }

        public sealed class sfDENSEA : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfDENSEA(Host Host)
                : base(Host, null, DENSEA, 1, CellAffinity.INT)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.INT_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.INT;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEA(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                if (this._Params.Count != 1 || this._Params[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);

                return new Cell(x.INT_A);

            }

        }

        public sealed class sfDENSEB : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfDENSEB(Host Host)
                : base(Host, null, DENSEB, 1, CellAffinity.INT)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.INT_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.INT;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEB(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                if (this._Params.Count != 1 || this._Params[0].Affinity != ParameterAffinity.Scalar)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);

                return new Cell(x.INT_B);

            }

        }

        public sealed class sfDENSE : ScalarExpressionFunction
        {

            private int _size = 0;
            private bool _IsRun = false;
            private bool _IsVolitile = false;

            public sfDENSE(Host Host)
                : base(Host, null, DENSE, 2, CellAffinity.LONG)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.LONG;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEB(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                if (this._Params.Count != 2 || this._Params[0].Affinity != ParameterAffinity.Scalar || this._Params[1].Affinity != ParameterAffinity.Scalar)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);
                Cell y = this._Params[1].Scalar.Evaluate(Variants);

                return new Cell(x.valueINT, y.valueINT);

            }

        }

        public sealed class sfMATCH : ScalarExpressionFunction
        {

            public sfMATCH(Host Host)
                : base(Host, null, MATCH, 2, CellAffinity.LONG)
            {
            }

            public override int ReturnSize()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override CellAffinity ReturnAffinity()
            {
                return CellAffinity.LONG;
            }

            public override ScalarExpression CloneOfMe()
            {
                return new sfDENSEB(this._Host);
            }

            public override bool IsVolatile
            {
                get
                {
                    bool iv = false;
                    foreach (Parameter p in this._Params)
                    {
                        if (p.Affinity == ParameterAffinity.Scalar)
                        {
                            iv = iv | p.Scalar.IsVolatile;
                            if (iv) return true;
                        }
                    }
                    return false;
                }
            }

            public override Cell Evaluate(Expressions.FieldResolver Variants)
            {

                if (this._Params.Count != 2 || this._Params[0].Affinity != ParameterAffinity.Scalar || this._Params[1].Affinity != ParameterAffinity.Matrix)
                    throw new Exception();

                Cell x = this._Params[0].Scalar.Evaluate(Variants);
                CellMatrix y = this._Params[1].Matrix.Evaluate(Variants);

                return y.Match(x);

            }


        }

    }

    public sealed class ChronoLibrary : Library
    {

        public enum DatePart : byte
        {
            Year = 0,
            Month = 1,
            Day = 2,
            Hour = 3,
            Minute = 4,
            Second = 5,
            Milisecond = 6
        }

        public const string NOW = "NOW";
        public const string NOWD = "NOWD";
        public const string NOWT = "NOWT";
        public const string YEAR = "YEAR";
        public const string MONTH = "MONTH";
        public const string DAY = "DAY";
        public const string HOUR = "HOUR";
        public const string MINUTE = "MINUTE";
        public const string SECOND = "SECOND";
        public const string MILISECOND = "MILISECOND";
        public const string BUILD = "BUILD";
        public const string ADD = "ADD";
        public const string DIF = "DIF";
        public const string DOW = "DOW";
        public const string DOY = "DOY";
        public const string DATE_STRING = "DATE_STRING";
        public const string TIME_STRING = "TIME_STRING";

        public readonly string[] ActionNames = { };
        public readonly string[] ScalarNames = { NOW, NOWD, NOWT, YEAR, MONTH, DAY, HOUR, MINUTE, SECOND, MILISECOND, BUILD, ADD, DIF, DOW, DOY, DATE_STRING, TIME_STRING };
        public readonly string[] MatrixNames = { };
        public readonly string[] RecordNames = { NOW, NOWD, NOWT };
        public readonly string[] TableNames = { };

        public ChronoLibrary(Host Host)
            : base(Host, "CHRONO")
        {
        }

        public override bool ActionExists(string Name)
        {
            return ActionNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return ScalarNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool MatrixFunctionExists(string Name)
        {
            return MatrixNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool RecordFunctionExists(string Name)
        {
 	        return RecordNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TableFunctionExists(string Name)
        {
 	        return TableNames.Contains(Name, StringComparer.OrdinalIgnoreCase);
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {

                case NOW:
                    return new sfNOW(this._Host);
                case NOWD:
                    return new sfNOWD(this._Host);
                case NOWT:
                    return new sfNOWT(this._Host);
                case YEAR:
                    return new sfYEAR(this._Host);
                case MONTH:
                    return new sfMONTH(this._Host);
                case DAY:
                    return new sfDAY(this._Host);
                case HOUR:
                    return new sfHOUR(this._Host);
                case MINUTE:
                    return new sfMINUTE(this._Host);
                case SECOND:
                    return new sfSECOND(this._Host);
                case MILISECOND:
                    return new sfMILISECOND(this._Host);
                case BUILD:
                    return new sfBUILD(this._Host);
                case ADD:
                    return new sfADD(this._Host);
                case DIF:
                    return new sfDIF(this._Host);
                case DOW:
                    return new sfDOW(this._Host);
                case DOY:
                    return new sfDOY(this._Host);
                case DATE_STRING:
                    return new sfDATE_STRING(this._Host);
                case TIME_STRING:
                    return new sfTIME_STRING(this._Host);

            }

            throw new Exception(string.Format("Element does not exist '{0}'", Name));

        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Element does not exist '{0}'", Name));
        }

        public sealed class sfNOW : ScalarExpressionFunction
        {

            public sfNOW(Host Host)
                : base(Host, null, NOW, 0, CellAffinity.DATE_TIME)
            {
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(DateTime.Now);
            }

        }

        public sealed class sfNOWD : ScalarExpressionFunction
        {

            public sfNOWD(Host Host)
                : base(Host, null, NOWD, 0, CellAffinity.DATE_TIME)
            {
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(DateTime.Now.Date);
            }

        }

        public sealed class sfNOWT : ScalarExpressionFunction
        {

            public sfNOWT(Host Host)
                : base(Host, null, NOWT, 0, CellAffinity.LONG)
            {
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                return new Cell(DateTime.Now.TimeOfDay.Ticks);
            }

        }

        public sealed class sfYEAR : ScalarExpressionFunction
        {

            public sfYEAR(Host Host)
                : base(Host, null, YEAR, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Year);
            }

        }

        public sealed class sfMONTH : ScalarExpressionFunction
        {

            public sfMONTH(Host Host)
                : base(Host, null, MONTH, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Month);
            }

        }

        public sealed class sfDAY : ScalarExpressionFunction
        {

            public sfDAY(Host Host)
                : base(Host, null, DAY, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Day);
            }

        }

        public sealed class sfHOUR : ScalarExpressionFunction
        {

            public sfHOUR(Host Host)
                : base(Host, null, HOUR, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Hour);
            }

        }

        public sealed class sfMINUTE : ScalarExpressionFunction
        {

            public sfMINUTE(Host Host)
                : base(Host, null, MINUTE, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Minute);
            }

        }

        public sealed class sfSECOND : ScalarExpressionFunction
        {

            public sfSECOND(Host Host)
                : base(Host, null, SECOND, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Second);
            }

        }

        public sealed class sfMILISECOND : ScalarExpressionFunction
        {

            public sfMILISECOND(Host Host)
                : base(Host, null, MILISECOND, 1, CellAffinity.INT)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {
                Cell c = this._ChildNodes[0].Evaluate(Variants);
                if (c.IsNull || c.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullINT;
                return new Cell(c.valueDATE.Millisecond);
            }

        }

        public sealed class sfBUILD : ScalarExpressionFunction
        {

            public sfBUILD(Host Host)
                : base(Host, null, BUILD, -7, CellAffinity.DATE_TIME)
            {
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this._Params.Count == 0)
                    return new Cell(DateTime.Now);

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                int year = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                int month = (this._Params.Count >= 2 ? this._Params[1].Scalar.Evaluate(Variants).valueINT : 1);
                int day = (this._Params.Count >= 3 ? this._Params[2].Scalar.Evaluate(Variants).valueINT : 1);
                int hour = (this._Params.Count >= 4 ? this._Params[3].Scalar.Evaluate(Variants).valueINT : 0);
                int minute = (this._Params.Count >= 5 ? this._Params[4].Scalar.Evaluate(Variants).valueINT : 0);
                int second = (this._Params.Count >= 6 ? this._Params[5].Scalar.Evaluate(Variants).valueINT : 0);
                int milisecond = (this._Params.Count >= 7 ? this._Params[6].Scalar.Evaluate(Variants).valueINT : 0);

                DateTime t = new DateTime(year, month, day, hour, minute, second, milisecond);
                return new Cell(t);

            }

        }

        public sealed class sfADD : ScalarExpressionFunction
        {

            public sfADD(Host Host)
                : base(Host, null, ADD, 3, CellAffinity.DATE_TIME)
            {
                
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                DateTime dt = this._Params[0].Scalar.Evaluate(Variants).valueDATE;
                int val = this._Params[1].Scalar.Evaluate(Variants).valueINT;
                DatePart dp = ChronoLibrary.ImputeDatePart(this._Params[2].Scalar.Evaluate(Variants));

                switch (dp)
                {
                    case DatePart.Year: dt = dt.AddYears(val);
                        break;
                    case DatePart.Month: dt = dt.AddMonths(val);
                        break;
                    case DatePart.Day: dt = dt.AddDays(val);
                        break;
                    case DatePart.Hour: dt = dt.AddHours(val);
                        break;
                    case DatePart.Minute: dt = dt.AddMinutes(val);
                        break;
                    case DatePart.Second: dt = dt.AddSeconds(val);
                        break;
                    case DatePart.Milisecond: dt = dt.AddMilliseconds(val);
                        break;
                }

                return new Cell(dt);

            }


        }

        public sealed class sfDIF : ScalarExpressionFunction
        {

            public sfDIF(Host Host)
                : base(Host, null, DIF, 2, CellAffinity.LONG)
            {

            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                Cell b = this._Params[1].Scalar.Evaluate(Variants);
                if (a.IsNull || b.IsNull || a.Affinity != CellAffinity.DATE_TIME || b.Affinity != CellAffinity.DATE_TIME)
                    return CellValues.NullLONG;
                return new Cell((a.valueDATE - b.valueDATE).Ticks);

            }


        }

        public sealed class sfDOW : ScalarExpressionFunction
        {

            public sfDOW(Host Host)
                : base(Host, null, DOW, 1, CellAffinity.BYTE)
            {

            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullBYTE;

                return new Cell((byte)a.valueDATE.DayOfWeek);

            }

        }

        public sealed class sfDOY : ScalarExpressionFunction
        {

            public sfDOY(Host Host)
                : base(Host, null, DOY, 1, CellAffinity.SHORT)
            {

            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullSHORT;

                return new Cell((short)a.valueDATE.DayOfYear);

            }

        }

        public sealed class sfDATE_STRING : ScalarExpressionFunction
        {

            public sfDATE_STRING(Host Host)
                : base(Host, null, DATE_STRING, 1, CellAffinity.BSTRING)
            {

            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullBSTRING;
                DateTime dt = a.valueDATE;

                return new Cell(string.Format("{0}-{1}-{2}", dt.Year, dt.Month, dt.Day));

            }

        }

        public sealed class sfTIME_STRING : ScalarExpressionFunction
        {

            public sfTIME_STRING(Host Host)
                : base(Host, null, TIME_STRING, 1, CellAffinity.BSTRING)
            {

            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                this.CheckParameters();

                if (!this._Params.TrueForAll((x) => { return x.Affinity == ParameterAffinity.Scalar; }))
                    throw new Exception("This function requies all scalars");

                Cell a = this._Params[0].Scalar.Evaluate(Variants);
                if (a.IsNull)
                    return CellValues.NullBSTRING;
                TimeSpan ts = new TimeSpan();
                if (a.Affinity == CellAffinity.DATE_TIME)
                    ts = a.valueDATE.TimeOfDay;
                else if (CellAffinityHelper.IsIntegral(a.Affinity))
                    ts = new TimeSpan(a.valueLONG);

                return new Cell(ts.ToString(), true);

            }

        }
        
        // --------- Support --------- 
        private static DatePart ImputeDatePart(Cell Parameter)
        {

            if (Parameter.IsNull)
                return DatePart.Milisecond;
            if (Parameter.Affinity == CellAffinity.CSTRING || Parameter.Affinity == CellAffinity.BSTRING)
            {

                switch (Parameter.ToString().ToUpper())
                {
                    
                    case "YEAR":
                    case "YYYY":
                    case "YR":
                    case "YY":
                    case "Y":
                        return DatePart.Year;

                    case "MONTH":
                    case "MNTH":
                    case "MM":
                    case "M":
                        return DatePart.Month;

                    case "DAY":
                    case "DY":
                    case "DD":
                    case "D":
                        return DatePart.Day;

                    case "HOUR":
                    case "HR":
                    case "H":
                        return DatePart.Hour;
                   
                    case "MINUTE":
                    case "MIN":
                    case "MN":
                        return DatePart.Minute;

                    case "SECOND":
                    case "SEC":
                    case "S":
                        return DatePart.Second;

                    case "MILISECOND":
                    case "MILLISECOND":
                    case "MILI":
                    case "MS":
                        return DatePart.Milisecond;


                }

            }
            else if (CellAffinityHelper.IsNumeric(Parameter.Affinity))
            {
                byte b = Parameter.valueBYTE;
                if (b > 6) throw new Exception("DateParts from integral parameters must be between 0 and 6");
                return (DatePart)b;
            }

            return DatePart.Milisecond;


        }


    }

    public sealed class RandomLibrary : Library
    {

        public const string NEXT_BOOL = "NEXT_BOOL";
        public const string NEXT_DATE_TIME = "NEXT_DATE_TIME";
        public const string NEXT_BYTE = "NEXT_BYTE";
        public const string NEXT_SHORT = "NEXT_SHORT";
        public const string NEXT_INT = "NEXT_INT";
        public const string NEXT_LONG = "NEXT_LONG";
        public const string NEXT_SINGLE = "NEXT_SINGLE";
        public const string NEXT_DOUBLE = "NEXT_DOUBLE";
        public const string NEXT_BINARY = "NEXT_BINARY";
        public const string NEXT_BSTRING = "NEXT_BSTRING";
        public const string NEXT_CSTRING = "NEXT_CSTRING";

        public static readonly string[] Scalar_Names = new string[] { NEXT_BOOL, NEXT_DATE_TIME, NEXT_BYTE, NEXT_SHORT, NEXT_INT, NEXT_LONG, NEXT_SINGLE, NEXT_DOUBLE, NEXT_BINARY, NEXT_BSTRING, NEXT_CSTRING };

        public const string SET_SEED = "SET_SEED";

        private RandomCell _Gen;

        public RandomLibrary(Host Host)
            :base(Host, "RANDOM")
        {
            this._Gen = new RandomCell(RandomCell.TruelyRandomSeed());
        }

        public override bool ActionExists(string Name)
        {
            throw new Exception(string.Format("Action does not exist '{0}'", Name));
        }

        public override ActionExpressionParameterized ActionLookup(string Name)
        {
            throw new Exception(string.Format("Action does not exist '{0}'", Name));
        }

        public override bool ScalarFunctionExists(string Name)
        {
            return Scalar_Names.Contains(Name.ToUpper());
        }

        public override ScalarExpressionFunction ScalarFunctionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case NEXT_BOOL: return new sfNextBool(this._Host, this._Gen);
                case NEXT_DATE_TIME: return new sfNextDate(this._Host, this._Gen);
                case NEXT_BYTE: return new sfNextByte(this._Host, this._Gen);
                case NEXT_SHORT: return new sfNextShort(this._Host, this._Gen);
                case NEXT_INT: return new sfNextInt(this._Host, this._Gen);
                case NEXT_LONG: return new sfNextLong(this._Host, this._Gen);
                case NEXT_SINGLE: return new sfNextSingle(this._Host, this._Gen);
                case NEXT_DOUBLE: return new sfNextDouble(this._Host, this._Gen);
                case NEXT_BINARY: return new sfNextBinary(this._Host, this._Gen);
                case NEXT_BSTRING: return new sfNextBString(this._Host, this._Gen);
                case NEXT_CSTRING: return new sfNextCString(this._Host, this._Gen);
            }

            throw new Exception(string.Format("Scalar function does not exist '{0}'", Name));

        }

        public override bool MatrixFunctionExists(string Name)
        {
            throw new Exception(string.Format("Matrix does not exist '{0}'", Name));
        }

        public override MatrixExpressionFunction MatrixFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Matrix does not exist '{0}'", Name));
        }

        public override bool RecordFunctionExists(string Name)
        {
            throw new Exception(string.Format("Record does not exist '{0}'", Name));
        }

        public override RecordExpressionFunction RecordFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Record does not exist '{0}'", Name));
        }

        public override bool TableFunctionExists(string Name)
        {
            throw new Exception(string.Format("Table does not exist '{0}'", Name));
        }

        public override TableExpressionFunction TableFunctionLookup(string Name)
        {
            throw new Exception(string.Format("Table does not exist '{0}'", Name));
        }

        public sealed class sfNextBool : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextBool(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BOOL, -1, CellAffinity.BOOL)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextBool(0.5);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextBool(this._Params[0].Scalar.Evaluate(Variants).valueSINGLE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextDate : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextDate(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_DATE_TIME, -2, CellAffinity.DATE_TIME)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextDate();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDate(DateTime.MinValue, this._Params[0].Scalar.Evaluate(Variants).valueDATE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDate(this._Params[0].Scalar.Evaluate(Variants).valueDATE, this._Params[0].Scalar.Evaluate(Variants).valueDATE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextByte : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextByte(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BYTE, -2, CellAffinity.BYTE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextByte();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextByte(0, this._Params[0].Scalar.Evaluate(Variants).valueBYTE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextByte(this._Params[0].Scalar.Evaluate(Variants).valueBYTE, this._Params[0].Scalar.Evaluate(Variants).valueBYTE);
                }
                throw new Exception("Invalid parameters passed");
                
            }

        }

        public sealed class sfNextShort : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextShort(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_SHORT, -2, CellAffinity.SHORT)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextShort();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextShort(0, this._Params[0].Scalar.Evaluate(Variants).valueSHORT);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextShort(this._Params[0].Scalar.Evaluate(Variants).valueSHORT, this._Params[0].Scalar.Evaluate(Variants).valueSHORT);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextInt : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextInt(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_INT, -2, CellAffinity.INT)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextInt();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextInt(0, this._Params[0].Scalar.Evaluate(Variants).valueINT);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextInt(this._Params[0].Scalar.Evaluate(Variants).valueINT, this._Params[0].Scalar.Evaluate(Variants).valueINT);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextLong : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextLong(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_LONG, -2, CellAffinity.LONG)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextLong();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextLong(0, this._Params[0].Scalar.Evaluate(Variants).valueLONG);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextLong(this._Params[0].Scalar.Evaluate(Variants).valueLONG, this._Params[0].Scalar.Evaluate(Variants).valueLONG);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextSingle : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextSingle(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_SINGLE, -2, CellAffinity.SINGLE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextSingle();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextSingle(0, this._Params[0].Scalar.Evaluate(Variants).valueSINGLE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextSingle(this._Params[0].Scalar.Evaluate(Variants).valueSINGLE, this._Params[0].Scalar.Evaluate(Variants).valueSINGLE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextDouble : ScalarExpressionFunction
        {

            private RandomCell _Gen;

            public sfNextDouble(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_DOUBLE, -2, CellAffinity.DOUBLE)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextDouble();
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDouble(0, this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    return this._Gen.NextDouble(this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE, this._Params[0].Scalar.Evaluate(Variants).valueDOUBLE);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextBinary : ScalarExpressionFunction
        {

            private RandomCell _Gen;
            private int _Size = Schema.DEFAULT_BLOB_SIZE;

            public sfNextBinary(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BINARY, -2, CellAffinity.BINARY)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextBinary(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    return this._Gen.NextBinary(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    byte[] corpus = this._Params[1].Scalar.Evaluate(Variants).valueBINARY;
                    return this._Gen.NextBinary(this._Size, corpus);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextBString : ScalarExpressionFunction
        {

            private RandomCell _Gen;
            private int _Size = Schema.DEFAULT_STRING_SIZE;

            public sfNextBString(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_BSTRING, -2, CellAffinity.BSTRING)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextBString(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    return this._Gen.NextBString(this._Size);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    BString b = this._Params[1].Scalar.Evaluate(Variants).valueBSTRING;
                    return this._Gen.NextBString(this._Size, b);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

        public sealed class sfNextCString : ScalarExpressionFunction
        {

            private RandomCell _Gen;
            private int _Size = Schema.DEFAULT_STRING_SIZE;

            public sfNextCString(Host Host, RandomCell Gen)
                : base(Host, null, NEXT_CSTRING, -2, CellAffinity.CSTRING)
            {
                this._Gen = Gen;
            }

            public override bool IsVolatile
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(FieldResolver Variants)
            {

                if (this.CheckSigniture())
                {
                    return this._Gen.NextCString(this._Size, (int)char.MaxValue);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    return this._Gen.NextCString(this._Size, (int)char.MaxValue);
                }
                else if (this.CheckSigniture(ParameterAffinity.Scalar, ParameterAffinity.Scalar))
                {
                    this._Size = this._Params[0].Scalar.Evaluate(Variants).valueINT;
                    string b = this._Params[1].Scalar.Evaluate(Variants).valueCSTRING;
                    return this._Gen.NextBString(this._Size, b);
                }
                throw new Exception("Invalid parameters passed");

            }

        }

    }

    //public sealed class MathLibrary : Library
    //{

    //    public const string LOG = "LOG";
    //    public const string LOG2 = "LOG2";
    //    public const string LOG10 = "LOG10";
    //    public const string EXP = "EXP";
    //    public const string EXP2 = "EXP2";
    //    public const string EXP10 = "EXP10";
    //    public const string SIN = "SIN";
    //    public const string COS = "COS";
    //    public const string TAN = "TAN";
    //    public const string ASIN = "ASIN";
    //    public const string ACOS = "ACOS";
    //    public const string ATAN = "ATAN";
    //    public const string SINH = "SINH";
    //    public const string COSH = "COSH";
    //    public const string TANH = "TANH";
    //    public const string ASINH = "ASINH";
    //    public const string ACOSH = "ACOSH";
    //    public const string ATANH = "ATANH";
    //    public const string ABS = "ABS";
    //    public const string SIGN = "SIGN";
    //    public const string ROUND = "ROUND";
    //    public const string SQR = "SQR";
    //    public const string SQRT = "SQRT";
    //    public const string LOGIT = "LOGIT";

    //}

}
