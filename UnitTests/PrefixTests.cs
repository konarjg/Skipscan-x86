using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Skipscan_x86;

namespace UnitTests
{
    [TestClass]
    public class PrefixTests
    {
        [TestMethod]
        public void PrefixCombinationWithREXPrefixBeforeLegacyPrefixShouldNotBeValid()
        {
            var testCase1 = "0x40666567";
            var testCase2 = "0x65436567";
            var testCase3 = "0x65674567";
            var testCase4 = "0x6567664A";

            var result1 = Prefix.IsPrefixCombinationValid(ByteWord.FromHex(testCase1));
            var result2 = Prefix.IsPrefixCombinationValid(ByteWord.FromHex(testCase2));
            var result3 = Prefix.IsPrefixCombinationValid(ByteWord.FromHex(testCase3));
            var result4 = Prefix.IsPrefixCombinationValid(ByteWord.FromHex(testCase4));
            
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
            Assert.IsFalse(result3);
            Assert.IsFalse(result4);
        }

        [TestMethod]
        public void PrefixCombinationWithLengthZeroShouldNotBeValid()
        {
            var result = Prefix.IsPrefixCombinationValid(new ByteWord(0));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PrefixCombinationWithLengthOverFourShouldNotBeValid()
        {
            var result = Prefix.IsPrefixCombinationValid(new ByteWord(5));
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PrefixCombinationWithRepeatedPrefixesShouldNotBeValid()
        {
            var testCase1 = "0x66666540";
            var testCase2 = "0x66666666";
            var result1 = Prefix.IsPrefixCombinationValid(ByteWord.FromHex(testCase1));
            var result2 = Prefix.IsPrefixCombinationValid(ByteWord.FromHex(testCase2));
            Assert.IsFalse(result1);
            Assert.IsFalse(result2);
        }
    }
}