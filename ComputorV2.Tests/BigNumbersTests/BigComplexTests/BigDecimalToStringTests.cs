using BigNumbers;
using NUnit.Framework;

namespace ComputorV2.Tests.BigNumbersTests.BigComplexTests
{
    public class BigDecimalToStringTests
    {
        [Test]
        [TestCase("i", "i")]
        [TestCase("2i", "2i")]
        [TestCase("-2i", "-2i")]
        [TestCase("-1i", "-i")]
        [TestCase("-1-i", "-1-i")]
        [TestCase("-1+2i", "-1+2i")]
        [TestCase("-0+2i", "2i")]
        [TestCase("-2+0i", "-2")]
        [TestCase("-0+0i", "0")]
        public void ToString_WhenCalled_ReturnsStringValue(string input, string expected)
        {
            var testedNumber = new BigComplex(input);

            var actual = testedNumber.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}