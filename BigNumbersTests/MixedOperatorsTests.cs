using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests
{
    public class MixedOperatorsTests
    {
        [Test]
        [TestCase("2", "2", "4")]
        [TestCase("2", "i", "2 + i")]
        [TestCase("2", "-i", "2 - i")]
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
        //swap bd and bc
        //div for compl/bd
    }
}
