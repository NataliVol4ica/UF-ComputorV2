using System;
using System.Text;
using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    [TestFixture]
    public class BigDecimalOperationsDivTests
    {
        static int i;
        static readonly Random rnd = new Random((int) DateTime.Now.Ticks);

        static void DoTesting(string left, string right, string result)
        {
            BigDecimal A = new BigDecimal(left);
            BigDecimal B = new BigDecimal(right);

            BigDecimal C = A / B;
            Assert.AreEqual(result, C.ToString());
        }

        public static string DecimalToString(decimal number)
        {
            string str = number.ToString();
            if (!str.Contains(","))
                return str;
            str = str.Replace(",", ".");
            StringBuilder sb = new StringBuilder(str);
            while (sb[sb.Length - 1] == '0')
                sb.Remove(sb.Length - 1, 1);
            if (sb[sb.Length - 1] == '.')
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        static void RandomTest()
        {
            int a = rnd.Next(0, Int32.MaxValue);
            int b = rnd.Next(0, Int32.MaxValue);
            a -= Int32.MaxValue / 2;
            b -= Int32.MaxValue / 2;
            decimal A = a;
            decimal B = b;
            A /= 10000;
            B /= 100000;
            decimal C = A * B;

            DoTesting(DecimalToString(C),
                DecimalToString(B),
                DecimalToString(A));
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("-5", "0", "0")]
        public void Division_WhenDividingOnZero_ThrowsException(string a, string b, string expexted)
        {
            Assert.Throws<DivideByZeroException>(() =>
                DoTesting(a, b, expexted));
        }

        [Test]
        [TestCase("0", "-5", "0")]
        [TestCase("-6", "5", "-1.2")]
        [TestCase("20.1", "0.05", "402")]
        [TestCase("20.1", "5", "4.02")]
        [TestCase("0", "-5", "0")]
        [TestCase("25", "100", "0.25")]
        [TestCase("25", "1000", "0.025")]
        [TestCase("25", "100000000", "0.00000025")]
        public void Division_WhenCalled_ReturnsExpectedResult(string a, string b, string expexted)
        {
            DoTesting(a, b, expexted);
        }

        [Test]
        public void Random_10000_tests()
        {
            for (i = 0; i < 10000; i++)
                RandomTest();
        }
    }
}