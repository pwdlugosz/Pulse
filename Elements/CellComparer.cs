using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Elements
{


    /// <summary>
    /// Methods for comparing cells
    /// </summary>
    public static class CellComparer
    {

        public static bool Equals(Cell A, Cell B)
        {

            if (A.NULL == 1 && B.NULL == 1)
                return true;
            else if (A.NULL == 1 || B.NULL == 1)
                return false;

            if (A.AFFINITY == B.AFFINITY)
            {

                if (A.AFFINITY == CellAffinity.BOOL)
                    return A.BOOL == B.BOOL;
                else if (A.AFFINITY == CellAffinity.DATE)
                    return A.DATE == B.DATE;
                else if (A.AFFINITY == CellAffinity.BYTE)
                    return A.BYTE == B.BYTE;
                else if (A.AFFINITY == CellAffinity.SHORT)
                    return A.SHORT == B.SHORT;
                else if (A.AFFINITY == CellAffinity.INT)
                    return A.INT == B.INT;
                else if (A.AFFINITY == CellAffinity.LONG)
                    return A.LONG == B.LONG;
                else if (A.AFFINITY == CellAffinity.FLOAT)
                    return A.FLOAT == B.FLOAT;
                else if (A.AFFINITY == CellAffinity.DOUBLE)
                    return A.DOUBLE == B.DOUBLE;
                else if (A.AFFINITY == CellAffinity.BLOB)
                    return ByteArrayCompare(A.BLOB, B.BLOB) == 0;
                else if (A.AFFINITY == CellAffinity.TEXT || A.Affinity == CellAffinity.STRING)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.STRING, B.STRING) == 0;

            }
            else
            {

                if (A.AFFINITY == CellAffinity.STRING || B.AFFINITY == CellAffinity.STRING)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueSTRING, B.valueSTRING) == 0;
                else if (A.AFFINITY == CellAffinity.TEXT || B.AFFINITY == CellAffinity.TEXT)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueTEXT, B.valueTEXT) == 0;
                else if (A.AFFINITY == CellAffinity.BLOB || B.AFFINITY == CellAffinity.BLOB)
                    return ByteArrayCompare(A.BLOB, B.BLOB) == 0;
                else if (A.AFFINITY == CellAffinity.DOUBLE || B.AFFINITY == CellAffinity.DOUBLE)
                    return A.valueDOUBLE == B.valueDOUBLE;
                else if (A.AFFINITY == CellAffinity.FLOAT || B.AFFINITY == CellAffinity.FLOAT)
                    return A.valueFLOAT == B.valueFLOAT;
                else if (A.AFFINITY == CellAffinity.LONG || B.AFFINITY == CellAffinity.LONG)
                    return A.valueLONG == B.valueLONG;
                else if (A.AFFINITY == CellAffinity.INT || B.AFFINITY == CellAffinity.INT)
                    return A.valueINT == B.valueINT;
                else if (A.AFFINITY == CellAffinity.SHORT || B.AFFINITY == CellAffinity.SHORT)
                    return A.valueSHORT == B.valueSHORT;
                else if (A.AFFINITY == CellAffinity.BYTE || B.AFFINITY == CellAffinity.BYTE)
                    return A.valueBYTE == B.valueBYTE;
                else if (A.AFFINITY == CellAffinity.DATE || B.AFFINITY == CellAffinity.DATE)
                    return A.valueDATE == B.valueDATE;
                else if (A.valueBOOL == B.valueBOOL)
                    return A.valueBOOL == B.valueBOOL;

            }

            return false;

        }

        public static bool EqualsStrict(Cell A, Cell B)
        {

            if (A.AFFINITY != B.AFFINITY)
                return false;

            if (A.NULL == 1 && B.NULL == 1)
                return true;
            else if (A.NULL == 1 || B.NULL == 1)
                return false;

            if (A.AFFINITY == CellAffinity.BOOL)
                return A.BOOL == B.BOOL;
            else if (A.AFFINITY == CellAffinity.DATE)
                return A.DATE == B.DATE;
            else if (A.AFFINITY == CellAffinity.BYTE)
                return A.BYTE == B.BYTE;
            else if (A.AFFINITY == CellAffinity.SHORT)
                return A.SHORT == B.SHORT;
            else if (A.AFFINITY == CellAffinity.INT)
                return A.INT == B.INT;
            else if (A.AFFINITY == CellAffinity.LONG)
                return A.LONG == B.LONG;
            else if (A.AFFINITY == CellAffinity.FLOAT)
                return A.FLOAT == B.FLOAT;
            else if (A.AFFINITY == CellAffinity.DOUBLE)
                return A.DOUBLE == B.DOUBLE;
            else if (A.AFFINITY == CellAffinity.BLOB)
                return ByteArrayCompare(A.BLOB, B.BLOB) == 0;
            else if (A.AFFINITY == CellAffinity.TEXT || A.Affinity == CellAffinity.STRING)
                return StringComparer.OrdinalIgnoreCase.Compare(A.STRING, B.STRING) == 0;

            return false;

        }

        public static int Compare(Cell A, Cell B)
        {

            if (A.NULL == 1 && B.NULL == 1)
                return 0;
            else if (A.NULL == 1)
                return 1;
            else if (B.NULL == 1)
                return -1;

            if (A.AFFINITY == B.AFFINITY)
            {

                if (A.AFFINITY == CellAffinity.BOOL)
                    return (A.BOOL == B.BOOL ? 0 : (A.BOOL ? 1 : -1));
                else if (A.AFFINITY == CellAffinity.DATE)
                    return DateTime.Compare(A.DATE, B.DATE);
                else if (A.AFFINITY == CellAffinity.BYTE)
                    return (int)(A.BYTE - B.BYTE);
                else if (A.AFFINITY == CellAffinity.SHORT)
                    return (int)(A.SHORT - B.SHORT);
                else if (A.AFFINITY == CellAffinity.INT)
                    return (int)(A.INT - B.INT);
                else if (A.AFFINITY == CellAffinity.LONG)
                    return (int)(A.LONG - B.LONG);
                else if (A.AFFINITY == CellAffinity.FLOAT)
                    return (A.FLOAT == B.FLOAT ? 0 : (A.FLOAT < B.FLOAT ? -1 : 1));
                else if (A.AFFINITY == CellAffinity.DOUBLE)
                    return (A.DOUBLE == B.DOUBLE ? 0 : (A.DOUBLE < B.DOUBLE ? -1 : 1));
                else if (A.AFFINITY == CellAffinity.BLOB)
                    return ByteArrayCompare(A.BLOB, B.BLOB);
                else if (A.AFFINITY == CellAffinity.TEXT || A.Affinity == CellAffinity.STRING)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.STRING, B.STRING);

            }
            else
            {

                if (A.AFFINITY == CellAffinity.STRING || B.AFFINITY == CellAffinity.STRING)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueSTRING, B.valueSTRING);
                else if (A.AFFINITY == CellAffinity.TEXT || B.AFFINITY == CellAffinity.TEXT)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueTEXT, B.valueTEXT);
                else if (A.AFFINITY == CellAffinity.BLOB || B.AFFINITY == CellAffinity.BLOB)
                    return ByteArrayCompare(A.BLOB, B.BLOB);
                else if (A.AFFINITY == CellAffinity.DOUBLE || B.AFFINITY == CellAffinity.DOUBLE)
                    return (A.DOUBLE == B.DOUBLE ? 0 : (A.DOUBLE < B.DOUBLE ? -1 : 1));
                else if (A.AFFINITY == CellAffinity.FLOAT || B.AFFINITY == CellAffinity.FLOAT)
                    return (A.FLOAT == B.FLOAT ? 0 : (A.FLOAT < B.FLOAT ? -1 : 1));
                else if (A.AFFINITY == CellAffinity.LONG || B.AFFINITY == CellAffinity.LONG)
                    return (int)(A.valueLONG - B.valueLONG);
                else if (A.AFFINITY == CellAffinity.INT || B.AFFINITY == CellAffinity.INT)
                    return A.valueINT - B.valueINT;
                else if (A.AFFINITY == CellAffinity.SHORT || B.AFFINITY == CellAffinity.SHORT)
                    return A.valueSHORT - B.valueSHORT;
                else if (A.AFFINITY == CellAffinity.BYTE || B.AFFINITY == CellAffinity.BYTE)
                    return A.valueBYTE - B.valueBYTE;
                else if (A.AFFINITY == CellAffinity.DATE || B.AFFINITY == CellAffinity.DATE)
                    return DateTime.Compare(A.DATE, B.DATE);
                else if (A.valueBOOL == B.valueBOOL)
                    return (A.BOOL == B.BOOL ? 0 : (A.BOOL ? 1 : -1));

            }

            return 0;

        }

        internal static int ByteArrayCompare(byte[] A, byte[] B)
        {

            if (A.Length != B.Length)
                return A.Length - B.Length;

            int c = 0;
            for (int i = 0; i < A.Length; i++)
            {
                c = A[i] - B[i];
                if (c != 0)
                    return c;
            }
            return 0;

        }

    }


}
