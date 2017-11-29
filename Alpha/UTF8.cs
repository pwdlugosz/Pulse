using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.Alpha
{
    
    public sealed class UTF8
    {

        private byte[] _elements;

        public UTF8(byte[] Data)
        {
            this._elements = Data;
        }
        
        public UTF8(string Text)
            : this(UTF8Encoding.StringToBytes(Text))
        {
        }

        public UTF8(int Length)
            :this(new byte[Length])
        {
        }

        public int Length
        {
            get { return this._elements.Length; }
        }

        public byte[] ToByteArray
        {
            get
            {
                byte[] b = new byte[this.Length];
                Array.Copy(this._elements, 0, b, 0, b.Length);
                return b;
            }
        }

        public byte this[int Index]
        {
            get { return this._elements[Index]; }
        }

        // String Functions //
        public UTF8 Trim(byte[] Elements)
        {

            List<byte> vals = new List<byte>();
            foreach (byte b in this._elements)
            {
                if (!Elements.Contains(b))
                    vals.Add(b);
            }
            return new UTF8(vals.ToArray());

        }

        public UTF8 Trim()
        {
            return this.Trim(UTF8Encoding.WhiteSpace);
        }

        public UTF8 Substring(int Start, int Length)
        {

            if (Start + Length > this.Length)
                throw new IndexOutOfRangeException();

            byte[] b = new byte[Length];
            Array.Copy(this._elements, Start, b, 0, Length);
            return new UTF8(b);

        }

        public UTF8 Left(int Length)
        {
            return this.Substring(0, Length);
        }

        public UTF8 Right(int Length)
        {
            return this.Substring(this.Length - Length, Length);
        }

        public UTF8 ToLower()
        {
            byte[] b = new byte[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                b[i] = UTF8Encoding.ToLower(this._elements[i]);
            }
            return new UTF8(b);
        }

        public UTF8 ToUpper()
        {
            byte[] b = new byte[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                b[i] = UTF8Encoding.ToUpper(this._elements[i]);
            }
            return new UTF8(b);
        }

        public int Find(UTF8 Value, int StartAt)
        {

            if (Value.Length > this.Length - StartAt)
                return -1;

            bool found = false;
            for (int i = StartAt; i < this.Length - Value.Length; i++)
            {

                found = true;
                for (int j = 0; j < Value.Length; j++)
                {
                    if (this[i + j] != Value[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return i;

            }

            return -1;

        }

        public int Find(UTF8 Value)
        {
            return this.Find(Value, 0);
        }

        //public int[] FindAll(UTF8 Value, int StartAt)
        //{
        //}

        //public UTF8 Replace(UTF8 Old, UTF8 New)
        //{
        //}

        //public UTF8 Remove(UTF8 Value)
        //{
        //}

        // Compare Functions //
        public static int CompareStrict(UTF8 A, UTF8 B)
        {

            if (A.Length != B.Length)
                return A.Length - B.Length;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i])
                    return (int)A[i] - (int)B[i];
            }

            return 0;

        }

        public static int CompareWeak(UTF8 A, UTF8 B)
        {

            for (int i = 0; i < Math.Min(A.Length, B.Length); i++)
            {
                if (A[i] != B[i])
                    return (int)A[i] - (int)B[i];
            }

            return 0;

        }

        public static int CompareStrictIgnoreCase(UTF8 A, UTF8 B)
        {

            if (A.Length != B.Length)
                return A.Length - B.Length;

            for (int i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i])
                    return (int)UTF8Encoding.ToLower(A[i]) - (int)UTF8Encoding.ToLower(B[i]);
            }

            return 0;

        }

        public static int CompareWeakIgnoreCase(UTF8 A, UTF8 B)
        {

            for (int i = 0; i < Math.Min(A.Length, B.Length); i++)
            {
                if (A[i] != B[i])
                    return (int)UTF8Encoding.ToLower(A[i]) - (int)UTF8Encoding.ToLower(B[i]);
            }

            return 0;

        }


        // Helpers //

        public class UTF8Builder
        {

            private List<byte> _Values;

            public UTF8Builder()
            {
                this._Values = new List<byte>();
            }
            
            public void AppendLine()
            {
                this._Values.Add(UTF8Encoding.CarriageReturn);
                this._Values.Add(UTF8Encoding.LineFeed);
            }
            
            public void Append(byte Value)
            {
                this._Values.Add(Value);
            }

            public void AppendLine(byte Value)
            {
                this.Append(Value);
                this.AppendLine();
            }
            
            public void Append(char Value)
            {
                this._Values.Add(UTF8Encoding.CharToByte(Value));
            }

            public void AppendLine(char Value)
            {
                this.Append(Value);
                this.AppendLine();
            }

            public void Append(UTF8 Value)
            {
                foreach(byte b in Value._elements)
                {
                    this._Values.Add(b);
                }
            }

            public void AppendLine(UTF8 Value)
            {
                this.Append(Value);
                this.AppendLine();
            }
            
            public void Append(string Value)
            {
                this.Append(UTF8Encoding.StringToBytes(Value));
            }
            
            public void AppendLine(string Value)
            {
                this.Append(UTF8Encoding.StringToBytes(Value));
                this.AppendLine();
            }

            public void Append(byte[] Value)
            {
                foreach(byte b in Value)
                {
                    this._Values.Add(b);
                }
            }
            
            public void AppendLine(byte[] Value)
            {
                this.Append(Value);
                this.AppendLine();
            }
            
            public void Append(char[] Value)
            {
                foreach(char c in Value)
                {
                    this.Append(c);
                }
            }
            
            public void AppendLine(char[] Value)
            {
                this.Append(Value);
                this.AppendLine();
            }

            public UTF8 ToUTF8()
            {
                return new UTF8(this._Values.ToArray());
            }

        }

    }


    public static class UTF8Encoding
    {

        public static byte Tab
        {
            get { return 9; }
        }

        public static byte LineFeed
        {
            get { return 10; }
        }

        public static byte CarriageReturn
        {
            get { return 13; }
        }

        public static byte Space
        {
            get { return 32; }
        }

        public static byte[] WhiteSpace
        {
            get { return new byte[]{9,10,11,12,13,32,133,160}; } 
        }

        public static byte CharToByte(char Value)
        {
            return (byte)(Value & 255);
        }

        public static char ByteToChar(byte Value)
        {
            return (char)Value;
        }

        public static bool IsLatinChar(byte Value)
        {
            return (Value >= 65 && Value <= 90) || (Value >= 97 && Value <= 122);
        }

        public static bool IsNumeric(byte Value)
        {
            return (Value >= 48 && Value <= 57);
        }

        public static bool IsWhiteSpace(byte Value)
        {
            return (Value >= 9 && Value <= 13) || Value == 32 || Value == 133 || Value == 160;
        }
        
        public static byte[] StringToBytes(string Text)
        {

            byte[] b = new byte[Text.Length];
            int i = 0;
            foreach(char c in Text)
            {
                b[i] = (byte)(c & 255);
                i++;
            }
            return b;

        }

        public static string BytesToString(byte[] Binary)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in Binary)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
        }

        public static byte ToUpper(byte Value)
        {
            return (byte)(Value >= 97 && Value <= 122 ? Value - 32 : Value);
        }

        public static byte ToLower(byte Value)
        {
            return (byte)(Value >= 65 && Value <= 90 ? Value + 32 : Value);
        }

    }

}
