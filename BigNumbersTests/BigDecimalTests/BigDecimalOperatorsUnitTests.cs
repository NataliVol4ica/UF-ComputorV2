using System;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    [TestFixture]
    public class BigDecimalOperatorsUnitTests
    {
        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("0", "-5", "-5")]
        [TestCase("-5", "0", "-5")]
        [TestCase("-5", "6", "1")]
        [TestCase("123.45", "45.678", "169.128")]
        [TestCase("-3.1", "10.005", "6.905")]
        [TestCase("-10.005", "-3.1", "-13.105")]
        public void AddOperator_WhenCalled_ShouldReturnSumOfTwoNumbers(string op1, string op2, string expected)
        {
            BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Add);
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("0", "-5", "5")]
        [TestCase("-5", "0", "-5")]
        [TestCase("-5", "6", "-11")]
        [TestCase("123.45", "45.678", "77.772")]
        [TestCase("-3.1", "10.005", "-13.105")]
        [TestCase("-10.005", "-3.1", "-6.905")]
        public void SubOperator_WhenCalled_ShouldReturnSubOfTwoNumbers(string op1, string op2, string expected)
        {
            BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Sub);
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("0", "-5", "0")]
        [TestCase("-5", "0", "0")]
        [TestCase("-5", "6", "-30")]
        [TestCase("123.45", "45.678", "5638.9491")]
        [TestCase("-3.1", "10.005", "-31.0155")]
        [TestCase("-10.005", "-3.1", "31.0155")]
        public void MulOperator_WhenCalled_ShouldReturnMulOfTwoNumbers(string op1, string op2, string expected)
        {
            BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Mul);
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
        public void DivOperator_WhenCalled_ShouldReturnDivOfTwoNumbers(string op1, string op2, string expected)
        {
            BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Div);
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("-5", "0", "0")]
        public void DivOperator_WhenDividingOnZero_ShouldThrowException(string op1, string op2, string expected)
        {
            Assert.Throws<DivideByZeroException>(() =>
                BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Div));
        }

        [Test]
        [TestCase("0", "-5", "0")]
        [TestCase("-6", "5", "-1")]
        [TestCase("20.1", "0.05", "0")]
        [TestCase("20.1", "5", "0.1")]
        [TestCase("6897.1312", "7785.3", "6897.1312")]
        public void ModOperator_WhenCalled_ShouldReturnModOfTwoNumbers(string op1, string op2, string expected)
        {
            BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Mod);
        }

        [Test]
        [TestCase("0", "0", "0")]
        [TestCase("-5", "0", "0")]
        public void ModOperator_WhenModdingOnZero_ShouldThrowException(string op1, string op2, string expected)
        {
            Assert.Throws<ArgumentException>(() =>
                BigDecimalTestHelper.DoTesting(op1, op2, expected, Operation.Mod));
        }

        [Test]
        [TestCase(Operation.Add)]
        [TestCase(Operation.Sub)]
        [TestCase(Operation.Mul)]
        [TestCase(Operation.Div)]
        [TestCase(Operation.Mod)]
        public void Operators_10000RandomTests(Operation operation)
        {
            for (var i = 0; i < 10000; i++)
                BigDecimalTestHelper.ExecuteRandomTest(operation);
        }
    }
}