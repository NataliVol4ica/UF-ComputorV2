using ComputorV2;
using NUnit.Framework;
using System.Collections.Generic;

namespace ComputorV2Tests.ConsoleReaderTests.Unit
{
    public class VariableStorageTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("lalala", true)]
        [TestCase("varA",true)]
        [TestCase("  \t \r varA ",true)]
        [TestCase("", false)]
        [TestCase(" lalala1 ", false)]
        [TestCase("name name", false)]
        [TestCase(" 100500", false)]
        [TestCase("i", false)]
        [TestCase("I", false)]
        public void IsValidVarName_WhenCalled_ReturnsResult(string varname, bool expected)
        {
            var actual = VariableStorage.IsValidVarName(varname);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        [TestCase("lalala", "", "", false)]
        [TestCase("varA", "", "", false)]
        [TestCase("varA()", "", "", false)]
        [TestCase("varA(()", "", "", false)]
        [TestCase(" f(c) ","f", "c", true)]
        [TestCase(" f(C) ","f", "C", true)]
        [TestCase("f(existingVar)", "", "", false)]
        [TestCase(" f(i)", "", "", false)]
        [TestCase("f(a1)", "", "", false)]
        [TestCase("functionName(parameterName)", "functionName", "parameterName", true)]
        public void IsValidFunctionDeclaration_WhenCalled_ReturnsResult(
            string funcStr, string expectedFuncName, string expectedParamName, bool expectedResult)
        {
            var vs = new VariableStorage();
            vs.AddOrUpdateVariableValue("existingvar", new Expression(new List<RPNToken>(), false));
            var actual =vs
                .IsValidFunctionDeclaration(funcStr, out string fName, out string pName, out string reason);
            Assert.AreEqual(expectedResult, actual);
            Assert.AreEqual(expectedFuncName, fName);
            Assert.AreEqual(expectedParamName, pName);
        }
    }
}