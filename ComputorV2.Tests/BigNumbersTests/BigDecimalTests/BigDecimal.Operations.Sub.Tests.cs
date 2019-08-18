﻿using BigNumbers;
using System;
using System.Text;
using NUnit.Framework;

namespace BigNumbers.Tests.BigDecimalTests
{
    [TestFixture]
    public class BigDecimalOperationsSubTests
    {
        static Random rnd = new Random((int)DateTime.Now.Ticks);
        static void DoTesting(string left, string right, string result)
        {
            BigDecimal A = new BigDecimal(left);
            BigDecimal B = new BigDecimal(right);

            BigDecimal C = A - B;
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
            decimal C = A - B;

            DoTesting(DecimalToString(A),
                    DecimalToString(B),
                    DecimalToString(C));
        }

        [Test]
        public void Zero_Zero()
        {
            DoTesting("0", "0", "0");
        }

        [Test]
        public void Zero_m5()
        {
            DoTesting("0", "-5", "5");
        }

        [Test]
        public void m5_zero()
        {
            DoTesting("-5", "0", "-5");
        }

        [Test]
        public void m5_p6()
        {
            DoTesting("-5", "6", "-11");
        }

        [Test]
        public void p123D45_p45D678()
        {
            DoTesting("123.45", "45.678", "77.772");
        }

        [Test]
        public void m3D1_p10D005()
        {
            DoTesting("-3.1", "10.005", "-13.105");
        }

        [Test]
        public void m10D005_m3D1()
        {
            DoTesting("-10.005", "-3.1", "-6.905");
        }

        [Test]
        public void random_10000_tests()
        {
            for (int i = 0; i < 10000; i++)
                RandomTest();
        }
    }
}
