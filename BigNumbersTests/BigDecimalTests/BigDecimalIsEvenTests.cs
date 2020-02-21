using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    public class BigDecimalIsEvenTests
    {
        [Test]
        public void IsEven_WhenCalledOnFracNumber_ReturnsFalse()
        {
            var testedNumber = new BigDecimal("1.1");

            Assert.IsFalse(testedNumber.IsEven);
        }

        [Test]
        public void IsEven_WhenCalledOddEvenIntNumber_ReturnsFalse()
        {
            var testedNumber = new BigDecimal("1000000000000000000000000000001.0000");

            Assert.IsFalse(testedNumber.IsEven);
        }

        [Test]
        public void IsEven_WhenCalledOnEvenIntNumber_ReturnsTrue()
        {
            var testedNumber = new BigDecimal("1000000000000000002.0000");

            Assert.IsTrue(testedNumber.IsEven);
        }
    }
}