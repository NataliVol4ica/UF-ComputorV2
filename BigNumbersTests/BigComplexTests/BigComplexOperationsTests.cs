using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigComplexTests
{
    public class BigComplexOperationsTests
    {
        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("0", "i", "i")]
        [TestCase("i", "0", "i")]
        [TestCase("i", "i", "2i")]
        [TestCase("i", "-i", "0")]
        [TestCase("i", "-2i", "-i")]
        [TestCase("2+i", "-2i", "2-i")]
        [TestCase("-2+i", "1-2i", "-1-i")]
        [TestCase("-2+i", "0", "-2+i")]
        public void Add_WhenCalled_ReturnsSum(string a, string b, string expected)
        {
            var left = new BigComplex(a);
            var right = new BigComplex(b);

            var actual = (left + right).ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("0", "i", "-i")]
        [TestCase("i", "0", "i")]
        [TestCase("i", "i", "0")]
        [TestCase("i", "-i", "2i")]
        [TestCase("i", "-2i", "3i")]
        [TestCase("2+i", "-2i", "2+3i")]
        [TestCase("-2+i", "1-2i", "-3+3i")]
        [TestCase("-2+i", "-1+2i", "-1-i")]
        [TestCase("-2+i", "0", "-2+i")]
        public void Sub_WhenCalled_ReturnsSub(string a, string b, string expected)
        {
            var left = new BigComplex(a);
            var right = new BigComplex(b);

            var actual = (left - right).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("0", "i", "0")]
        [TestCase("i", "i", "-1")]
        [TestCase("i", "-i", "1")]
        [TestCase("i", "-2i", "2")]
        [TestCase("2+i", "-2i", "2-4i")]
        [TestCase("-2+i", "1-2i", "5i")]
        [TestCase("-2+i", "-1+2i", "-5i")]
        [TestCase("-2+i", "3", "-6+3i")]
        public void Mul_WhenCalled_ReturnsMul(string a, string b, string expected)
        {
            var left = new BigComplex(a);
            var right = new BigComplex(b);

            var actual = (left * right).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("0", "i", "0")]
        [TestCase("i", "i", "1")]
        [TestCase("i", "-i", "-1")]
        [TestCase("i", "-2i", "-0.5")]
        [TestCase("2+i", "-2i", "-0.5+i")]
        [TestCase("-2+i", "1-2i", "-0.8-0.6i")]
        [TestCase("-2+i", "-1+2i", "0.8+0.6i")]
        [TestCase("-3+i", "2", "-1.5+0.5i")]
        [TestCase("-2+i", "1-i", "-1.5-0.5i")]
        public void Div_WhenCalled_ReturnsDiv(string a, string b, string expected)
        {
            var left = new BigComplex(a);
            var right = new BigComplex(b);

            var actual = (left / right).ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("i", "2", "-1")]
        public void Pow_WhenCalled_ReturnsPow(string a, string pow, string expected)
        {
            var left = new BigComplex(a);
            var right = new BigDecimal(pow);

            var actual = left.Pow(right).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}