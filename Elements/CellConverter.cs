using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Elements
{

    /// <summary>
    /// Support for converting cells to differnt cell types
    /// </summary>
    public static class CellConverter
    {

        /// <summary>
        /// Casts a cell to a given affinity
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Affinity"></param>
        /// <returns></returns>
        public static Cell Cast(Cell Value, CellAffinity Affinity)
        {

            switch (Affinity)
            {
                case CellAffinity.BOOL:
                    return ToBOOL(Value);
                case CellAffinity.DATE:
                    return ToDATE(Value);
                case CellAffinity.BYTE:
                    return ToBYTE(Value);
                case CellAffinity.SHORT:
                    return ToSHORT(Value);
                case CellAffinity.INT:
                    return ToINT(Value);
                case CellAffinity.LONG:
                    return ToLONG(Value);
                case CellAffinity.FLOAT:
                    return ToFLOAT(Value);
                case CellAffinity.DOUBLE:
                    return ToDOUBLE(Value);
                case CellAffinity.BLOB:
                    return ToBLOB(Value);
                case CellAffinity.TEXT:
                    return ToTEXT(Value);
                case CellAffinity.STRING:
                    return ToSTRING(Value);
            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinity));

        }

        /// <summary>
        /// Converts a cell value to a bool cell
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBOOL(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BOOL)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBOOL;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullBOOL;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullBOOL;
    
            switch (Value.AFFINITY)
            {

                case CellAffinity.BYTE:
                    if (Value.BYTE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.SHORT:
                    if (Value.SHORT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.INT:
                    if (Value.INT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.LONG:
                    if (Value.LONG != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.FLOAT:
                    if (Value.FLOAT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.DOUBLE:
                    if (Value.DOUBLE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length == 0)
                        return CellValues.NullBOOL;
                    if (Value.BLOB[0] != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    if (StringComparer.OrdinalIgnoreCase.Compare(Value.STRING, Cell.TRUE_STRING) == 0)
                        Value.BOOL = true;
                    else if (StringComparer.OrdinalIgnoreCase.Compare(Value.STRING, Cell.FALSE_STRING) == 0)
                        Value.BOOL = false;
                    else
                        return CellValues.NullBOOL;
                    break;

            }

            return CellValues.NullBOOL;

        }

        /// <summary>
        /// Converts a cell to a byte value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBYTE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BYTE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBYTE;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullBYTE;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullBYTE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((byte)(Value.BOOL ? 1 : 0));
                case CellAffinity.SHORT:
                    return new Cell((byte)(Value.SHORT & byte.MaxValue));
                case CellAffinity.INT:
                    return new Cell((byte)(Value.INT & byte.MaxValue));
                case CellAffinity.DATE:
                case CellAffinity.LONG:
                    return new Cell((byte)(Value.LONG & byte.MaxValue));
                case CellAffinity.FLOAT:
                    return new Cell((byte)(Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell((byte)(Value.DOUBLE));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length == 0)
                        return CellValues.NullBYTE;
                    return new Cell(Value.BLOB[1]);
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    byte b = 0;
                    if (byte.TryParse(Value.STRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullBYTE;
            }

            return CellValues.NullBYTE;

        }

        /// <summary>
        /// Converts a value to a short
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToSHORT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.SHORT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullSHORT;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullSHORT;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullSHORT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((short)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((short)(Value.BYTE));
                case CellAffinity.INT:
                    return new Cell((short)(Value.INT & short.MaxValue));
                case CellAffinity.DATE:
                case CellAffinity.LONG:
                    return new Cell((short)(Value.LONG & short.MaxValue));
                case CellAffinity.FLOAT:
                    return new Cell((short)(Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell((short)(Value.DOUBLE));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length < 2)
                        return CellValues.NullSHORT;
                    return new Cell(BitConverter.ToInt16(Value.BLOB, 0));
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    short b = 0;
                    if (short.TryParse(Value.STRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullSHORT;
            }

            return CellValues.NullSHORT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToINT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.INT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullINT;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullINT;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullINT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((int)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((int)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((int)(Value.SHORT));
                case CellAffinity.DATE:
                case CellAffinity.LONG:
                    return new Cell((int)(Value.LONG & int.MaxValue));
                case CellAffinity.FLOAT:
                    return new Cell((int)(Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell((int)(Value.DOUBLE));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length < 4)
                        return CellValues.NullINT;
                    return new Cell(BitConverter.ToInt32(Value.BLOB, 0));
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    int b = 0;
                    if (int.TryParse(Value.STRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullINT;
            }

            return CellValues.NullINT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToLONG(Cell Value)
        {

            if (Value.Affinity == CellAffinity.LONG)
                return Value;

            if (Value.IsNull)
                return CellValues.NullLONG;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullLONG;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullLONG;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((long)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((long)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((long)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((long)(Value.INT));
                case CellAffinity.DATE:
                    return new Cell((long)(Value.LONG));
                case CellAffinity.FLOAT:
                    return new Cell((long)(Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell((long)(Value.DOUBLE));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length < 8)
                        return CellValues.NullLONG;
                    return new Cell(BitConverter.ToInt64(Value.BLOB, 0));
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    long b = 0;
                    if (long.TryParse(Value.STRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullLONG;
            }

            return CellValues.NullLONG;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToFLOAT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.FLOAT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullFLOAT;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullFLOAT;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullFLOAT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((float)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((float)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((float)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((float)(Value.INT));
                case CellAffinity.DATE:
                case CellAffinity.LONG:
                    return new Cell((float)(Value.LONG));
                case CellAffinity.FLOAT:
                    return new Cell((float)(Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell((float)(Value.DOUBLE));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length < 4)
                        return CellValues.NullFLOAT;
                    return new Cell(BitConverter.ToSingle(Value.BLOB, 0));
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    float b = 0;
                    if (float.TryParse(Value.STRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullFLOAT;
            }

            return CellValues.NullFLOAT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToDOUBLE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.DOUBLE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullDOUBLE;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullDOUBLE;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullDOUBLE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((double)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((double)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((double)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((double)(Value.INT));
                case CellAffinity.DATE:
                case CellAffinity.LONG:
                    return new Cell((double)(Value.LONG));
                case CellAffinity.FLOAT:
                    return new Cell((double)(Value.FLOAT));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length < 8)
                        return CellValues.NullDOUBLE;
                    return new Cell(BitConverter.ToDouble(Value.BLOB, 0));
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    double b = 0;
                    if (double.TryParse(Value.STRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullDOUBLE;
            }

            return CellValues.NullDOUBLE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToDATE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.DATE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullDATE;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullDATE;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullDATE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    return new Cell(DateTime.FromBinary((long)Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell(DateTime.FromBinary((long)Value.SHORT));
                case CellAffinity.INT:
                    return new Cell(DateTime.FromBinary((long)Value.INT));
                case CellAffinity.LONG:
                    return new Cell(DateTime.FromBinary((long)Value.LONG));
                case CellAffinity.FLOAT:
                    return new Cell(DateTime.FromBinary((long)Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell(DateTime.FromBinary((long)Value.DOUBLE));
                case CellAffinity.BLOB:
                    if (Value.BLOB.Length < 8)
                        return CellValues.NullDATE;
                    return new Cell(DateTime.FromBinary(BitConverter.ToInt64(Value.BLOB, 0)));
                case CellAffinity.TEXT:
                case CellAffinity.STRING:
                    return CellParser.DateParse(Value.STRING);
            }

            return CellValues.NullDATE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToBLOB(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BLOB)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBLOB;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullBLOB;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? new byte[1] { 1 } : new byte[1] { 0 });
                case CellAffinity.BYTE:
                    return new Cell(new byte[1] { Value.BYTE });
                case CellAffinity.SHORT:
                    return new Cell(BitConverter.GetBytes(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell(BitConverter.GetBytes(Value.INT));
                case CellAffinity.DATE:
                case CellAffinity.LONG:
                    return new Cell(BitConverter.GetBytes(Value.LONG));
                case CellAffinity.FLOAT:
                    return new Cell(BitConverter.GetBytes(Value.FLOAT));
                case CellAffinity.DOUBLE:
                    return new Cell(BitConverter.GetBytes(Value.DOUBLE));
                case CellAffinity.TEXT:
                    return new Cell(System.Text.ASCIIEncoding.UTF8.GetBytes(Value.STRING));
                case CellAffinity.STRING:
                    return new Cell(System.Text.ASCIIEncoding.Unicode.GetBytes(Value.STRING));

            }

            return CellValues.NullBLOB;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToTEXT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.TEXT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullTEXT;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullTEXT;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullTEXT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING, true);
                case CellAffinity.BYTE:
                    return new Cell(Value.BYTE.ToString(), true);
                case CellAffinity.SHORT:
                    return new Cell(Value.SHORT.ToString(), true);
                case CellAffinity.INT:
                    return new Cell(Value.INT.ToString(), true);
                case CellAffinity.LONG:
                    return new Cell(Value.LONG.ToString(), true);
                case CellAffinity.DATE:
                    return new Cell(Value.DATE.ToString(), true);
                case CellAffinity.FLOAT:
                    return new Cell(Value.FLOAT.ToString(), true);
                case CellAffinity.DOUBLE:
                    return new Cell(Value.DOUBLE.ToString(), true);
                case CellAffinity.BLOB:
                    return new Cell(System.Text.ASCIIEncoding.UTF8.GetString(Value.BLOB, 0, Value.BLOB.Length), true);
                case CellAffinity.STRING:
                    return new Cell(Value.STRING, true);
            }

            return CellValues.NullTEXT;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToSTRING(Cell Value)
        {

            if (Value.Affinity == CellAffinity.STRING)
                return Value;

            if (Value.IsNull)
                return CellValues.NullSTRING;

            if (Value.BLOB == null && Value.AFFINITY == CellAffinity.BLOB)
                return CellValues.NullSTRING;

            if ((Value.STRING == null) && (Value.AFFINITY == CellAffinity.STRING || Value.AFFINITY == CellAffinity.TEXT))
                return CellValues.NullSTRING;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING, false);
                case CellAffinity.BYTE:
                    return new Cell(Value.BYTE.ToString(), false);
                case CellAffinity.SHORT:
                    return new Cell(Value.SHORT.ToString(), false);
                case CellAffinity.INT:
                    return new Cell(Value.INT.ToString(), false);
                case CellAffinity.LONG:
                    return new Cell(Value.LONG.ToString(), false);
                case CellAffinity.DATE:
                    return new Cell(Value.DATE.ToString(), false);
                case CellAffinity.FLOAT:
                    return new Cell(Value.FLOAT.ToString(), false);
                case CellAffinity.DOUBLE:
                    return new Cell(Value.DOUBLE.ToString(), false);
                case CellAffinity.BLOB:
                    return new Cell(System.Text.ASCIIEncoding.Unicode.GetString(Value.BLOB, 0, Value.BLOB.Length / 2), false);
                case CellAffinity.TEXT:
                    return new Cell(Value.STRING, false);
            }

            return CellValues.NullSTRING;


        }


    }

}
