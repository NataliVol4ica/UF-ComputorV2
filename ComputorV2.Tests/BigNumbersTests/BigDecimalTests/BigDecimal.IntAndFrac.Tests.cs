using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    [TestFixture]
    public class BigDecimalIntFracTests
    {
        [Test]
        public void Zero()
        {
            BigDecimal actual = new BigDecimal("0");

            Assert.AreEqual(1, actual.IntegerLength);
            Assert.AreEqual(0, actual.FractionalLength);
        }
        [Test]
        public void Hundred()
        {
            BigDecimal actual = new BigDecimal("-100");

            Assert.AreEqual(3, actual.IntegerLength);
            Assert.AreEqual(0, actual.FractionalLength);
        }
        [Test]
        public void ZeroDot156()
        {
            BigDecimal actual = new BigDecimal("-0.156");

            Assert.AreEqual(1, actual.IntegerLength);
            Assert.AreEqual(3, actual.FractionalLength);
        }
        [Test]
        public void BigNumber()
        {
            BigDecimal actual = new BigDecimal("923742.42382");

            Assert.AreEqual(6, actual.IntegerLength);
            Assert.AreEqual(5, actual.FractionalLength);
        }       
    }
}
