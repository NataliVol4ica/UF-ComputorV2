using System;
using BigNumbers;
using NUnit.Framework;

namespace ComputorV2.UnitTests.BigNumbersTests.BigDecimalTests
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
        [TestCase(Operation.Add)]
        [TestCase(Operation.Sub)]
        [TestCase(Operation.Mul)]
        [TestCase(Operation.Div)]
        [TestCase(Operation.Mod)]
        public void Operators_10000RandomTests(Operation operation)
        {
            for (int i = 0; i < 10000; i++)
                BigDecimalTestHelper.ExecuteRandomTest(operation);
        }
    }
}