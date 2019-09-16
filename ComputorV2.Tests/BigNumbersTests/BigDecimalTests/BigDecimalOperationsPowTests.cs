using BigNumbers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputorV2.Tests.BigNumbersTests.BigDecimalTests
{
    public class BigDecimalOperationsPowTests
    {
        [SetUp]
        public void Setup() { }
        [Test]
        public void Pow_WhenCalledWithNotBigDecimal_ThrowsEXception()
        {
            var fakeBigNumber = new Mock<BigNumber>();
            var numToPow = new BigDecimal("5");

            Assert.Throws<ArgumentException>(() =>
            numToPow.Pow(fakeBigNumber.Object));
        }
        [Test]
        public void Pow_WhenCalledWithNotInteger_ThrowsEXception() {
            var fakeBigDecimal = new Mock<BigDecimal>();
            fakeBigDecimal.SetupGet(bd => bd.IsInteger).Returns(false);
            var numToPow = new BigDecimal("5");

            Assert.Throws<ArgumentException>(() =>
            numToPow.Pow(fakeBigDecimal.Object));
        }
        [Test]
        public void Pow_WhenCalledWithZero_ReturnsOne()
        {
            var numToPow = new BigDecimal("5");

            var actualString = numToPow.Pow(new BigDecimal("0")).ToString();
            Assert.AreEqual("1", actualString);
        }
        [Test]
        [TestCase("1", "1", "1")]
        [TestCase("0", "2", "0")]
        [TestCase("1", "2", "1")]
        [TestCase("1", "100", "1")]
        [TestCase("1", "1000000000", "1")]
        [TestCase("2", "10", "1024")]
        public void Pow_WhenCalledWithPositivePows_ReturnsPow(string str, string pow, string expected)
        {
            var numToPow = new BigDecimal(str);

            var actualString = numToPow.Pow(new BigDecimal(pow)).ToString();
            Assert.AreEqual(expected, actualString);
        }
        [Test]
        [TestCase("1", "-1", "1")]
        [TestCase("2", "-1", "0.5")]
        [TestCase("2", "-2", "0.25")]
        [TestCase("5", "-3", "0.008")]
        public void Pow_WhenCalledWithNegativePows_ReturnsPow(string str, string pow, string expected)
        {
            var numToPow = new BigDecimal(str);

            var actualString = numToPow.Pow(new BigDecimal(pow)).ToString();
            Assert.AreEqual(expected, actualString);
        }
    }
}
