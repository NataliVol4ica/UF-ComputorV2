﻿using NUnit.Framework;
using ComputorV2;
using System.Collections.Generic;
using Moq;

namespace ComputorV2Tests.ExpressionProcessorTests
{

    class ExpressionProcessorTests
    {
        private readonly Mock<IVariableStorage> _variableStorage = new Mock<IVariableStorage>();
        private List<string> _varNames;
        private ExpressionProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _varNames = new List<string> { "vara", "varb", "varc", "vard"};
            _variableStorage.Setup(vs => vs.VariablesNames).Returns(_varNames);
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
        public void CreateExpression_WhenCalled_CreatesExpression(string str, string expected)
        {
            var actual = _processor.CreateExpression(str).ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}