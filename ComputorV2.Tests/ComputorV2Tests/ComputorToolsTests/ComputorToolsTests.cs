using NUnit.Framework;
using ComputorV2;
using System;

namespace ComputorV2Tests
{
    public class ComputorToolsTests
    {
        [Test]
        [TestCase("  trash text ")]
        [TestCase("exitt")]
        [TestCase("var")]
        [TestCase("")]
        [TestCase(" \t\n")]
        [TestCase(null)]
        public void GetCommandType_WhenCalledWithInvalidCommand_ThrowsArgumentException(string command)
        {           
            Assert.That(() => ComputorTools.GetCommandType(command),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [TestCase("==", 2)]
        [TestCase("a = 2 =3", 2)]
        [TestCase("a = 2 + 3 * 5 ; a = b; a = c;", 3)]
        public void GetCommandType_WhenCalledWithCommandWithMultipleEqualities_ReturnsCommandType(string command, int equalities)
        {
            Assert.That(() => ComputorTools.GetCommandType(command),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo($"Command cannot contain: '{equalities}' equal signs"));
        }

        [Test]
        [TestCase("   exit ", CommandType.Exit)]
        [TestCase("   vars     \t", CommandType.ShowVars)]
        [TestCase("   VaRs     \t", CommandType.ShowVars)]
        [TestCase("help", CommandType.ShowHelp)]
        [TestCase("vara = 2", CommandType.AssignVar)]
        [TestCase("vara = ?", CommandType.EvaluateExpression)]
        [TestCase("detailed", CommandType.Detailed)]
        [TestCase("reset", CommandType.Reset)]
        [TestCase("allowed ", CommandType.ShowAlowedOperations)]
        [TestCase(" a    +\t\t b = ?    \t", CommandType.EvaluateExpression)]
        [TestCase("3= ?", CommandType.EvaluateExpression)]
        [TestCase("= ?\r", CommandType.EvaluateExpression)]
        [TestCase("a = 2", CommandType.AssignVar)]
        [TestCase("b = \t 2 + 2 * 2", CommandType.AssignVar)]
        public void GetCommandType_WhenCalledWithValidCommand_ReturnsCommandType(string cmd, CommandType cmdType)
        {
            var actual = ComputorTools.GetCommandType(cmd);
            var expected = cmdType;
            Assert.AreEqual(expected, actual);
        }
    }
}