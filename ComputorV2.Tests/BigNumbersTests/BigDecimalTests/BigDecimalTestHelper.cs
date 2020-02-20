using System;
using System.Collections.Generic;
using System.Text;
using BigNumbers;
using NUnit.Framework;

namespace ComputorV2.UnitTests.BigNumbersTests.BigDecimalTests
{
    public enum Operation
    {
        Add, Sub, Mul, Div, Mod
    }

    static class BigDecimalTestHelper
    {
        private static readonly Random rand = new Random((int) DateTime.Now.Ticks);
        private static int _halfMaxInt = int.MaxValue / 2;

        public static void DoTesting(string left, string right, string expectedResult, Operation operation)
        {
            try
            {
                var a = new BigDecimal(left);
                var b = new BigDecimal(right);
                BigDecimal c = _bnFunctions[operation](a, b);
                try
                {
                    Assert.AreEqual(expectedResult, c.ToString());
                }
                catch (AssertionException)
                {
                    throw new Exception(
                        $"A = {a}, B = {b}.\n Expected result is {expectedResult}\n Actual result is{c}");
                }
            }
            catch (Exception)
            {
                throw new Exception(
                    $"Unexpected exception for numbers A = {left}, B = {right}");
            }
        }

        public static string DecimalToString(decimal number)
        {
            string str = number.ToString();
            if (!str.Contains(",") && !str.Contains("."))
                return str;
            str = str.Replace(",", ".");
            StringBuilder sb = new StringBuilder(str);
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

            decimal c = _decFunctions[operation](a, b);

            DoTesting(DecimalToString(a),
                DecimalToString(b),
                DecimalToString(c),
                operation);
        }

        private static int GenerateRandomInt()
        {
            return rand.Next(0, int.MaxValue) - _halfMaxInt;
        }

        #region Operations Delegates

        private static readonly Func<BigDecimal, BigDecimal, BigDecimal> _addBnFunction = (x, y) => x + y;
        private static readonly Func<decimal, decimal, decimal> _addDecFunction = (x, y) => x + y;

        private static readonly Func<BigDecimal, BigDecimal, BigDecimal> _subBnFunction = (x, y) => x - y;
        private static readonly Func<decimal, decimal, decimal> _subDecFunction = (x, y) => x - y;

        private static readonly Func<BigDecimal, BigDecimal, BigDecimal> _mulBnFunction = (x, y) => x * y;
        private static readonly Func<decimal, decimal, decimal> _mulDecFunction = (x, y) => x * y;

        private static readonly Func<BigDecimal, BigDecimal, BigDecimal> _divBnFunction = (x, y) => x / y;
        private static readonly Func<decimal, decimal, decimal> _divDecFunction = (x, y) => x / y;

        private static readonly Func<BigDecimal, BigDecimal, BigDecimal> _modBnFunction = (x, y) => x % y;
        private static readonly Func<decimal, decimal, decimal> _modDecFunction = (x, y) => x % y;

        private static Dictionary<Operation, Func<BigDecimal, BigDecimal, BigDecimal>> _bnFunctions =
            new Dictionary<Operation, Func<BigDecimal, BigDecimal, BigDecimal>>
        {
            {Operation.Add, _addBnFunction },
            {Operation.Sub, _subBnFunction },
            {Operation.Mul, _mulBnFunction },
            {Operation.Div, _divBnFunction },
            {Operation.Mod, _modBnFunction }
        };

        private static Dictionary<Operation, Func<decimal, decimal, decimal>> _decFunctions =
            new Dictionary<Operation, Func<decimal, decimal, decimal>>
        {
            {Operation.Add, _addDecFunction },
            {Operation.Sub, _subDecFunction },
            {Operation.Mul, _mulDecFunction },
            {Operation.Div, _divDecFunction },
            {Operation.Mod, _modDecFunction }
        };

        #endregion Operations Delegates

    }
}