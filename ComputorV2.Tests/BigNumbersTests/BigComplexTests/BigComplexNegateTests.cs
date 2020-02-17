using BigNumbers;
using NUnit.Framework;

namespace ComputorV2.Tests.BigNumbersTests.BigComplexTests
{
    public class BigComplexNegateTests
    {
        [Test]
        [TestCase("i", "-i")]
        [TestCase("2i", "-2i")]
        [TestCase("-2i", "2i")]
        [TestCase("1-2i", "-1+2i")]
        [TestCase("1+2i", "-1-2i")]
        [TestCase("-1+2i", "1-2i")]
        [TestCase("-1-2i", "1+2i")]
        [TestCase("-1-1i", "1+i")]
        [TestCase("0", "0")]
        public void Negate_WhenCalled_ReturnsNegatedNumber(string input, string expected)
        {
            var testedNumber = new BigComplex(input);

            var actual = (-testedNumber).ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}