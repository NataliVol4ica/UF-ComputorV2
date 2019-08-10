using NUnit.Framework;
using ComputorV2;
using System;

namespace Tests
{
    public class ReaderCommandToolsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetValidCommandsType()
        {
            var actual = ReaderCommandTools.GetCommandType("   exit ");
            var expected = CommandType.Exit;
            Assert.AreEqual(expected, actual);

            actual = ReaderCommandTools.GetCommandType("help");
            expected = CommandType.ShowHelp;
            Assert.AreEqual(expected, actual);

            actual = ReaderCommandTools.GetCommandType("   vars     \t");
            expected = CommandType.ShowVars;
            Assert.AreEqual(expected, actual);


            actual = ReaderCommandTools.GetCommandType("   VaRs     \t");
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
            var actual = ReaderCommandTools.GetCommandType(" a    +\t\t b = ?    \t");
            var expected = CommandType.EvaluateExpression;
            Assert.AreEqual(expected, actual);

            actual = ReaderCommandTools.GetCommandType("3= ?");
            expected = CommandType.EvaluateExpression;
            Assert.AreEqual(expected, actual);

            actual = ReaderCommandTools.GetCommandType("= ?\r");
            expected = CommandType.EvaluateExpression;
            Assert.AreEqual(expected, actual);

            actual = ReaderCommandTools.GetCommandType("a = 2");
            expected = CommandType.AssignVar;
            Assert.AreEqual(expected, actual);

            actual = ReaderCommandTools.GetCommandType("b = \t 2 + 2 * 2");
            expected = CommandType.AssignVar;
            Assert.AreEqual(expected, actual);
        }

        void ExpectGetCommandTypeException(string command)
        {
            var cmdTrim = command.Trim();
            Assert.That(() => ReaderCommandTools.GetCommandType(command),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo($"Unknown command: '{cmdTrim}'"));
        }
        void ExpectGetCommandTypeEqualityException(string command, int equalities)
        {
            var cmdTrim = command.Trim();
            Assert.That(() => ReaderCommandTools.GetCommandType(command),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo($"Command cannot contain: '{equalities}' equal signs"));
        }
    }
}