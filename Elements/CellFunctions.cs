using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Elements
{


    /// <summary>
    /// Contains functions for cells
    /// </summary>
    public static class CellFunctions
    {

        /// <summary>
        /// Performs the log base Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Log(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the log base 2; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Log2(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log(C.valueDOUBLE) / Math.Log(2D);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the log base 10; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Log10(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Log10(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Exp(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Exp(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base 2; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Exp2(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Pow(2, C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the exponential base 10; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Exp10(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Pow(10, C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic sine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Sin(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Sin(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic cosine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Cos(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Cos(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the trigonomic tangent; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Tan(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Tan(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic sine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Sinh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Sinh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic cosine; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Cosh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Cosh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the hyperbolic tangent; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Tanh(Cell C)
        {

            if (C.NULL == 1)
                return C;

            double d = Math.Tanh(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the square root; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Sqrt(Cell C)
        {

            if (C.NULL == 1)
                return C;

            //if (C.AFFINITY == CellAffinity.LONG)
            //{

            //    if (C.LONG <= 0)
            //    {
            //        C.NULL = 1;
            //        return C;
            //    }

            //    C.LONG = Cell.IntRoot(C.LONG);
            //    return C;

            //}

            double d = Math.Sqrt(C.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C.NULL = 1;
                return C;
            }

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = d;
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = (float)d;
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = (long)d;
            else if (C.AFFINITY == CellAffinity.INT) C.INT = (int)d;
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)d;
            else if (C.AFFINITY == CellAffinity.BYTE) C.BYTE = (byte)d;
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the power; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C1">The base</param>
        /// <param name="C2">The exponent</param>
        /// <returns>Cell Value</returns>
        public static Cell Power(Cell C1, Cell C2)
        {

            if (C1.NULL == 1)
                return C1;
            else if (C2.NULL == 1)
                return C2;

            //if (C1.AFFINITY == CellAffinity.LONG && C2.AFFINITY == CellAffinity.LONG)
            //{
            //    C1.LONG = Cell.IntPower(C1.LONG, C2.LONG);
            //    return C1;
            //}

            double d = Math.Pow(C1.valueDOUBLE, C2.valueDOUBLE);
            if (double.IsInfinity(d) || double.IsNaN(d))
            {
                C1.NULL = 1;
                return C1;
            }

            if (C1.AFFINITY == CellAffinity.DOUBLE) C1.DOUBLE = d;
            else if (C1.AFFINITY == CellAffinity.FLOAT) C1.FLOAT = (float)d;
            else if (C1.AFFINITY == CellAffinity.LONG) C1.LONG = (long)d;
            else if (C1.AFFINITY == CellAffinity.INT) C1.INT = (int)d;
            else if (C1.AFFINITY == CellAffinity.SHORT) C1.SHORT = (short)d;
            else if (C1.AFFINITY == CellAffinity.BYTE) C1.BYTE = (byte)d;
            else C1.NULL = 1;

            return C1;

        }

        /// <summary>
        /// Returns the absolute Value of a cell's numeric Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value</returns>
        public static Cell Abs(Cell C)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = Math.Abs(C.DOUBLE);
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = Math.Abs(C.FLOAT);
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = Math.Abs(C.LONG);
            else if (C.AFFINITY == CellAffinity.INT) C.INT = Math.Abs(C.INT);
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = Math.Abs(C.SHORT);
            else if (C.AFFINITY == CellAffinity.BYTE) C.LONG = Math.Abs(C.BYTE);
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Returns the sign of a cell's numeric Value; the resulting Value will be null if the result is either nan or infinity; casts the result back to original affinity passed
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell Value, NULL, +1, -1, or 0</returns>
        public static Cell Sign(Cell C)
        {

            if (C.NULL == 1)
                return C;

            if (C.AFFINITY == CellAffinity.DOUBLE) C.DOUBLE = Math.Sign(C.DOUBLE);
            else if (C.AFFINITY == CellAffinity.FLOAT) C.FLOAT = Math.Sign(C.FLOAT);
            else if (C.AFFINITY == CellAffinity.LONG) C.LONG = Math.Sign(C.LONG);
            else if (C.AFFINITY == CellAffinity.INT) C.INT = Math.Sign(C.INT);
            else if (C.AFFINITY == CellAffinity.SHORT) C.SHORT = (short)Math.Sign(C.SHORT);
            else if (C.AFFINITY == CellAffinity.BYTE) C.LONG = Math.Sign(C.BYTE);
            else C.NULL = 1;

            return C;

        }

        /// <summary>
        /// Performs the logic 'IF'
        /// </summary>
        /// <param name="A">Predicate: uses A.BOOL to perform the logical if</param>
        /// <param name="B">The Value returned if A is true</param>
        /// <param name="C">The Value returned if A is false</param>
        /// <returns>Aither B or C</returns>
        public static Cell If(Cell A, Cell B, Cell C)
        {
            if (A.BOOL)
                return B;
            if (B.AFFINITY != C.AFFINITY)
                return CellConverter.Cast(C, B.Affinity);
            return C;
        }

        /// <summary>
        /// Returns the smallest Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Min(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One Value //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2 && Data[0] < Data[1]) return Data[0];
            if (Data.Length == 2) return Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i] < c)
                    c = Data[i];
            }
            if (Data[0].Affinity != c.Affinity)
                return CellConverter.Cast(c, Data[0].Affinity);
            return c;

        }

        /// <summary>
        /// Returns the largest Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Max(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One Value //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2 && Data[0] > Data[1]) return Data[0];
            if (Data.Length == 2) return Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                if (Data[i] > c) c = Data[i];
            }
            if (Data[0].Affinity != c.Affinity)
                return CellConverter.Cast(c, Data[0].Affinity);
            return c;

        }

        /// <summary>
        /// Returns the most extreme Value in a sequence
        /// </summary>
        /// <param name="Elements">The sequence to evaluate; the first Value is the radix, the next N values are compared to the radix</param>
        /// <returns>The Value with the greatest distance from the radix</returns>
        public static Cell Extreme(params Cell[] Data)
        {

            // Handle invalid argument structures //
            if (Data.Length == 0)
                return CellValues.NullINT;

            CellAffinity t = Data[0].AFFINITY;

            // Handle invalid types //
            if (t == CellAffinity.BLOB || t == CellAffinity.BOOL || t == CellAffinity.STRING)
                return new Cell(t);

            // Handle arrays too small //
            if (Data.Length < 2)
                return new Cell(t);

            // Get the radix //
            Cell radix = Data[0];
            Cell MostExtreme = CellValues.Zero(t);
            Cell GreatestDistance = CellValues.Zero(t);

            // Cycle through looking for the most extreme Value //
            for (int i = 1; i < Data.Length; i++)
            {

                Cell distance = CellFunctions.Abs(Data[i] - radix);

                if (distance > GreatestDistance)
                {
                    GreatestDistance = distance;
                    MostExtreme = Data[i];
                }

            }

            if (MostExtreme.AFFINITY != t)
                return CellConverter.Cast(MostExtreme, t);
            return MostExtreme;

        }

        /// <summary>
        /// Returns the cumulative AND Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell And(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(false);
            if (Data.Length == 0) new Cell(false);

            // Three or more //
            bool b = true;
            for (int i = 0; i < Data.Length; i++)
            {
                b = b && Data[i].valueBOOL;
                if (!b) return new Cell(b);
            }
            return new Cell(b);

        }

        /// <summary>
        /// Returns the cumulative OR Value of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Or(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(false);
            if (Data.Length == 0) new Cell(false);

            // Three or more //
            bool b = false;
            for (int i = 0; i < Data.Length; i++)
            {
                b = b || Data[i].valueBOOL;
                if (b) return new Cell(b);
            }
            return new Cell(b);

        }

        /// <summary>
        /// Returns the sum of an array of cells
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Sum(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            // One Value //
            if (Data.Length == 1) return Data[0];

            // Two values //
            if (Data.Length == 2) return Data[0] + Data[1];

            // Three or more //
            Cell c = Data[0];
            for (int i = 1; i < Data.Length; i++)
            {
                c += Data[i];
            }
            return c;

        }

        /// <summary>
        /// Returns the first non-null cell in a collection
        /// </summary>
        /// <param name="Elements">A collection of cells</param>
        /// <returns>A cell</returns>
        public static Cell Coalesce(params Cell[] Data)
        {

            // Empty //
            if (Data == null) new Cell(CellAffinity.LONG);
            if (Data.Length == 0) new Cell(CellAffinity.LONG);

            for (int i = 0; i < Data.Length; i++)
            {
                if (!Data[i].IsNull)
                {
                    if (Data[i].Affinity != Data[0].Affinity)
                        return CellConverter.Cast(Data[i], Data[0].Affinity);
                    return Data[i];
                }
            }
            return new Cell(Data[0].Affinity);

        }

        /// <summary>
        /// Extracts the year Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Year(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE)
                return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Year);
        }

        /// <summary>
        /// Extracts the month Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Month(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Month);
        }

        /// <summary>
        /// Extracts the day Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Day(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Day);
        }

        /// <summary>
        /// Extracts the hour Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Hour(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Hour);
        }

        /// <summary>
        /// Extracts the minute Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Minute(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Minute);
        }

        /// <summary>
        /// Extracts the second Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Second(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Second);
        }

        /// <summary>
        /// Extracts the millisecond Value of a date time cell, returns null for non-date cells
        /// </summary>
        /// <param name="C">A cell Value</param>
        /// <returns>An integer cell</returns>
        public static Cell Millisecond(Cell C)
        {
            if (C.Affinity != CellAffinity.DATE) return new Cell(CellAffinity.INT);
            return new Cell(C.valueDATE.Millisecond);
        }

        /// <summary>
        /// Manipulates the ticks Value
        /// </summary>
        /// <param name="C"></param>
        /// <param name="Ticks"></param>
        /// <returns></returns>
        public static Cell AddTicks(Cell C, Cell Ticks)
        {

            if (C.AFFINITY != CellAffinity.DATE)
                return CellValues.NullDATE;

            C.LONG += Ticks.LONG;

            return C;

        }

        /// <summary>
        /// Trims a given string Value
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell Trim(Cell C)
        {
            return new Cell(C.valueSTRING.Trim());
        }

        /// <summary>
        /// Converts a given string to uppercase
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell ToUpper(Cell C)
        {
            return new Cell(C.valueSTRING.ToUpper());
        }

        /// <summary>
        /// Converts a given string to lowercase
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <returns>Cell with a stirng affinity</returns>
        public static Cell ToLower(Cell C)
        {
            return new Cell(C.valueSTRING.ToLower());
        }

        /// <summary>
        /// Returns all characters/bytes left of given point
        /// </summary>
        /// <param name="C">The string or BLOB Value</param>
        /// <param name="Length">The maximum number of chars/bytes</param>
        /// <returns>A string or blob cell</returns>
        public static Cell Left(Cell C, long Length)
        {
            int len = Math.Min(C.AFFINITY == CellAffinity.BLOB ? C.BLOB.Length : C.valueSTRING.Length, (int)Length);
            return CellFunctions.Substring(C, 0, len);
        }

        /// <summary>
        /// Returns all characters/bytes right of given point
        /// </summary>
        /// <param name="C">The string or BLOB Value</param>
        /// <param name="Length">The maximum number of chars/bytes</param>
        /// <returns>A string or blob cell</returns>
        public static Cell Right(Cell C, long Length)
        {
            int l = C.AFFINITY == CellAffinity.BLOB ? C.BLOB.Length : C.valueSTRING.Length;
            int begin = Math.Max(l - (int)Length, 0);
            int len = (int)Length;
            if (begin + Length > l) len = l - begin;
            return CellFunctions.Substring(C, begin, len);
        }

        /// <summary>
        /// Checks if a given string contains another string
        /// </summary>
        /// <param name="Source">The string to be checked</param>
        /// <param name="Check">The string being check for</param>
        /// <returns>Cell with boolean type</returns>
        public static Cell Contains(Cell Source, Cell Check)
        {
            return new Cell(Source.valueSTRING.Contains(Check.valueSTRING));
        }

        /// <summary>
        /// Returns either the sub stirng or sub blob
        /// </summary>
        /// <param name="C">Cell Value</param>
        /// <param name="Position">The starting point</param>
        /// <param name="Length">The maximum length of the new string</param>
        /// <returns>Either a string or blob Value</returns>
        public static Cell Substring(Cell C, long Position, long Length)
        {

            if (C.AFFINITY == CellAffinity.BLOB)
            {
                if (Position + Length > C.BLOB.Length || Position < 0 || Length < 0)
                    return CellValues.NullBLOB;
                byte[] b = new byte[Length];
                Array.Copy(C.BLOB, Position, b, 0, Length);
                C.BLOB = b;
                return C;
            }
            else if (C.AFFINITY == CellAffinity.TEXT)
            {
                if (Position + Length > C.valueSTRING.Length || Position < 0 || Length < 0)
                    return CellValues.NullTEXT;
                return new Cell(C.valueSTRING.Substring((int)Position, (int)Length), true);
            }
            else
            {
                if (Position + Length > C.valueSTRING.Length || Position < 0 || Length < 0)
                    return CellValues.NullSTRING;
                return new Cell(C.valueSTRING.Substring((int)Position, (int)Length), false);
            }

        }

        /// <summary>
        /// Replaces all occurances of a string/BLOB Value with another string/BLOB Value
        /// </summary>
        /// <param name="Source">The string to be searched</param>
        /// <param name="LookFor">The string being searched for</param>
        /// <param name="ReplaceWith">The string that serves as the replacement</param>
        /// <returns>Cell string Value</returns>
        public static Cell Replace(Cell Source, Cell LookFor, Cell ReplaceWith)
        {

            if (Source.AFFINITY == CellAffinity.BOOL || Source.AFFINITY == CellAffinity.DATE
                || Source.AFFINITY == CellAffinity.DOUBLE || Source.AFFINITY == CellAffinity.LONG)
                return new Cell(Source.AFFINITY);

            if (!(Source.AFFINITY == CellAffinity.BLOB && LookFor.AFFINITY == CellAffinity.BLOB && ReplaceWith.AFFINITY == CellAffinity.BLOB))
            {
                Source.STRING = Source.valueSTRING.Replace(LookFor.valueSTRING, ReplaceWith.valueSTRING);
                Source.AFFINITY = CellAffinity.STRING;
                Source.ULONG = 0;
                return Source;
            }

            string t = BitConverter.ToString(Source.BLOB);
            string u = BitConverter.ToString(LookFor.BLOB);
            string v = BitConverter.ToString(ReplaceWith.BLOB);
            t = t.Replace(u, v).Replace("-", "");
            Source.BLOB = CellParser.Parse(t, CellAffinity.BLOB).BLOB;
            return Source;

        }

        /// <summary>
        /// Given a pattern and a string or blob, this function will try to seek out the starting position of a patern
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Pattern"></param>
        /// <param name="StartAt"></param>
        /// <returns>Either an integer indicating the position of the pattern or NULL if the pattern wasn't found</returns>
        public static Cell Position(Cell Source, Cell Pattern, int StartAt)
        {

            if (StartAt < 0)
                StartAt = 0;

            if (StartAt > CellSerializer.Length(Source))
                return CellValues.NullINT;

            if (Source.AFFINITY == CellAffinity.STRING)
            {
                int idx = Source.STRING.IndexOf(Pattern.valueSTRING, StartAt);
                if (idx == -1)
                    return CellValues.NullINT;
                return new Cell(Source.STRING.IndexOf(Pattern.valueSTRING, StartAt));
            }

            if (Source.AFFINITY == CellAffinity.BLOB)
            {

                byte[] data = Source.BLOB;
                byte[] pattern = Pattern.valueBLOB;
                bool match = false;
                for (int i = StartAt; i < data.Length; i++)
                {

                    match = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (i + j >= data.Length)
                        {
                            match = false;
                            break;
                        }
                        if (data[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }

                    }

                    if (match)
                    {
                        return new Cell(i);
                    }

                }

            }

            return CellValues.NullINT;

        }



        /// <summary>
        /// Converts a byte array to a string using UTF16 encoding
        /// </summary>
        /// <param name="Hash"></param>
        /// <returns></returns>
        internal static string ByteArrayToUTF16String(byte[] Hash)
        {

            byte[] to_convert = Hash;
            if (Hash.Length % 2 != 0)
            {
                to_convert = new byte[Hash.Length + 1];
                Array.Copy(Hash, to_convert, Hash.Length);
            }

            return ASCIIEncoding.BigEndianUnicode.GetString(to_convert);

        }

        /// <summary>
        /// Performs an optimized integer power
        /// </summary>
        /// <param name="Base">The base Value</param>
        /// <param name="Exp">The exponent</param>
        /// <returns>Another integer: Base ^ Exp</returns>
        internal static long IntPower(long Base, long Exp)
        {

            if (Exp == 0)
                return 1;
            else if (Exp == 1)
                return Base;

            if ((Exp % 2) == 1)
                return IntPower(Base * Base, Exp / 2) * Base;
            else
                return IntPower(Base * Base, Exp / 2);

        }

        /// <summary>
        /// Calculates the integer root
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static long IntRoot(long Value)
        {

            if (Value <= 0)
                return 0;

            long x = (Value / 2) + 1;
            long y = (x + Value / x) / 2;
            while (y < x)
            {
                x = y;
                y = (x + Value / x) / 2;
            }

            return x;

        }

        /// <summary>
        /// Gets the index of the highest bit //
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        internal static int HighestBit(long Value)
        {

            long mask = 1;

            for (int i = 0; i < 63; i++)
            {

                if ((mask & Value) == mask)
                    return i;

                mask = mask << 1;

            }

            return 0;

        }


    }


}
