using System;
using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    public class BigDecimalSqrtUnitTests
    {
        public BigDecimalSqrtUnitTests()
        {

        }

        [Test]
        [TestCase("0", "0")]
        [TestCase("1", "1")]
        [TestCase("0.25", "0.5")]
        [TestCase("25", "5")]
        [TestCase("0.0625", "0.25")]
        public void Sqrt_WhenCalled_ShouldReturnResultWithPrecision(string number, string expected)
        {
            //Arrange
            var num = new BigDecimal(number);

            //Act
            BigDecimal actual = BigDecimal.Sqrt(num);

            //Assert
            Assert.AreEqual(expected, actual.ToString());
        }

        [Test]
        public void Sqrt_WhenCalledForNegativeNumber_ShouldThrowException()
        {
            //Arrange
            var num = new BigDecimal("-1");

            //Act & Assert
            Assert.Throws<ArgumentException> ( () => BigDecimal.Sqrt(num));
        }

        [Test]
        [TestCase("-1", "i")]
        [TestCase("-4", "2i")]
        [TestCase("-0.25", "0.5i")]
        public void ComplexSqrt_WhenCalledForNegativeNumber_ShouldReturnComplexResultWithPrecision(string number, string expected)
        {
            //Arrange
            var num = new BigDecimal(number);

            //Act
            BigComplex actual = BigDecimal.ComplexSqrt(num);

            //Assert
            Assert.AreEqual(expected, actual.ToString());
        }
    }
}
