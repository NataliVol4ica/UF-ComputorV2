using NUnit.Framework;
using ComputorV2;
using System;

namespace ComputorV2.Tests
{
    public class ConsoleReaderToolsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetValidCommandsType()
        {
            var actual = ConsoleReaderTools.GetCommandType("   exit ");
            var expected = CommandType.Exit;
            Assert.AreEqual(expected, actual);

            actual = ConsoleReaderTools.GetCommandType("help");
            expected = CommandType.ShowHelp;
            Assert.AreEqual(expected, actual);

            actual = ConsoleReaderTools.GetCommandType("   vars     \t");
            expected = CommandType.ShowVars;
            Assert.AreEqual(expected, actual);


            actual = ConsoleReaderTools.GetCommandType("   VaRs     \t");
            expected = CommandType.ShowVars;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetInvalidCommandType_WSTrashText_NoEqual()
        {
            ExpectGetCommandTypeException("  trash text ");
            ExpectGetCommandTypeException("exitt");
            ExpectGetCommandTypeException("var");
            ExpectGetCommandTypeException("");
        }

        [Test]
        public void GetInvalidCommandType_WSTrashText_WithEquals()
        {
            ExpectGetCommandTypeEqualityException("==", 2);
            ExpectGetCommandTypeEqualityException("a = 2 =3", 2);
            ExpectGetCommandTypeEqualityException("a = 2 + 3 * 5 ; a = b; a = c;", 3);
        }

        [Test]
        public void GetValidComplexCommandType()
        { 
            var actual = ConsoleReaderTools.GetCommandType(" a    +\t\t b = ?    \t");
            var expected = CommandType.EvaluateExpression;
            Assert.AreEqual(expected, actual);

            actual = ConsoleReaderTools.GetCommandType("3= ?");
            expected = CommandType.EvaluateExpression;
            Assert.AreEqual(expected, actual);

            actual = ConsoleReaderTools.GetCommandType("= ?\r");
            expected = CommandType.EvaluateExpression;
            Assert.AreEqual(expected, actual);

            actual = ConsoleReaderTools.GetCommandType("a = 2");
            expected = CommandType.AssignVar;
            Assert.AreEqual(expected, actual);

            actual = ConsoleReaderTools.GetCommandType("b = \t 2 + 2 * 2");
            expected = CommandType.AssignVar;
            Assert.AreEqual(expected, actual);
        }

        void ExpectGetCommandTypeException(string command)
        {
            var cmdTrim = command.Trim();
            Assert.That(() => ConsoleReaderTools.GetCommandType(command),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo($"Unknown command: '{cmdTrim}'"));
        }
        void ExpectGetCommandTypeEqualityException(string command, int equalities)
        {
            var cmdTrim = command.Trim();
            Assert.That(() => ConsoleReaderTools.GetCommandType(command),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo($"Command cannot contain: '{equalities}' equal signs"));
        }
    }
}