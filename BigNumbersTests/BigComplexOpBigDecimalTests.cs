using System;
using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests
{
    public class BigComplexOpBigDecimalTests
    {
        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("i", "2", "2+i")]
        [TestCase("-i", "2", "2-i")]
        [TestCase("2-i", "2", "4-i")]
        [TestCase("2+7i", "2", "4+7i")]
        public void BigComplexAndBigDecimal_Add_ShouldGiveProperResult(string com, string dec, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bc + bd).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("2", "2", "0")]
        [TestCase("i", "2", "-2+i")]
        [TestCase("2+i", "2", "i")]
        [TestCase("2-i", "-2", "4-i")]
        public void BigComplexAndBigDecimal_Sub_ShouldGiveProperResult(string com, string dec, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bc - bd).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("2i", "2", "4i")]
        [TestCase("-2i", "2", "-4i")]
        [TestCase("2-2i", "2", "4-4i")]
        [TestCase("3-i", "2", "6-2i")]
        public void BigComplexAndBigDecimal_Mul_ShouldGiveProperResult(string com, string dec, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bc * bd).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("2", "2", "1")]
        [TestCase("2i", "2", "i")]
        [TestCase("15i", "3", "5i")]
        [TestCase("15i", "-3", "-5i")]
        [TestCase("-15i", "3", "-5i")]
        [TestCase("100-15i", "-5", "-20+3i")]
        public void BigComplexAndBigDecimal_Div_ShouldGiveProperResult(string com, string dec, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bc / bd).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void BigComplexAndBigDecimal_Mod_ShouldThrowException()
        {
            //Arrange
            var bd = new BigDecimal("3");
            var bc = new BigComplex("5i");

            //Act & Assert
            Assert.Throws<NotImplementedException>(()=> (bc % bd).ToString());
        }
    }
}