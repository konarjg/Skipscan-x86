using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skipscan_x86
{
    public static class Prefix
    {
        public static byte[] REX = new byte[16];
        public static byte[] LEGACY = { 240, 242, 243, 38, 46, 54, 62, 100, 101, 102, 103 };

        public static void Initialize()
        {
            for (byte i = 0; i < REX.Length; ++i)
                REX[i] = (byte)(64 + i);
        }

        public static ByteWord RandomPrefixCombination(int length, Random generator)
        {
            var word = new ByteWord(length);

            for (int i = 0; i < length; ++i)
            {
                var type = generator.Next(0, 2);
                int index;

                if (type == 0)
                {
                    index = generator.Next(0, REX.Length);
                    word.SetByte(i, REX[index]);
                }
                else
                {
                    index = generator.Next(0, LEGACY.Length);
                    word.SetByte(i, LEGACY[index]);
                }
            }

            return word;
        }

        public static ByteWord RandomPrefixCombination(Random generator)
        {
            var length = generator.Next(0, 6);
            var word = new ByteWord(length);

            for (int i = 0; i < length; ++i)
            {
                var type = generator.Next(0, 2);
                int index;

                if (type == 0)
                {
                    index = generator.Next(0, REX.Length);
                    word.SetByte(i, REX[index]);
                }
                else
                {
                    index = generator.Next(0, LEGACY.Length);
                    word.SetByte(i, LEGACY[index]);
                }
            }

            return word;
        }

        public static bool IsPrefix(byte prefix)
        {
            return REX.Contains(prefix) || LEGACY.Contains(prefix); 
        }

        public static bool IsREX(byte prefix)
        {
            return REX.Contains(prefix);
        }

        private static bool IsREXCorrectlyPlaced(ByteWord combination)
        {
            for (int i = 0; i < combination.Length() - 1; ++i)
            {
                var current = combination.GetByte(i);
                var next = combination.GetByte(i + 1);

                if (!IsREX(current))
                    continue;

                if (!IsREX(next))
                    return false;
            }

            return true;
        }

        private static bool IsLengthValid(ByteWord combination)
        {
            return combination.Length() > 0 && combination.Length() <= 4;
        }

        private static bool IsREXCountValid(ByteWord combination)
        {
            int count = 0;

            for (int i = 0; i < combination.Length(); ++i)
            {
                if (IsREX(combination.GetByte(i)))
                    ++count;
            }

            return count == 1;
        }

        private static bool ArePrefixesDistinct(ByteWord combination)
        {
            for (int i = 0; i < combination.Length(); ++i)
            {
                int count = 0;

                for (int j = 0; j < combination.Length(); ++j)
                {
                    if (combination.GetByte(j) == combination.GetByte(i))
                        ++count;
                }

                if (count > 1)
                    return false;
            }

            return true;
        }

        public static bool IsPrefixCombinationValid(ByteWord combination)
        {
            for (int i = 0; i < combination.Length(); ++i)
            {
                if (!IsPrefix(combination.GetByte(i)))
                    return false;
            }

            return IsREXCorrectlyPlaced(combination) && IsLengthValid(combination) && IsREXCountValid(combination) && ArePrefixesDistinct(combination);
        }
    }
}
