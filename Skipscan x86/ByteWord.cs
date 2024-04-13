using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skipscan_x86
{
    static class ByteUtils
    {
        public static byte ParseByte(this string s)
        {
            byte b = 0;
            byte power = 1;

            for (int i = s.Length - 1; i >= 0; --i)
            {
                byte digit = 0;

                if (s[i] >= 'a' && s[i] <= 'f')
                    digit = (byte)(s[i] - 'a' + 10);
                else
                    digit = (byte)(s[i] - '0');

                b += (byte)(digit * power);
                power *= 16;
            }

            return b;
        }

        public static string Hex(this byte b)
        {
            var hex = "";

            if (b == 0)
                return "00";

            while (b > 0)
            {
                var digit = b % 16;
                char hexDigit;

                if (digit > 9)
                    hexDigit = (char)('a' + digit - 10);
                else
                    hexDigit = (char)(digit + '0');

                hex = hexDigit + hex;
                b /= 16;
            }

            if (hex.Length != 2)
                hex = '0' + hex;

            return hex;
        }
    }

    public class ByteWord
    {
        private byte[] Values;

        public ByteWord(int length)
        {
            Values = new byte[length];
        }

        public ByteWord(params byte[] values)
        {
            Values = values;
        }

        public byte[] ToByteArray()
        {
            return Values;
        }

        public static ByteWord FromHex(string hex)
        {
            var hexDigits = hex.Split('x')[1];
            var word = new ByteWord(hexDigits.Length / 2);

            int j = 0;

            for (int i = 1; i < hexDigits.Length; i += 2)
            {
                var hexByte = hexDigits[i - 1] + "" + hexDigits[i];
                word.SetByte(j, hexByte.ParseByte());
                ++j;
            }

            return word;
        }

        public void FillRandomly(Random generator)
        {
            generator.NextBytes(Values);
        }

        public int Length()
        {
            return Values.Length;
        }

        public bool Contains(byte b)
        {
            return Values.Contains(b);
        }

        public bool AreEqual(ByteWord word)
        {
            if (word.Length() != Length())
                return false;

            for (int i = 0; i < word.Length(); ++i)
            {
                if (word.GetByte(i) != GetByte(i))
                    return false;
            }

            return true;
        }

        public bool AreSimilar(ByteWord word)
        {
            if (word.Length() != Length())
                return false;

            var bytes1 = Values.ToList();
            var bytes2 = word.Values.ToList();

            bytes1.Sort();
            bytes2.Sort();

            for (int i = 0; i < Length(); ++i)
            {
                if (bytes1[i] != bytes2[i])
                    return false;
            }

            return true;
        }

        public void SetBytes(int startIndex, params byte[] values)
        {
            for (int i = 0; i < values.Length; ++i)
                Values[startIndex + i] = values[i];
        }

        public void SetByte(int index, byte value)
        {
            Values[index] = value;
        }

        public byte GetByte(int index)
        {
            return Values[index];
        }

        public override string ToString()
        {
            var word = "0x";

            for (int i = 0; i < Values.Length; ++i)
                word += Values[i].Hex();

            return word;
        }

        public static ByteWord operator +(ByteWord word1, ByteWord word2)
        {
            var b1 = word1.Values;
            var b2 = word2.Values;
            var result = new ByteWord(b1.Length + b2.Length);

            for (int i = 0; i < b1.Length; ++i)
                result.SetByte(i, b1[i]);

            for (int i = b1.Length; i < b1.Length + b2.Length; ++i)
                result.SetByte(i, b2[i - b1.Length]);

            return result;
        }
    }
}
