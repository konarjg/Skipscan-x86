using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skipscan_x86
{
    public class Instruction
    {
        public ByteWord PrefixBytes { get; private set; }
        public ByteWord InstructionBytes { get; private set; }
        public ByteWord FullInstructionBytes { get => PrefixBytes + InstructionBytes; }

        public Instruction(int prefixLength, int length)
        {
            PrefixBytes = new ByteWord(prefixLength);
            InstructionBytes = new ByteWord(length - prefixLength);
        }

        public bool AreEqual(Instruction instruction)
        {
            return instruction.FullInstructionBytes.AreEqual(FullInstructionBytes);
        }

        public int Length()
        {
            return FullInstructionBytes.Length();
        }

        public void FillPrefixBytes(ByteWord bytes)
        {
            PrefixBytes = bytes;
        }

        public void FillInstructionBytes(ByteWord bytes)
        {
            InstructionBytes = bytes;
        }

        public void FillRandomly(Random generator)
        {
            PrefixBytes = Prefix.RandomPrefixCombination(PrefixBytes.Length(), generator);
            InstructionBytes.FillRandomly(generator);
        }

        public bool ContainsTriggersOrFeatureValues()
        {
            byte feature1 = 2;
            byte feature2 = 1;
            byte trigger = 255;

            return InstructionBytes.Contains(feature1)
                    || InstructionBytes.Contains(feature2)
                    || InstructionBytes.Contains(trigger);
        }

        public bool IsValid()
        {
            return FullInstructionBytes.Length() <= 15 && Prefix.IsPrefixCombinationValid(PrefixBytes) && !ContainsTriggersOrFeatureValues();
        }

        public override string ToString()
        {
            return FullInstructionBytes.ToString();
        }
    }
}
