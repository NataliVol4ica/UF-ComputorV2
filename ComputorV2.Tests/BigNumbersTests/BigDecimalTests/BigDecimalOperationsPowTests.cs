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
        public void Pow_WhenCalledWithNotInteger_ThrowsEXception() { }
        [Test]
        public void Pow_WhenCalledWithZero_ReturnsOne() { }
        [Test]
        public void Pow_WhenCalledWithPositivePows_ReturnsPow() { }
        [Test]
        [TestCase("1", "-1", "1")]
        [TestCase("2", "-1", "0.5")]
        [TestCase("2", "-2", "0.25")]
        [TestCase("5", "-3", "0.008")]
        public void Pow_WhenCalledWithNegativePows_ReturnsPow() { }
    }
}
