using System;
using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests
{
    public class BigDecimalOpBigComplexTests
    {
        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("2", "i", "2+i")]
        [TestCase("2", "-i", "2-i")]
        [TestCase("2", "2-i", "4-i")]
        [TestCase("2", "2+7i", "4+7i")]
        public void BigDecimalAndBigComplex_Add_ShouldGiveProperResult(string dec, string com, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bd + bc).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        [TestCase("2", "2", "0")]
        [TestCase("2", "i", "2-i")]
        [TestCase("2", "-i", "2+i")]
        [TestCase("2", "2-i", "i")]
        [TestCase("2", "2+i", "-i")]
        public void BigDecimalAndBigComplex_Sub_ShouldGiveProperResult(string dec, string com, string expected)
        {

            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bd - bc).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        } 
        
        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("2", "2i", "4i")]
        [TestCase("2", "-2i", "-4i")]
        [TestCase("2", "2-2i", "4-4i")]
        [TestCase("2", "3-i", "6-2i")]
        public void BigDecimalAndBigComplex_Mul_ShouldGiveProperResult(string dec, string com, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bd * bc).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("2", "2", "1")]
        [TestCase("2", "2i", "-i")]
        public void BigDecimalAndBigComplex_Div_ShouldThrowException(string dec, string com, string expected)
        {
            //Arrange
            var bd = new BigDecimal(dec);
            var bc = new BigComplex(com);

            //Act
            var result = (bd / bc).ToString();

            //Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void BigDecimalAndBigComplex_Mod_ShouldThrowException()
        {
            //Arrange
            var bd = new BigDecimal("3");
            var bc = new BigComplex("5i");

            //Act & Assert
            Assert.Throws<NotImplementedException>(() => (bd % bc).ToString());
        }
    }
}
