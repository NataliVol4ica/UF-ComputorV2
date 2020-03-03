using System;
using System.Collections.Generic;
using System.Text;
using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    public enum Operation
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod
    }

    internal static class BigDecimalOperatorTestHelper
    {
        private static readonly Random Rand = new Random((int) DateTime.Now.Ticks);
        private static readonly int _halfMaxInt = int.MaxValue / 2;

        public static void DoTesting(string left, string right, string expectedResult, Operation operation)
        {
            var a = new BigDecimal(left);
            var b = new BigDecimal(right);
            var c = BnFunctions[operation](a, b);
            try
            {
                var actual = c.ToString();
                if (operation == Operation.Div && expectedResult.Length != actual.Length)
                {
                    expectedResult = expectedResult.Substring(0, actual.Length);
                }

                Assert.AreEqual(expectedResult, c.ToString());
            }
            catch (AssertionException)
            {
                throw new Exception(
                    $"A = {a}, B = {b}.\n Expected result is {expectedResult}\n Actual result is{c}");
            }
        }

        public static string DecimalToString(decimal number)
        {
            var str = number.ToString();
            if (!str.Contains(",") && !str.Contains("."))
                return str;
            str = str.Replace(",", ".");
            var sb = new StringBuilder(str);
            while (sb[sb.Length - 1] == '0')
                sb.Remove(sb.Length - 1, 1);
            if (sb[sb.Length - 1] == '.')
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static void ExecuteRandomTest(Operation operation)
        {
            decimal a = GenerateRandomInt() / 10000;
            decimal b = GenerateRandomInt() / 100000;

            if ((operation == Operation.Div || operation == Operation.Mod) && b == 0)
                return;

            var c = DecFunctions[operation](a, b);

            DoTesting(DecimalToString(a),
                DecimalToString(b),
                DecimalToString(c),
                operation);
        }

        private static int GenerateRandomInt()
        {
            return Rand.Next(0, int.MaxValue) - _halfMaxInt;
        }

        #region Operations Delegates

        private static readonly Dictionary<Operation, Func<BigDecimal, BigDecimal, BigDecimal>> BnFunctions =
            new Dictionary<Operation, Func<BigDecimal, BigDecimal, BigDecimal>>
            {
                {Operation.Add, (x, y) => x + y},
                {Operation.Sub, (x, y) => x - y},
                {Operation.Mul, (x, y) => x * y},
                {Operation.Div, (x, y) => x / y},
                {Operation.Mod, (x, y) => x % y}
            };

        private static readonly Dictionary<Operation, Func<decimal, decimal, decimal>> DecFunctions =
            new Dictionary<Operation, Func<decimal, decimal, decimal>>
            {
                {Operation.Add, (x, y) => x + y},
                {Operation.Sub, (x, y) => x - y},
                {Operation.Mul, (x, y) => x * y},
                {Operation.Div, (x, y) => x / y},
                {Operation.Mod, (x, y) => x % y}
            };

        #endregion Operations Delegates
    }
}