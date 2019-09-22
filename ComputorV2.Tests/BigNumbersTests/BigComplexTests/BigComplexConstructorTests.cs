using NUnit.Framework;
using BigNumbers;
using System;

namespace ComputorV2.Tests.BigNumbersTests.BigComplexTests
{
    public class BigComplexConstructorTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("sdas")]
        [TestCase("2+2+i")]
        [TestCase("2+.i")]
        public void Constructor_WhenCalledWithInvalidString_ThrowsException(string input)
        {
           var exception = Assert.Throws<ArgumentException>(() =>  new BigComplex(input));
            if (!(input is null))
                StringAssert.Contains(input, exception.Message);
        }

        [Test]
        [TestCase("0", "0", "0", "0")]
        [TestCase("2", "2", "0", "2")]
        [TestCase("i", "0", "1", "i")]
        [TestCase("-I", "0", "-1", "-i")]
        [TestCase("  I  ", "0", "1", "i")]
        [TestCase("2i", "0", "2", "2i")]
        [TestCase("2.0i", "0", "2", "2i")]
        [TestCase("+2.0I", "0", "2", "2i")]
        [TestCase("2.0+i", "2", "1", "2+i")]
        [TestCase("2.0+3I", "2", "3", "2+3i")]
        [TestCase("-2.0   -  3i", "-2", "-3", "-2-3i")]
        public void Constructor_WhenCalledWithValidString_CreatesInstanceWithRealAndImaginary(
            string input, string eReal, string eImaginary, string eToString)
        {
            var complex = new BigComplex(input);

            Assert.AreEqual(eReal, complex.Real.ToString());
            Assert.AreEqual(eImaginary, complex.Imaginary.ToString());
            Assert.AreEqual(eToString, complex.ToString());
        }
    }
}
