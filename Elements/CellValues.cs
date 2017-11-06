using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Elements
{

    /// <summary>
    /// Support for special cell values
    /// </summary>
    public static class CellValues
    {

        public static Cell Null(CellAffinity Affinty)
        {

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return NullBYTE;
                case CellAffinity.SHORT:
                    return NullSHORT;
                case CellAffinity.INT:
                    return NullINT;
                case CellAffinity.LONG:
                    return NullLONG;
                case CellAffinity.FLOAT:
                    return NullFLOAT;
                case CellAffinity.DOUBLE:
                    return NullDOUBLE;
                case CellAffinity.DATE:
                    return NullDATE;
                case CellAffinity.BLOB:
                    return NullBLOB;
                case CellAffinity.TEXT:
                    return NullTEXT;
                case CellAffinity.STRING:
                    return NullSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Zero(CellAffinity Affinty)
        {

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return ZeroBYTE;
                case CellAffinity.SHORT:
                    return ZeroSHORT;
                case CellAffinity.INT:
                    return ZeroINT;
                case CellAffinity.LONG:
                    return ZeroLONG;
                case CellAffinity.FLOAT:
                    return ZeroFLOAT;
                case CellAffinity.DOUBLE:
                    return ZeroDOUBLE;
                case CellAffinity.DATE:
                    return ZeroDATE;
                case CellAffinity.BLOB:
                    return ZeroBLOB;
                case CellAffinity.TEXT:
                    return ZeroTEXT;
                case CellAffinity.STRING:
                    return ZeroSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell One(CellAffinity Affinty)
        {

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return OneBYTE;
                case CellAffinity.SHORT:
                    return OneSHORT;
                case CellAffinity.INT:
                    return OneINT;
                case CellAffinity.LONG:
                    return OneLONG;
                case CellAffinity.FLOAT:
                    return OneFLOAT;
                case CellAffinity.DOUBLE:
                    return OneDOUBLE;
                case CellAffinity.DATE:
                    return OneDATE;
                case CellAffinity.BLOB:
                    return OneBLOB;
                case CellAffinity.TEXT:
                    return OneTEXT;
                case CellAffinity.STRING:
                    return OneSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Min(CellAffinity Affinty)
        {

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return MinBYTE;
                case CellAffinity.SHORT:
                    return MinSHORT;
                case CellAffinity.INT:
                    return MinINT;
                case CellAffinity.LONG:
                    return MinLONG;
                case CellAffinity.FLOAT:
                    return MinFLOAT;
                case CellAffinity.DOUBLE:
                    return MinDOUBLE;
                case CellAffinity.DATE:
                    return MinDATE;
                case CellAffinity.BLOB:
                    return MinBLOB;
                case CellAffinity.TEXT:
                    return MinTEXT;
                case CellAffinity.STRING:
                    return MinSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Max(CellAffinity Affinty)
        {

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return MaxBYTE;
                case CellAffinity.SHORT:
                    return MaxSHORT;
                case CellAffinity.INT:
                    return MaxINT;
                case CellAffinity.LONG:
                    return MaxLONG;
                case CellAffinity.FLOAT:
                    return MaxFLOAT;
                case CellAffinity.DOUBLE:
                    return MaxDOUBLE;
                case CellAffinity.DATE:
                    return MaxDATE;
                case CellAffinity.BLOB:
                    return MaxBLOB;
                case CellAffinity.TEXT:
                    return MaxTEXT;
                case CellAffinity.STRING:
                    return MaxSTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Empty(CellAffinity Affinty)
        {

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return EmptyBYTE;
                case CellAffinity.SHORT:
                    return EmptySHORT;
                case CellAffinity.INT:
                    return EmptyINT;
                case CellAffinity.LONG:
                    return EmptyLONG;
                case CellAffinity.FLOAT:
                    return EmptyFLOAT;
                case CellAffinity.DOUBLE:
                    return EmptyDOUBLE;
                case CellAffinity.DATE:
                    return EmptyDATE;
                case CellAffinity.BLOB:
                    return EmptyBLOB;
                case CellAffinity.TEXT:
                    return EmptyTEXT;
                case CellAffinity.STRING:
                    return EmptySTRING;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        // BOOL
        public static Cell NullBOOL = new Cell(CellAffinity.BOOL);
        public static Cell ZeroBOOL = new Cell(false);
        public static Cell OneBOOL = new Cell(true);
        public static Cell MinBOOL = new Cell(false);
        public static Cell MaxBOOL = new Cell(true);
        public static Cell EmptyBOOL = new Cell(false);

        // BYTE
        public static Cell NullBYTE = new Cell(CellAffinity.BYTE);
        public static Cell ZeroBYTE = new Cell((byte)0);
        public static Cell OneBYTE = new Cell((byte)1);
        public static Cell MinBYTE = new Cell(byte.MinValue);
        public static Cell MaxBYTE = new Cell(byte.MaxValue);
        public static Cell EmptyBYTE = new Cell((byte)0);

        // SHORT
        public static Cell NullSHORT = new Cell(CellAffinity.SHORT);
        public static Cell ZeroSHORT = new Cell((short)0);
        public static Cell OneSHORT = new Cell((short)1);
        public static Cell MinSHORT = new Cell(short.MinValue);
        public static Cell MaxSHORT = new Cell(short.MaxValue);
        public static Cell EmptySHORT = new Cell((short)0);

        // INT
        public static Cell NullINT = new Cell(CellAffinity.INT);
        public static Cell ZeroINT = new Cell((int)0);
        public static Cell OneINT = new Cell((int)1);
        public static Cell MinINT = new Cell(int.MinValue);
        public static Cell MaxINT = new Cell(int.MaxValue);
        public static Cell EmptyINT = new Cell((int)0);

        // LONG
        public static Cell NullLONG = new Cell(CellAffinity.LONG);
        public static Cell ZeroLONG = new Cell((long)0);
        public static Cell OneLONG = new Cell((long)1);
        public static Cell MinLONG = new Cell(long.MinValue);
        public static Cell MaxLONG = new Cell(long.MaxValue);
        public static Cell EmptyLONG = new Cell((long)0);

        // FLOAT
        public static Cell NullFLOAT = new Cell(CellAffinity.FLOAT);
        public static Cell ZeroFLOAT = new Cell((float)0);
        public static Cell OneFLOAT = new Cell((float)1);
        public static Cell MinFLOAT = new Cell(float.MinValue);
        public static Cell MaxFLOAT = new Cell(float.MaxValue);
        public static Cell EmptyFLOAT = new Cell((float)0);

        // DOUBLE
        public static Cell NullDOUBLE = new Cell(CellAffinity.DOUBLE);
        public static Cell ZeroDOUBLE = new Cell((double)0);
        public static Cell OneDOUBLE = new Cell((double)1);
        public static Cell MinDOUBLE = new Cell(double.MinValue);
        public static Cell MaxDOUBLE = new Cell(double.MaxValue);
        public static Cell EmptyDOUBLE = new Cell((double)0);

        // DATE
        public static Cell NullDATE = new Cell(CellAffinity.DATE);
        public static Cell ZeroDATE = new Cell(DateTime.MinValue);
        public static Cell OneDATE = new Cell(DateTime.MaxValue);
        public static Cell MinDATE = new Cell(DateTime.MinValue);
        public static Cell MaxDATE = new Cell(DateTime.MaxValue);
        public static Cell EmptyDATE = new Cell(DateTime.MinValue);

        // BLOB
        public static Cell NullBLOB = new Cell(CellAffinity.BLOB);
        public static Cell ZeroBLOB = new Cell(new byte[1] { 0 });
        public static Cell OneBLOB = new Cell(new byte[1] { 1 });
        public static Cell MinBLOB = new Cell(new byte[0] { });
        public static Cell MaxBLOB = new Cell(System.Text.ASCIIEncoding.UTF8.GetBytes(new string(char.MaxValue, 2048)));
        public static Cell EmptyBLOB = new Cell(new byte[0] { });

        // STRING
        public static Cell NullSTRING = new Cell(CellAffinity.STRING);
        public static Cell ZeroSTRING = new Cell("ZERO");
        public static Cell OneSTRING = new Cell("ONE");
        public static Cell MinSTRING = new Cell("");
        public static Cell MaxSTRING = new Cell(new string(char.MaxValue, 4096));
        public static Cell EmptySTRING = new Cell("");

        // TEXT
        public static Cell NullTEXT = new Cell(CellAffinity.TEXT);
        public static Cell ZeroTEXT = new Cell(("ZERO"));
        public static Cell OneTEXT = new Cell("ONE");
        public static Cell MinTEXT = new Cell("");
        public static Cell MaxTEXT = new Cell(new string((char)byte.MaxValue, 4096));
        public static Cell EmptyTEXT = new Cell("");

        // Special Values //
        public static Cell Pi = new Cell(Math.PI);
        public static Cell E = new Cell(Math.E);
        public static Cell Epsilon = new Cell(double.Epsilon);
        public static Cell True = new Cell(true);
        public static Cell False = new Cell(false);


    }


}
