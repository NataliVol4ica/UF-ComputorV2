using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    public class BigDecimalIsIntegerTests
    {
        [Test]
        [TestCase("0", true)]
        [TestCase("0.000000", true)]
        [TestCase("1232423423", true)]
        [TestCase("-1232423423", true)]
        [TestCase("1.0000000000", true)]
        [TestCase("1.00000001", false)]
        [TestCase("0.2", false)]
        public void IsInteger_WhenCalled_ReturnsResult(string number, bool isInteger)
        {
            var testedNumber = new BigDecimal(number);
            var actual = testedNumber.IsInteger;

            Assert.AreEqual(isInteger, actual);
        }
    }
}