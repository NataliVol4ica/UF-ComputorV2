using System.Collections.Generic;
using ComputorV2;
using Moq;
using NUnit.Framework;

namespace ComputorV2Tests.ExpressionProcessorTests
{
    internal class ExpressionProcessorTests
    {
        private readonly Mock<IVariableStorage> _variableStorage = new Mock<IVariableStorage>();
        private ExpressionProcessor _processor;
        private List<string> _varNames;

        [SetUp]
        public void Setup()
        {
            _varNames = new List<string> {"vara", "varb", "varc", "vard"};
            _variableStorage.Setup(vs => vs.AllVariablesNames).Returns(_varNames);
            _processor = new ExpressionProcessor(_variableStorage.Object);
        }

        [Test]
        [TestCase("-0", "0")]
        [TestCase("-5", "-5")]
        [TestCase("2 + 2", "4")]
        [TestCase("-2 + - 2", "-4")]
        [TestCase("-2 * -2", "4")]
        [TestCase("(2)", "2")]
        [TestCase("((((2))))", "2")]
        [TestCase("((-((2))))", "-2")]
        [TestCase("-(-(-((2))))", "-2")]
        [TestCase("(((((2) + (2)))))", "4")]
        [TestCase("2+2*2", "6")]
        [TestCase("(2+2)*2", "8")]
        [TestCase("(-2+2)*2", "0")]
        [TestCase("8*3%5", "4")]
        [TestCase("2+--2", "4")]
        [TestCase("(6 / 3) * 7 + ((5 - 7) * (21 % (12/6)))", "12")]
        [TestCase("2 ^ 0", "1")]
        [TestCase("2 ^ -1", "0.5")]
        [TestCase("2 ^ 2", "4")]
        [TestCase("2 ^ 3", "8")]
        [TestCase("2 ^ 3 ^ 2", "512")]
        public void CreateExpression_Decimal_WhenCalled_CreatesExpression(string str, string expected)
        {
            var actual = _processor.CreateExpression(str).ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("i ^ 2", "-1")]
        [TestCase("i + 2", "2+i")]
        [TestCase("i + 2*3", "6+i")]
        [TestCase("i + i + 5*6", "30+2i")]
        public void CreateExpression_Complex_WhenCalled_CreatesExpression(string str, string expected)
        {
            var actual = _processor.CreateExpression(str).ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}