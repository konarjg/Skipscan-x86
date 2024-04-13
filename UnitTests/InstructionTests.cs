using Skipscan_x86;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class InstructionTests
    {
        [TestMethod]
        public void InstructionWithInvalidPrefixShouldNotBeValid()
        {
            var generator = new Random();
            var hex1 = "0x40666765";
            var hex2 = "0x6667656640";

            var word1 = ByteWord.FromHex(hex1);
            var word2 = ByteWord.FromHex(hex2);

            var instruction1 = new Instruction(word1.Length(), 15);
            var instruction2 = new Instruction(word2.Length(), 15);
           
            instruction1.FillRandomly(generator);
            instruction1.FillPrefixBytes(word1);
            instruction2.FillRandomly(generator);
            instruction2.FillPrefixBytes(word1);

            Assert.IsFalse(instruction1.IsValid());
            Assert.IsFalse(instruction2.IsValid());
        }

        [TestMethod]
        public void InstructionWithLengthOverFifteenShouldNotBeValid()
        {
            var generator = new Random();

            var instruction = new Instruction(4, 16);

            Assert.IsFalse(instruction.IsValid());
        }
    }
}
