using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Skipscan_x86
{
    public static class InstructionsGenerator
    {
        private static void Print(ref byte[] elements, ref int[] combination, int length)
        {
            for (int i = 1; i <= length; ++i)
                Console.Write(elements[combination[i]]);

            Console.WriteLine();
        }

        private static ByteWord GenerateByteCombination(ref byte[] elements, ref int[] combination, int length)
        {
            var bytes = new byte[length];

            for (int i = 1; i <= length; ++i)
                bytes[i - 1] = elements[combination[i]];

            return new ByteWord(bytes);
        }

        private static byte[] InitializeElements()
        {
            var elements = new byte[256];

            for (int i = 0; i < 256; ++i)
                elements[i] = (byte)i;

            return elements;
        }

        private static int[] InitializeCombinations(int k, int n)
        {
            var c = new int[k + 3];

            for (int i = 1; i <= k; ++i)
                c[i] = i - 1;

            c[k + 1] = n;
            c[k + 2] = 0;

            return c;
        }

        private static Instruction GenerateInstructionWithRandomPrefix(Random generator, ByteWord bytes, int prefixLength, int instructionLength)
        {
            var instruction = new Instruction(prefixLength, instructionLength);
            instruction.FillRandomly(generator);
            instruction.FillInstructionBytes(bytes);

            return instruction;
        }

        public static List<Instruction> GenerateRandom(Random generator, int prefixLength, int length, int count)
        {
            var instructions = new List<Instruction>();

            for (int i = 0; i < count; ++i)
            {
                var instruction = new Instruction(prefixLength, length);
                instruction.FillRandomly(generator);

                while (instructions.Find(inst => inst.AreEqual(instruction)) != null)
                    instruction.FillRandomly(generator);

                if (instruction.IsValid() && instructions.Find(inst => inst.PrefixBytes.AreSimilar(instruction.PrefixBytes)) == null)
                    instructions.Add(instruction);
            }

            return instructions;
        }

        public static List<Instruction> GenerateLexicographicWithRandomPrefixes(Random generator, int prefixLength, int length, int count)
        {
            var elements = InitializeElements();
            
            int n = elements.Length;
            int k = length - prefixLength;

            var c = InitializeCombinations(k, n);
            var combinations = new List<Instruction>();

            int j = 1;
            int foundSoFar = 0;

            while (j <= k)
            {
                j = 1;

                if (foundSoFar > count)
                    return combinations;

                var bytes = GenerateByteCombination(ref elements, ref c, k);
                var instruction = GenerateInstructionWithRandomPrefix(generator, bytes, prefixLength, length);

                if (instruction.IsValid() && combinations.Find(inst => inst.PrefixBytes.AreSimilar(instruction.PrefixBytes)) == null)
                    combinations.Add(instruction);

                while (c[j + 1] == c[j] + 1)
                {
                    c[j] = j - 1;
                    ++j;
                }

                ++c[j];
                ++foundSoFar;
            }

            return combinations;
        }
    }
}
