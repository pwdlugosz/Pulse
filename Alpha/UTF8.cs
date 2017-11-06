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
            : this(StringToBytes(Text))
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

        // String Functions //
        
        // Compare Functions //

        // Helpers //
        public bool IsLatin(byte Element)
        {
            return (Element >
        }

        public bool IsNumeric(byte Element)
        {
        }


        private static byte[] StringToBytes(string Text)
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

        private static string BytesToString(byte[] Binary)
        {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in Binary)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
        }

    }

}
