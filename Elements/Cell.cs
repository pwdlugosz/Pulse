﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Elements
{

    /// <summary>
    /// The basic unit of in memory data
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct Cell : IComparable<Cell>, IComparer<Cell>
    {

        // Cell constants //
        public const String NULL_STRING_TEXT = "@@NULL"; // the null Value text
        public const string HEX_LITERARL = "0x"; // the expected qualifier for a hex string 
        public const int MAX_STRING_LENGTH = 64 * 1024 * 1024; // maximum length of a string, 64k

        // Cell internal statics //
        internal static int NUMERIC_ROUNDER = 5; // used for rounding double values 
        internal static int DATE_FORMAT = 1; // 0 = full date time, 1 = date only, 2 = time only
        internal static string TRUE_STRING = "TRUE";
        internal static string FALSE_STRING = "FALSE";

        // Static common cells //

        #region Runtime_Variables

        /* Offset:      0   1   2   3   4   5   6   7   8   9   10  11  12  13  14  15
         * 
         * NullFlag     OriginalPage
         * Affinity         OriginalPage
         * INT64                OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * DATE                 OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * DOUBLE               OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * BOOL                 OriginalPage
         * STRING                                                       OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * BLOB                                                         OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * INT32A               OriginalPage   OriginalPage   OriginalPage   OriginalPage   
         * INT32B                               OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * ULONG                OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage   OriginalPage
         * 
         */

        // Metadata elements //
        /// <summary>
        /// The cell affinity, offset 9
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(9)]
        internal CellAffinity AFFINITY;

        /// <summary>
        /// The null byte indicator, offset 10
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(10)]
        internal byte NULL;

        // Elements variables //
        /// <summary>
        /// The .Net bool Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal bool BOOL;

        /// <summary>
        /// The .Net short Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte BYTE;

        /// <summary>
        /// The .Net short Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal short SHORT;

        /// <summary>
        /// The .Net int Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int INT;

        /// <summary>
        /// The .Net long Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal long LONG;

        /// <summary>
        /// The .Net float Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal float FLOAT;

        /// <summary>
        /// The .Net double Value, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal double DOUBLE;

        /// <summary>
        /// The .Net DateTime variable, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal DateTime DATE;

        /// <summary>
        /// The .Net string variable, offset 12
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal string STRING;

        /// <summary>
        /// The .Net byte[] variable, offset 12
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal byte[] BLOB;

        // Extended elements //
        /// <summary>
        /// The .Net integer Value at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int INT_A;

        /// <summary>
        /// The .Net integer Value at offset 4
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal int INT_B;

        /// <summary>
        /// The .Net float Value at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal float FLOAT_A;

        /// <summary>
        /// The .Net float Value at offset 4
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal float FLOAT_B;

        /// <summary>
        /// The .Net ulong Value at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal ulong ULONG;

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte B0;

        [System.Runtime.InteropServices.FieldOffset(1)]
        internal byte B1;

        [System.Runtime.InteropServices.FieldOffset(2)]
        internal byte B2;

        [System.Runtime.InteropServices.FieldOffset(3)]
        internal byte B3;

        [System.Runtime.InteropServices.FieldOffset(4)]
        internal byte B4;

        [System.Runtime.InteropServices.FieldOffset(5)]
        internal byte B5;

        [System.Runtime.InteropServices.FieldOffset(6)]
        internal byte B6;

        [System.Runtime.InteropServices.FieldOffset(7)]
        internal byte B7;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a boolean cell
        /// </summary>
        /// <param name="Value">A .Net bool</param>
        public Cell(bool Value)
            : this()
        {
            this.BOOL = Value;
            this.AFFINITY = CellAffinity.BOOL;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 8 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int8</param>
        public Cell(byte Value)
            : this()
        {

            this.BYTE = Value;
            this.AFFINITY = CellAffinity.BYTE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 16 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int16</param>
        public Cell(short Value)
            : this()
        {

            this.SHORT = Value;
            this.AFFINITY = CellAffinity.SHORT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 32 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int32</param>
        public Cell(int Value)
            : this()
        {

            this.INT = Value;
            this.AFFINITY = CellAffinity.INT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit integer cell
        /// </summary>
        /// <param name="Value">A .Net long or int64</param>
        public Cell(long Value)
            : this()
        {

            this.LONG = Value;
            this.AFFINITY = CellAffinity.LONG;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 32 bit numeric cell
        /// </summary>
        /// <param name="Value">A .Net double or Double</param>
        public Cell(float Value)
            : this()
        {
            this.FLOAT = Value;
            this.AFFINITY = CellAffinity.FLOAT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit numeric cell
        /// </summary>
        /// <param name="Value">A .Net double or Double</param>
        public Cell(double Value)
            : this()
        {
            this.DOUBLE = Value;
            this.AFFINITY = CellAffinity.DOUBLE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit date-time cell
        /// </summary>
        /// <param name="Value">A .Net DateTime</param>
        public Cell(DateTime Value)
            : this()
        {
            this.DATE = Value;
            this.AFFINITY = CellAffinity.DATE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a string cell; strings greater than 1024 chars will be truncated
        /// </summary>
        /// <param name="Value">A .Net string Value to be converted to a cell</param>
        /// <param name="TrimQuotes">True will conver 'ABCD' to ABCD</param>
        public Cell(string Value, bool UTF8)
            : this()
        {

            // Set the affinity //
            this.AFFINITY = (UTF8 ? CellAffinity.TEXT : CellAffinity.STRING);

            // Handle null strings //
            if (Value == null)
            {
                this.STRING = "\0";
                this.NULL = 1;
                return;
            }

            // Fix the values
            if (Value.Length == 0) // fix instances that are zero length
                Value = "\0";
            else if (Value.Length >= MAX_STRING_LENGTH) // Fix strings that are too long
                Value = Value.Substring(0, MAX_STRING_LENGTH);

            this.STRING = Value;
            this.NULL = 0;

            this.INT_A = Value.GetHashCode();
            this.INT_B = Value.Length;

        }

        /// <summary>
        /// Creates a string cell; strings greater than 1024 chars will be truncated
        /// </summary>
        /// <param name="Value">A .Net string Value to be converted to a cell</param>
        public Cell(string Value)
            : this(Value, true)
        {
        }

        /// <summary>
        /// Creats a BLOB cell
        /// </summary>
        /// <param name="Value">A .Net array of bytes</param>
        public Cell(byte[] Value)
            : this()
        {
            this.BLOB = Value;
            this.NULL = 0;
            this.AFFINITY = CellAffinity.BLOB;
            for (int i = 0; i < Value.Length; i++)
                this.INT_A += Value[i] * i;
            this.INT_A = this.INT_A ^ Value.Length;
            this.INT_B = Value.Length;
        }

        /// <summary>
        /// Creates a cell of a given affinity that is null
        /// </summary>
        /// <param name="Type">An affinity of the new cell</param>
        public Cell(CellAffinity Type)
            : this()
        {
            this.AFFINITY = Type;
            this.NULL = 1;
            if (Type == CellAffinity.STRING)
                this.STRING = "";
            if (Type == CellAffinity.BLOB)
                this.BLOB = new byte[0];
        }

        // -- Auto Casts -- //
        /// <summary>
        /// Creates a 64 bit integer cell
        /// </summary>
        /// <param name="ValueA">A .Net 32 bit integer that will make up the first 4 bytes of integer</param>
        /// <param name="ValueB"></param>
        internal Cell(int ValueA, int ValueB)
            : this()
        {

            // Set these values //
            this.INT_A = ValueA;
            this.INT_B = ValueB;
            this.AFFINITY = CellAffinity.LONG;
            this.NULL = 0;

        }

        internal Cell(ulong Value, CellAffinity Affinity)
            : this()
        {
            this.AFFINITY = Affinity;
            this.ULONG = Value;
            this.NULL = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The cell's Spike affinity
        /// </summary>
        public CellAffinity Affinity
        {
            get { return this.AFFINITY; }
        }

        /// <summary>
        /// True == null, False == not null
        /// </summary>
        public bool IsNull
        {
            get { return NULL == 1; }
        }

        /// <summary>
        /// True if the numeric Value is 0 or if the variable length Value has a zero length
        /// </summary>
        public bool IsZero
        {
            get
            {
                if (this.IsNull) return false;
                switch (this.Affinity)
                {
                    case CellAffinity.BYTE:
                    case CellAffinity.SHORT:
                    case CellAffinity.INT:
                    case CellAffinity.LONG: return this.LONG == 0;
                    case CellAffinity.FLOAT:
                    case CellAffinity.DOUBLE: return this.DOUBLE == 0;
                    case CellAffinity.BOOL: return !this.BOOL;
                    case CellAffinity.TEXT:
                    case CellAffinity.STRING: return this.STRING.Length == 0;
                    case CellAffinity.BLOB: return this.BLOB.Length == 0;
                    default: return false;
                }
            }

        }

        /// <summary>
        /// Returns true if the integer Value or double Value is 1, or if the boolean is true, false otherwise
        /// </summary>
        public bool IsOne
        {

            get
            {
                if (this.IsNull) return false;
                switch (this.Affinity)
                {
                    case CellAffinity.BYTE:
                    case CellAffinity.SHORT:
                    case CellAffinity.INT:
                    case CellAffinity.LONG: return this.LONG == 1;
                    case CellAffinity.FLOAT:
                    case CellAffinity.DOUBLE: return this.DOUBLE == 1;
                    case CellAffinity.BOOL: return this.BOOL;
                    default: return false;
                }
            }

        }

        /// <summary>
        /// Returns a hash Value as a long integer
        /// </summary>
        public long LASH
        {

            get
            {

                if (this.NULL == 1)
                {
                    return 0L;
                }
                else if (this.AFFINITY != CellAffinity.STRING && this.AFFINITY != CellAffinity.BLOB)
                {
                    return this.LONG;
                }
                else if (this.AFFINITY == CellAffinity.STRING)
                {
                    
                    long l = 0;
                    for (int i = 0; i < this.STRING.Length; i++)
                    {
                        l += (i + 1) * (this.STRING[i] + 1);
                    }
                    return l;

                }
                else if (this.AFFINITY == CellAffinity.BLOB)
                {

                    long l = 0;
                    for (int i = 0; i < this.BLOB.Length; i++)
                    {
                        l += (i + 1) * (this.BLOB[i] + 1);
                    }
                    return l;

                }

                return 0;

            }

        }

        /// <summary>
        /// Returns the length of the cell
        /// </summary>
        public int Length
        {
            get
            {

                int len = 0;
                if ((this.AFFINITY == CellAffinity.STRING || this.AFFINITY == CellAffinity.TEXT) && this.STRING != null)
                    len = this.STRING.Length;
                else if (this.AFFINITY == CellAffinity.BLOB && this.BLOB != null)
                    len = this.BLOB.Length;

                return CellSerializer.Length(this.AFFINITY, len);
                    
            }
        }

        #endregion

        #region SafeValues

        /// <summary>
        /// Returns the bool Value if the affinity is 'BOOL', true if the LONG property is 0, false otherwise
        /// </summary>
        public bool valueBOOL
        {
            get
            {
                if (this.AFFINITY == CellAffinity.BOOL) return this.BOOL;
                return this.LONG == 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte valueBYTE
        {
            get
            {

                if (this.AFFINITY == CellAffinity.BYTE)
                    return this.BYTE;

                if (this.AFFINITY == CellAffinity.SHORT)
                    return (byte)(this.SHORT & 255);
                if (this.AFFINITY == CellAffinity.INT)
                    return (byte)(this.INT & 255);
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE)
                    return (byte)(this.LONG & 255);
                if (this.AFFINITY == CellAffinity.FLOAT)
                    return (byte)(this.FLOAT);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (byte)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (byte)1 : (byte)0;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public short valueSHORT
        {
            get
            {

                if (this.AFFINITY == CellAffinity.SHORT)
                    return this.SHORT;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (short)this.BYTE;
                if (this.AFFINITY == CellAffinity.INT)
                    return (short)(this.INT & 255);
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE)
                    return (short)(this.LONG & 255);
                if (this.AFFINITY == CellAffinity.FLOAT)
                    return (short)(this.FLOAT);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (short)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (short)1 : (short)0;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int valueINT
        {
            get
            {
                
                if (this.AFFINITY == CellAffinity.INT)
                    return this.INT;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (int)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (int)this.SHORT;
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE)
                    return (int)(this.LONG & 255);
                if (this.AFFINITY == CellAffinity.FLOAT)
                    return (int)(this.FLOAT);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (int)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (int)1 : (int)0;

                return 0;
            }
        }

        /// <summary>
        /// Return the LONG Value if the affinity is LONG, casts the DOUBLE as an LONG if the affinity is a DOUBLE, 0 otherwise
        /// </summary>
        public long valueLONG
        {
            get
            {

                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE)
                    return this.LONG;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (long)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (long)this.SHORT;
                if (this.AFFINITY == CellAffinity.INT)
                    return (long)this.INT;
                if (this.AFFINITY == CellAffinity.FLOAT)
                    return (long)(this.FLOAT);
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (long)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (long)1 : (long)0;

                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float valueFLOAT
        {
            get
            {
                if (this.AFFINITY == CellAffinity.FLOAT)
                    return this.FLOAT;

                if (this.AFFINITY == CellAffinity.BYTE)
                    return (float)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (float)this.SHORT;
                if (this.AFFINITY == CellAffinity.INT)
                    return (float)this.INT;
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE)
                    return (float)this.LONG;
                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return (float)this.DOUBLE;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (float)1 : (float)0;

                return 0;

            }
        }

        /// <summary>
        /// Return the DOUBLE Value if the affinity is DOUBLE, casts the LONG as an DOUBLE if the affinity is a LONG, 0 otherwise
        /// </summary>
        public double valueDOUBLE
        {
            get
            {

                if (this.AFFINITY == CellAffinity.DOUBLE)
                    return this.DOUBLE;
                
                if (this.AFFINITY == CellAffinity.BYTE)
                    return (double)this.BYTE;
                if (this.AFFINITY == CellAffinity.SHORT)
                    return (double)this.SHORT;
                if (this.AFFINITY == CellAffinity.INT)
                    return (double)this.INT;
                if (this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DATE)
                    return (double)this.LONG;
                if (this.AFFINITY == CellAffinity.FLOAT)
                    return (double)this.FLOAT;
                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL ? (double)1 : (double)0;

                return 0;

            }
        }

        /// <summary>
        /// Returns the Spike DATE if the affinity is DATE, otherwise return the minimum date time .Net Value
        /// </summary>
        public DateTime valueDATE
        {
            get
            {
                if (this.Affinity == CellAffinity.DATE) return this.DATE;
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string valueTEXT
        {
            get
            {

                if (this.IsNull)
                    return Cell.NULL_STRING_TEXT;

                switch (this.Affinity)
                {

                    case CellAffinity.BYTE:
                        return this.BYTE.ToString();
                    case CellAffinity.SHORT:
                        return this.SHORT.ToString();
                    case CellAffinity.INT:
                        return this.INT.ToString();
                    case CellAffinity.LONG:
                        return this.LONG.ToString();

                    case CellAffinity.FLOAT:
                        return Math.Round(this.FLOAT, NUMERIC_ROUNDER).ToString();
                    case CellAffinity.DOUBLE:
                        return Math.Round(this.DOUBLE, NUMERIC_ROUNDER).ToString();

                    case CellAffinity.BOOL:
                        return this.BOOL ? TRUE_STRING : FALSE_STRING;

                    case CellAffinity.DATE:
                        return CellFormater.ToLongDate(this.DATE);

                    case CellAffinity.TEXT:
                    case CellAffinity.STRING:
                        return this.STRING;

                    case CellAffinity.BLOB:
                        return HEX_LITERARL + BitConverter.ToString(this.BLOB).Replace("-", "");

                    default:
                        return "";

                }

            }

        }

        /// <summary>
        /// If the cell is null, returns '@@NULL'; otherwise, casts the Value as a string
        /// </summary>
        public string valueSTRING
        {
            get
            {

                if (this.IsNull)
                    return Cell.NULL_STRING_TEXT;

                switch (this.Affinity)
                {

                    case CellAffinity.BYTE:
                        return this.BYTE.ToString();
                    case CellAffinity.SHORT:
                        return this.SHORT.ToString();
                    case CellAffinity.INT:
                        return this.INT.ToString();
                    case CellAffinity.LONG:
                        return this.LONG.ToString();

                    case CellAffinity.FLOAT:
                        return Math.Round(this.FLOAT, NUMERIC_ROUNDER).ToString();
                    case CellAffinity.DOUBLE:
                        return Math.Round(this.DOUBLE, NUMERIC_ROUNDER).ToString();

                    case CellAffinity.BOOL:
                        return this.BOOL ? TRUE_STRING : FALSE_STRING;

                    case CellAffinity.DATE:
                        return CellFormater.ToShortDate(this.DATE);

                    case CellAffinity.TEXT:
                    case CellAffinity.STRING:
                        return this.STRING;

                    case CellAffinity.BLOB:
                        return HEX_LITERARL + BitConverter.ToString(this.BLOB).Replace("-", "");

                    default:
                        return "";

                }

            }

        }

        /// <summary>
        /// If the affinity is null, returns an empty byte array; if the Value is a BLOB, returns the BLOB; if the Value is a stirng, returns the string as a byte array, unless the string has a hex prefix, then it converts the hex string to a byte array; otherwise it converts an LONG, DOUBLE, BOOL to a byte array.
        /// </summary>
        public byte[] valueBLOB
        {
            get
            {

                if (this.AFFINITY == CellAffinity.BLOB)
                    return this.NULL == 1 ? new byte[0] : this.BLOB;

                if (this.AFFINITY == CellAffinity.BOOL)
                    return this.BOOL == true ? new byte[1] { 1 } : new byte[1] { 0 };
                else if (this.AFFINITY == CellAffinity.BYTE)
                    return new byte[1] { this.BYTE };
                else if (this.AFFINITY == CellAffinity.SHORT)
                    return BitConverter.GetBytes(this.SHORT);
                else if (this.AFFINITY == CellAffinity.FLOAT || this.AFFINITY == CellAffinity.INT)
                    return BitConverter.GetBytes(this.INT);
                else if (this.AFFINITY == CellAffinity.DATE || this.AFFINITY == CellAffinity.LONG || this.AFFINITY == CellAffinity.DOUBLE)
                    return BitConverter.GetBytes(this.LONG);
                else // STRING
                    return ASCIIEncoding.BigEndianUnicode.GetBytes(this.STRING);

            }
        }

        /// <summary>
        /// Gets the Value of the cell as an object
        /// </summary>
        public object valueObject
        {

            get
            {
                switch(this.AFFINITY)
                {
                    case CellAffinity.BOOL: return this.BOOL;
                    case CellAffinity.BYTE: return this.BYTE;
                    case CellAffinity.SHORT: return this.SHORT;
                    case CellAffinity.INT: return this.INT;
                    case CellAffinity.LONG: return this.LONG;
                    case CellAffinity.FLOAT: return this.FLOAT;
                    case CellAffinity.DOUBLE: return this.DOUBLE;
                    case CellAffinity.DATE: return this.DATE;
                    case CellAffinity.TEXT:
                    case CellAffinity.STRING: return this.STRING;
                    case CellAffinity.BLOB: return this.BLOB;
                    
                }

                return CellValues.NullINT;
            }

        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the valueString property
        /// </summary>
        /// <returns>A string reprsentation of a cell</returns>
        public override string ToString()
        {

            // Check if null //
            if (this.IsNull == true) return Cell.NULL_STRING_TEXT;

            return this.valueSTRING;

        }

        /// <summary>
        /// Casts an object as a cell then compares it to the Spike instance
        /// </summary>
        /// <param name="obj">The object being tested for equality</param>
        /// <returns>A boolean indicating both objects are equal</returns>
        public override bool Equals(object obj)
        {
            return CellComparer.Compare(this, (Cell)obj) == 0;
        }

        /// <summary>
        /// If null, return int.MinValue, for LONG, DOUBLE, BOOL, and DATE, return INT_A; for blobs, returns the sum of all bytes; for strings, returns the sum of the (i + 1) OriginalPage char[i]
        /// </summary>
        /// <returns>An integer hash code</returns>
        public override int GetHashCode()
        {

            if (this.NULL == 1)
                return int.MinValue;

            if (this.Affinity != CellAffinity.STRING && this.AFFINITY != CellAffinity.BLOB)
                return this.INT_A;

            if (this.AFFINITY == CellAffinity.BLOB)
                return this.BLOB.Sum<byte>((x) => { return (int)x; });

            int t = 0;
            for (int i = 0; i < this.STRING.Length; i++)
                t += (i + 1) * this.STRING[i];

            return t;

        }

        #endregion

        #region Operators

        /*
         * if any of the below say they 'dont work', that just means they return a null Value
         * 
         * All opperations returning a Cell will return a null Cell if either A or B are null (the affinity based on A)
         * +: add, returns null for date, blob and boolean; returns concatenate for strings
         * -: subtract, returns null for boolean and blob and string; resturn a long for ticks difference for dates
         * *: multiply, returns null for date, blob, string and boolean
         * /: divide, returns null for date, blob, string and boolean
         * %: mod, returns null for date, blob, string, boolean, and double
         * ^: xor, works for all types, may return nonsense for double/datetime, good for encrypting strings
         * &: and, works for all types, may return nonsense for double/datetime, good for encrypting strings
         * |: or, works for all types, may return nonsense for double/datetime, good for encrypting strings
         * ==: equals, works for all types
         * !=: not equals, works for all types
         * <: less than, works for all types
         * <=: less than or equals, works for all types
         * >: greater than, works for all types
         * >=: greater than or equals, works for all types
         * true/false: all types
         * ++/--: only for numerics
         */

        /// <summary>
        /// Performs the 'NOT' opperation, will return for null for DATE, STRING, and BLOBs
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>A cell</returns>
        public static Cell operator !(Cell C)
        {

            // Check nulls //
            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.FLOAT)
                C.FLOAT = -C.FLOAT;
            else if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE = -C.DOUBLE;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT = (short)(-C.SHORT);
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT = -C.INT;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG = -C.LONG;
            else if (C.AFFINITY == CellAffinity.BOOL)
                C.BOOL = !C.BOOL;
            else
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Adds two cells together for LONG and DOUBLE, concatentates strings, returns null otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator +(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE += C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT)
                {
                    C1.FLOAT += C2.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE += C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT += C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.INT)
                {
                    C1.INT += C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG += C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.STRING || C1.AFFINITY == CellAffinity.TEXT)
                {
                    C1.STRING += C2.STRING;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB)
                {
                    byte[] b = new byte[C1.BLOB.Length + C2.BLOB.Length];
                    Array.Copy(C1.BLOB, 0, b, 0, C1.BLOB.Length);
                    Array.Copy(C2.BLOB, 0, b, C1.BLOB.Length, C2.BLOB.Length);
                    C1 = new Cell(b);
                }
                else
                    C1.NULL = 1;

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
                {
                    C1.STRING = C1.valueSTRING + C2.valueSTRING;
                    C1.AFFINITY = CellAffinity.STRING;
                }
                else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
                {
                    C1.STRING = C1.valueTEXT + C2.valueTEXT;
                    C1.AFFINITY = CellAffinity.TEXT;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB || C2.AFFINITY == CellAffinity.BLOB)
                {
                    byte[] b = new byte[C1.valueBLOB.Length + C2.valueBLOB.Length];
                    Array.Copy(C1.valueBLOB, 0, b, 0, C1.valueBLOB.Length);
                    Array.Copy(C2.valueBLOB, 0, b, C1.valueBLOB.Length, C2.valueBLOB.Length);
                    C1 = new Cell(b);
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE = C1.valueDOUBLE + C2.valueDOUBLE;
                    C1.AFFINITY = CellAffinity.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                {
                    C1.FLOAT = C1.valueFLOAT + C2.valueFLOAT;
                    C1.AFFINITY = CellAffinity.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG = C1.valueLONG + C2.valueLONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {
                    C1.INT = C1.valueINT + C2.valueINT;
                    C1.AFFINITY = CellAffinity.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT = (short)(C1.valueSHORT + C2.valueSHORT);
                    C1.AFFINITY = CellAffinity.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE = (byte)(C1.valueBYTE + C2.valueBYTE);
                    C1.AFFINITY = CellAffinity.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                {
                    C1.AFFINITY = CellAffinity.DATE;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.STRING = "";
            }
            return C1;

        }

        /// <summary>
        /// Converts either an LONG or DOUBLE to a positve Value, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator +(Cell C)
        {

            // Check nulls //
            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.FLOAT)
                C.FLOAT = +C.FLOAT;
            else if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE = +C.DOUBLE;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT = (short)(+C.SHORT);
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT = +C.INT;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG = +C.LONG;
            else
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Adds one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">The cell argument</param>
        /// <returns>Cell result</returns>
        public static Cell operator ++(Cell C)
        {
            if (C.NULL == 1)
                return C;
            if (C.AFFINITY == CellAffinity.FLOAT)
                C.FLOAT++;
            else if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE++;
            else if (C.AFFINITY == CellAffinity.BYTE)
                C.BYTE++;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT++;
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT++;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG++;
            return C;
        }

        /// <summary>
        /// Subtracts two cells together for LONG and DOUBLE, repalces instances of C2 in C1, for date times, return the tick count difference as an LONG
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator -(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {
                if (C1.AFFINITY == CellAffinity.FLOAT)
                {
                    C1.FLOAT -= C2.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE -= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE -= C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT -= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.INT)
                {
                    C1.INT -= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG -= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.DATE)
                {
                    C1.LONG = C1.LONG - C2.LONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.STRING || C1.AFFINITY == CellAffinity.TEXT)
                {
                    C1.STRING = C1.STRING.Replace(C2.STRING, "");
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
                {
                    C1.STRING = C1.valueSTRING.Replace(C2.valueSTRING, "");
                    C1.AFFINITY = CellAffinity.STRING;
                }
                else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
                {
                    C1.STRING = C1.valueTEXT.Replace(C2.valueTEXT, "");
                    C1.AFFINITY = CellAffinity.TEXT;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB || C2.AFFINITY == CellAffinity.BLOB)
                {
                    C1.AFFINITY = CellAffinity.BLOB;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE = C1.valueDOUBLE - C2.valueDOUBLE;
                    C1.AFFINITY = CellAffinity.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                {
                    C1.FLOAT = C1.valueFLOAT - C2.valueFLOAT;
                    C1.AFFINITY = CellAffinity.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG = C1.valueLONG - C2.valueLONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {
                    C1.INT = C1.valueINT - C2.valueINT;
                    C1.AFFINITY = CellAffinity.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT = (short)(C1.valueSHORT - C2.valueSHORT);
                    C1.AFFINITY = CellAffinity.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE = (byte)(C1.valueBYTE - C2.valueBYTE);
                    C1.AFFINITY = CellAffinity.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                {
                    C1.AFFINITY = CellAffinity.DATE;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.STRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Converts either an LONG or DOUBLE to a negative Value, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator -(Cell C)
        {

            // Check nulls //
            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE = -C.DOUBLE;
            else if (C.AFFINITY == CellAffinity.FLOAT)
                C.FLOAT = -C.FLOAT;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT = (short)(-C.SHORT);
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT = -C.INT;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG = -C.LONG;
            else if (C.AFFINITY == CellAffinity.STRING || C.AFFINITY == CellAffinity.TEXT)
                C.STRING = new string(C.STRING.Reverse().ToArray());
            else
                C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Subtracts one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">The cell argument</param>
        /// <returns>Cell result</returns>
        public static Cell operator --(Cell C)
        {
            if (C.NULL == 1)
                return C;
            if (C.AFFINITY == CellAffinity.DOUBLE)
                C.DOUBLE--;
            else if (C.AFFINITY == CellAffinity.FLOAT)
                C.FLOAT--;
            else if (C.AFFINITY == CellAffinity.SHORT)
                C.SHORT--;
            else if (C.AFFINITY == CellAffinity.INT)
                C.INT--;
            else if (C.AFFINITY == CellAffinity.LONG)
                C.LONG--;
            return C;
        }

        /// <summary>
        /// Multiplies two cells together for LONG and DOUBLE; if C1 is a string and C2 is either int/double, repeats the string C2 times; 
        /// otherwise, returns the cell passed otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator *(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE *= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT)
                {
                    C1.FLOAT *= C2.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE *= C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT *= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.INT)
                {
                    C1.INT *= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG *= C2.LONG;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
                {
                    C1.AFFINITY = CellAffinity.STRING;
                    C1.NULL = 1;
                }
                if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
                {
                    C1.AFFINITY = CellAffinity.TEXT;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB || C2.AFFINITY == CellAffinity.BLOB)
                {
                    C1.AFFINITY = CellAffinity.BLOB;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.DOUBLE = C1.valueDOUBLE * C2.valueDOUBLE;
                    C1.AFFINITY = CellAffinity.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                {
                    C1.FLOAT = C1.valueFLOAT * C2.valueFLOAT;
                    C1.AFFINITY = CellAffinity.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {
                    C1.LONG = C1.valueLONG * C2.valueLONG;
                    C1.AFFINITY = CellAffinity.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {
                    C1.INT = C1.valueINT * C2.valueINT;
                    C1.AFFINITY = CellAffinity.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {
                    C1.SHORT = (short)(C1.valueSHORT * C2.valueSHORT);
                    C1.AFFINITY = CellAffinity.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {
                    C1.BYTE = (byte)(C1.valueBYTE * C2.valueBYTE);
                    C1.AFFINITY = CellAffinity.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                {
                    C1.AFFINITY = CellAffinity.DATE;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.STRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator /(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
                {
                    C1.DOUBLE /= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT && C2.FLOAT != 0)
                {
                    C1.FLOAT /= C2.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
                {
                    C1.LONG /= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
                {
                    C1.INT /= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
                {
                    C1.SHORT /= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
                {
                    C1.BYTE /= C2.BYTE;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
                {
                    C1.AFFINITY = CellAffinity.STRING;
                    C1.NULL = 1;
                }
                if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
                {
                    C1.AFFINITY = CellAffinity.TEXT;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB || C2.AFFINITY == CellAffinity.BLOB)
                {
                    C1.AFFINITY = CellAffinity.BLOB;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {

                    if (C2.valueDOUBLE != 0)
                    {
                        C1.DOUBLE = C1.valueDOUBLE / C2.valueDOUBLE;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.DOUBLE;

                }
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                {

                    if (C2.valueFLOAT != 0)
                    {
                        C1.FLOAT = C1.valueFLOAT / C2.valueFLOAT;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.FLOAT;

                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {

                    if (C2.valueLONG != 0)
                    {
                        C1.LONG = C1.valueLONG / C2.valueLONG;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.LONG;

                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {

                    if (C2.valueINT != 0)
                    {
                        C1.INT = C1.valueINT / C2.valueINT;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.INT;

                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {

                    if (C2.valueSHORT != 0)
                    {
                        C1.SHORT = (short)(C1.valueSHORT / C2.valueSHORT);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.SHORT;

                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {

                    if (C2.valueBYTE != 0)
                    {
                        C1.BYTE = (byte)(C1.valueBYTE / C2.valueBYTE);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.BYTE;

                }
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                {
                    C1.AFFINITY = CellAffinity.DATE;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.STRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null; if C2 is 0, then it returns 0
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell CheckDivide(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
                {
                    C1.DOUBLE /= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT && C2.FLOAT != 0)
                {
                    C1.FLOAT /= C2.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
                {
                    C1.LONG /= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
                {
                    C1.INT /= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
                {
                    C1.SHORT /= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
                {
                    C1.BYTE /= C2.BYTE;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C1.AFFINITY == CellAffinity.SHORT || C1.AFFINITY == CellAffinity.INT 
                    || C1.AFFINITY == CellAffinity.LONG || C1.AFFINITY == CellAffinity.FLOAT || C1.AFFINITY == CellAffinity.DOUBLE)
                {
                    C1.ULONG = 0;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
                {
                    C1.AFFINITY = CellAffinity.STRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
                {
                    C1.AFFINITY = CellAffinity.TEXT;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB || C2.AFFINITY == CellAffinity.BLOB)
                {
                    C1.AFFINITY = CellAffinity.BLOB;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {

                    if (C2.valueDOUBLE != 0)
                    {
                        C1.DOUBLE = C1.valueDOUBLE / C2.valueDOUBLE;
                    }
                    else
                    {
                        C1.DOUBLE = 0D;
                    }
                    C1.AFFINITY = CellAffinity.DOUBLE;

                }
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                {

                    if (C2.valueFLOAT != 0)
                    {
                        C1.FLOAT = C1.valueFLOAT / C2.valueFLOAT;
                    }
                    else
                    {
                        C1.FLOAT = 0F;
                    }
                    C1.AFFINITY = CellAffinity.FLOAT;

                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {

                    if (C2.valueLONG != 0)
                    {
                        C1.LONG = C1.valueLONG / C2.valueLONG;
                    }
                    else
                    {
                        C1.LONG = 0;
                    }
                    C1.AFFINITY = CellAffinity.LONG;

                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {

                    if (C2.valueINT != 0)
                    {
                        C1.INT = C1.valueINT / C2.valueINT;
                    }
                    else
                    {
                        C1.INT = 0;
                    }
                    C1.AFFINITY = CellAffinity.INT;

                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {

                    if (C2.valueSHORT != 0)
                    {
                        C1.SHORT = (short)(C1.valueSHORT / C2.valueSHORT);
                    }
                    else
                    {
                        C1.SHORT = 0;
                    }
                    C1.AFFINITY = CellAffinity.SHORT;

                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {

                    if (C2.valueBYTE != 0)
                    {
                        C1.BYTE = (byte)(C1.valueBYTE / C2.valueBYTE);
                    }
                    else
                    {
                        C1.BYTE = 0;
                    }
                    C1.AFFINITY = CellAffinity.BYTE;

                }
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                {
                    C1.AFFINITY = CellAffinity.DATE;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.STRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Perform modulo between two cells together for LONG and DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator %(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // If affinities match //
            if (C1.AFFINITY == C2.AFFINITY)
            {

                if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
                {
                    C1.DOUBLE %= C2.DOUBLE;
                }
                else if (C1.AFFINITY == CellAffinity.FLOAT && C2.FLOAT != 0)
                {
                    C1.FLOAT %= C2.FLOAT;
                }
                else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
                {
                    C1.LONG %= C2.LONG;
                }
                else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
                {
                    C1.INT %= C2.INT;
                }
                else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
                {
                    C1.SHORT %= C2.SHORT;
                }
                else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
                {
                    C1.BYTE %= C2.BYTE;
                }
                else
                {
                    C1.NULL = 1;
                }

            }
            else
            {

                if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
                {
                    C1.AFFINITY = CellAffinity.STRING;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
                {
                    C1.AFFINITY = CellAffinity.TEXT;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.BLOB || C2.AFFINITY == CellAffinity.BLOB)
                {
                    C1.AFFINITY = CellAffinity.BLOB;
                    C1.NULL = 1;
                }
                else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                {

                    if (C2.valueDOUBLE != 0)
                    {
                        C1.DOUBLE = C1.valueDOUBLE % C2.valueDOUBLE;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.DOUBLE;

                }
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                {

                    if (C2.valueFLOAT != 0)
                    {
                        C1.FLOAT = C1.valueFLOAT % C2.valueFLOAT;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.FLOAT;

                }
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                {

                    if (C2.valueLONG != 0)
                    {
                        C1.LONG = C1.valueLONG % C2.valueLONG;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.LONG;

                }
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                {

                    if (C2.valueINT != 0)
                    {
                        C1.INT = C1.valueINT % C2.valueINT;
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.INT;

                }
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                {

                    if (C2.valueSHORT != 0)
                    {
                        C1.SHORT = (short)(C1.valueSHORT % C2.valueSHORT);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.SHORT;

                }
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                {

                    if (C2.valueBYTE != 0)
                    {
                        C1.BYTE = (byte)(C1.valueBYTE % C2.valueBYTE);
                    }
                    else
                    {
                        C1.NULL = 1;
                    }
                    C1.AFFINITY = CellAffinity.BYTE;

                }
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                {
                    C1.AFFINITY = CellAffinity.DATE;
                    C1.NULL = 1;
                }
                else
                {
                    C1.AFFINITY = CellAffinity.BOOL;
                    C1.NULL = 1;
                }

            }

            // Fix nulls //
            if (C1.NULL == 1)
            {
                C1.ULONG = 0;
                C1.STRING = "";
            }

            return C1;

        }

        /// <summary>
        /// Return the bitwise AND for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator &(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            // Handle AND two bools //
            if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
                return (C1.BOOL && C2.BOOL ? CellValues.True : CellValues.False);

            // If neither a string or blob //
            if (C1.AFFINITY != CellAffinity.STRING && C2.AFFINITY != CellAffinity.STRING
                && C1.AFFINITY != CellAffinity.TEXT && C2.AFFINITY != CellAffinity.TEXT
                && C1.AFFINITY != CellAffinity.BLOB && C2.AFFINITY != CellAffinity.BLOB)
            {

                C1.LONG = C1.LONG & C2.LONG;
                if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                    C1.AFFINITY = CellAffinity.DOUBLE;
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                    C1.AFFINITY = CellAffinity.FLOAT;
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                    C1.AFFINITY = CellAffinity.LONG;
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                    C1.AFFINITY = CellAffinity.INT;
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                    C1.AFFINITY = CellAffinity.SHORT;
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                    C1.AFFINITY = CellAffinity.BYTE;
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                    C1.AFFINITY = CellAffinity.DATE;
                else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
                    C1.AFFINITY = CellAffinity.BOOL;

            }
            else if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.STRING.Length; i++)
                {
                    if (t >= C2.valueSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.valueSTRING[i] & C2.valueSTRING[t]));
                    t++;
                }
                C1.STRING = sb.ToString();

            }
            else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.STRING.Length; i++)
                {
                    if (t >= C2.valueSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.valueSTRING[i] & C2.valueSTRING[t]));
                    t++;
                }
                C1.STRING = sb.ToString();

            }
            else
            {

                int t = 0;
                byte[] b = C2.valueBLOB;
                for (int i = 0; i < C1.BLOB.Length; i++)
                {
                    if (t >= b.Length)
                        t = 0;
                    C1.BLOB[i] = (byte)(C1.BLOB[i] & b[t]);
                    t++;
                }

            }

            return C1;

        }

        /// <summary>
        /// Returns the bitwise OR for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator |(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // Handle AND two bools //
            if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
                return (C1.BOOL || C2.BOOL ? CellValues.True : CellValues.False);

            // If neither a string or blob //
            if (C1.AFFINITY != CellAffinity.STRING && C2.AFFINITY != CellAffinity.STRING
                && C1.AFFINITY != CellAffinity.TEXT && C2.AFFINITY != CellAffinity.TEXT
                && C1.AFFINITY != CellAffinity.BLOB && C2.AFFINITY != CellAffinity.BLOB)
            {

                C1.LONG = C1.LONG | C2.LONG;
                if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                    C1.AFFINITY = CellAffinity.DOUBLE;
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                    C1.AFFINITY = CellAffinity.FLOAT;
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                    C1.AFFINITY = CellAffinity.LONG;
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                    C1.AFFINITY = CellAffinity.INT;
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                    C1.AFFINITY = CellAffinity.SHORT;
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                    C1.AFFINITY = CellAffinity.BYTE;
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                    C1.AFFINITY = CellAffinity.DATE;
                else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
                    C1.AFFINITY = CellAffinity.BOOL;

            }
            else if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
            {
                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.STRING.Length; i++)
                {
                    if (t >= C2.valueSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.STRING[i] | C2.valueSTRING[t]));
                    t++;
                }
                C1.STRING = sb.ToString();

            }
            else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
            {
                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.STRING.Length; i++)
                {
                    if (t >= C2.valueSTRING.Length)
                        t = 0;
                    sb.Append((char)(C1.STRING[i] | C2.valueSTRING[t]));
                    t++;
                }
                C1.STRING = sb.ToString();

            }
            else
            {

                int t = 0;
                byte[] b = C2.valueBLOB;
                for (int i = 0; i < C1.BLOB.Length; i++)
                {
                    if (t >= b.Length) t = 0;
                    C1.BLOB[i] = (byte)(C1.BLOB[i] | b[t]);
                    t++;
                }

            }

            return C1;

        }

        /// <summary>
        /// Returns the bitwise XOR for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator ^(Cell C1, Cell C2)
        {

            // Check nulls //
            if (C1.NULL == 1) return C1;
            else if (C2.NULL == 1) return C2;

            // Handle AND two bools //
            if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
                return (C1.BOOL ^ C2.BOOL ? CellValues.True : CellValues.False);

            // If neither a string or blob //
            if (C1.AFFINITY != CellAffinity.STRING && C2.AFFINITY != CellAffinity.STRING
                && C1.AFFINITY != CellAffinity.TEXT && C2.AFFINITY != CellAffinity.TEXT
                && C1.AFFINITY != CellAffinity.BLOB && C2.AFFINITY != CellAffinity.BLOB)
            {

                C1.LONG = C1.LONG ^ C2.LONG;
                if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
                    C1.AFFINITY = CellAffinity.DOUBLE;
                else if (C1.AFFINITY == CellAffinity.FLOAT || C2.AFFINITY == CellAffinity.FLOAT)
                    C1.AFFINITY = CellAffinity.FLOAT;
                else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
                    C1.AFFINITY = CellAffinity.LONG;
                else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
                    C1.AFFINITY = CellAffinity.INT;
                else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
                    C1.AFFINITY = CellAffinity.SHORT;
                else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
                    C1.AFFINITY = CellAffinity.BYTE;
                else if (C1.AFFINITY == CellAffinity.DATE || C2.AFFINITY == CellAffinity.DATE)
                    C1.AFFINITY = CellAffinity.DATE;
                else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
                    C1.AFFINITY = CellAffinity.BOOL;

            }
            else if (C1.AFFINITY == CellAffinity.STRING || C2.AFFINITY == CellAffinity.STRING)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.STRING.Length; i++)
                {
                    if (t >= C2.valueSTRING.Length) t = 0;
                    sb.Append((char)(C1.STRING[i] ^ C2.valueSTRING[t]));
                    t++;
                }
                C1.STRING = sb.ToString();

            }
            else if (C1.AFFINITY == CellAffinity.TEXT || C2.AFFINITY == CellAffinity.TEXT)
            {

                StringBuilder sb = new StringBuilder();
                int t = 0;
                for (int i = 0; i < C1.STRING.Length; i++)
                {
                    if (t >= C2.valueSTRING.Length) t = 0;
                    sb.Append((char)(C1.STRING[i] ^ C2.valueSTRING[t]));
                    t++;
                }
                C1.STRING = sb.ToString();

            }
            else
            {

                int t = 0;
                byte[] a = C2.valueBLOB;
                byte[] b = C2.valueBLOB;
                for (int i = 0; i < a.Length; i++)
                {
                    if (t >= b.Length) t = 0;
                    a[i] = (byte)(a[i] ^ b[t]);
                    t++;
                }
                C1.AFFINITY = CellAffinity.BLOB;
                C1.BLOB = a;
            
            }
            return C1;

        }

        /// <summary>
        /// Checks if two cells are equal
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>A boolean</returns>
        public static bool operator ==(Cell C1, Cell C2)
        {

            if (C1.NULL == 1 && C2.NULL == 1)
                return true;
            
            if (C1.AFFINITY != CellAffinity.STRING && C1.AFFINITY != CellAffinity.TEXT && C1.AFFINITY != CellAffinity.BLOB)
                return C1.LONG == C2.LONG;
            else if (C1.AFFINITY == CellAffinity.STRING || C1.AFFINITY == CellAffinity.TEXT)
                return C1.STRING == C2.valueSTRING;

            return CellComparer.Compare(C1, C2) == 0;

        }

        /// <summary>
        /// Checks if two cells are not equal
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>A boolean</returns>
        public static bool operator !=(Cell C1, Cell C2)
        {

            if (C1.NULL != C2.NULL)
                return true;

            if (C1.AFFINITY != CellAffinity.STRING && C1.AFFINITY != CellAffinity.BLOB)
                return C1.LONG != C2.LONG;
            else if (C1.AFFINITY == CellAffinity.STRING || C1.AFFINITY == CellAffinity.TEXT)
                return C1.STRING != C2.STRING;

            return CellComparer.Compare(C1, C2) != 0;

        }

        /// <summary>
        /// Checks if C1 is less than C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>A boolean</returns>
        public static bool operator <(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) < 0;
        }

        /// <summary>
        /// Checks if C1 is less than or equal to C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>A boolean</returns>
        public static bool operator <=(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) <= 0;
        }

        /// <summary>
        /// Checks if C1 is greater than C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>A boolean</returns>
        public static bool operator >(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) > 0;
        }

        /// <summary>
        /// Checks if C1 is greater than or equal to C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>A boolean</returns>
        public static bool operator >=(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) >= 0;
        }

        /// <summary>
        /// Determines whether or not a cell is 'TRUE'; if the cell is not null it returns the boolean Value
        /// </summary>
        /// <param name="C">The cell Value</param>
        /// <returns></returns>
        public static bool operator true(Cell C)
        {
            return C.NULL == 0 && C.BOOL;
        }

        /// <summary>
        /// Determines whether or not a cell is 'FALSE'; if the cell is null or the BOOL Value is false, returns false
        /// </summary>
        /// <param name="C">The cell Value</param>
        /// <returns></returns>
        public static bool operator false(Cell C)
        {
            return !(C.NULL == 0 && C.BOOL);
        }

        #endregion

        #region Implementations

        /// <summary>
        /// IComparable implementation
        /// </summary>
        /// <param name="C">A cell to compare to the Spike instance</param>
        /// <returns>An integer Value</returns>
        int IComparable<Cell>.CompareTo(Cell C)
        {
            return CellComparer.Compare(this, C);
        }

        /// <summary>
        /// IComparer implementation that compares two cells
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">Right cell</param>
        /// <returns>An integer representing </returns>
        int IComparer<Cell>.Compare(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2);
        }

        #endregion

        #region ImplicitConversion

        public static implicit operator bool(Cell C)
        {
            return C.valueBOOL;
        }

        public static implicit operator Cell(bool Value)
        {
            return new Cell(Value);
        }

        public static implicit operator DateTime(Cell C)
        {
            return C.valueDATE;
        }

        public static implicit operator Cell(DateTime Value)
        {
            return new Cell(Value);
        }

        public static implicit operator byte(Cell C)
        {
            return C.valueBYTE;
        }

        public static implicit operator Cell(byte Value)
        {
            return new Cell(Value);
        }

        public static implicit operator short(Cell C)
        {
            return C.valueSHORT;
        }

        public static implicit operator Cell(short Value)
        {
            return new Cell(Value);
        }

        public static implicit operator int(Cell C)
        {
            return C.valueINT;
        }

        public static implicit operator Cell(int Value)
        {
            return new Cell(Value);
        }

        public static implicit operator long(Cell C)
        {
            return C.valueLONG;
        }

        public static implicit operator Cell(long Value)
        {
            return new Cell(Value);
        }

        public static implicit operator float(Cell C)
        {
            return C.valueFLOAT;
        }

        public static implicit operator Cell(float Value)
        {
            return new Cell(Value);
        }

        public static implicit operator double(Cell C)
        {
            return C.valueDOUBLE;
        }

        public static implicit operator Cell(double Value)
        {
            return new Cell(Value);
        }

        public static implicit operator byte[](Cell C)
        {
            return C.valueBLOB;
        }

        public static implicit operator Cell(byte[] Value)
        {
            return new Cell(Value);
        }

        public static implicit operator string(Cell C)
        {
            return (C.Affinity == CellAffinity.TEXT ? C.valueTEXT : C.valueSTRING);
        }

        public static implicit operator Cell(string Value)
        {
            return new Cell(Value);
        }

        #endregion

        #region ExplicitConversion

        //public static explicit operator bool(Cell C)
        //{
        //    return C.valueBOOL;
        //}

        //public static explicit operator Cell(bool Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator DateTime(Cell C)
        //{
        //    return C.valueDATE;
        //}

        //public static explicit operator Cell(DateTime Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator byte(Cell C)
        //{
        //    return C.valueBYTE;
        //}

        //public static explicit operator Cell(byte Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator short(Cell C)
        //{
        //    return C.valueSHORT;
        //}

        //public static explicit operator Cell(short Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator int(Cell C)
        //{
        //    return C.valueINT;
        //}

        //public static explicit operator Cell(int Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator long(Cell C)
        //{
        //    return C.valueLONG;
        //}

        //public static explicit operator Cell(long Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator float(Cell C)
        //{
        //    return C.valueFLOAT;
        //}

        //public static explicit operator Cell(float Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator double(Cell C)
        //{
        //    return C.valueDOUBLE;
        //}

        //public static explicit operator Cell(double Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator byte[](Cell C)
        //{
        //    return C.valueBLOB;
        //}

        //public static explicit operator Cell(byte[] Value)
        //{
        //    return new Cell(Value);
        //}

        //public static explicit operator string(Cell C)
        //{
        //    return (C.Affinity == CellAffinity.TEXT ? C.valueTEXT : C.valueSTRING);
        //}

        //public static explicit operator Cell(string Value)
        //{
        //    return new Cell(Value);
        //}

        #endregion

        #region Debug

        /// <summary>
        /// A string representing: Affinity : IsNull : Value
        /// </summary>
        internal string Chime
        {
            get
            {
                return this.Affinity.ToString() + " : " + this.IsNull.ToString() + " : " + this.ToString();
            }
        }


        #endregion

    }

}