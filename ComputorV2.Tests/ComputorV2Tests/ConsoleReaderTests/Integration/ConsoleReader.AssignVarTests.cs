using ComputorV2;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;

namespace ComputorV2Tests.ConsoleReaderTests.Integration
{
    public class ConsoleReaderIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ExecuteAssignVarCommand_Null_NothingHappens()
        {
            new ConsoleReader().ExecuteAssignVarCommand(null);
        }
        [Test]
        public void ExecuteAssignVarCommand_EmptyString_NothingHappens()
        {
            new ConsoleReader().ExecuteAssignVarCommand("");
        }


        [Test]
        public void ExecuteAssignVarCommand_BigDecimal_VarAssigned()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 999999999999999999999.991");

            var expected = "999999999999999999999.991";
            var actual = cr["VaRa"];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExecuteAssignVarCommand_BigDecimalWithZeroes_SimplifyExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 00000000000000000000.0000000000000000000001");

            var expected = "0.0000000000000000000001";
            var actual = cr["VarA"];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExecuteAssignVarCommand_SumWithExistingVar_ValidResultExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 0.0000000000000000000001");
            cr.ExecuteAssignVarCommand("VarB = vARa + 7");

            var expected = "7.0000000000000000000001";
            var actual = cr["VarB"];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ExecuteAssignVarCommand_DivideByZero_NullExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 7.777777");
            cr.ExecuteAssignVarCommand("VarB = vARa - 7.777777");
            cr.ExecuteAssignVarCommand("VarC = 20 /      varb");

            var actual = cr["VarC"];

            Assert.IsNull(actual);
        }

        [Test]
        public void ExecuteAssignVarCommand_ManyBracketTests_AllOkExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = (2)");
            cr.ExecuteAssignVarCommand("VarB = ((((2))))");
            cr.ExecuteAssignVarCommand("VarC = ((-((2))))");
            cr.ExecuteAssignVarCommand("VarD = -(-(-((2))))");
            cr.ExecuteAssignVarCommand("VarE = (((((2) + (2)))))");

            Assert.AreEqual("2", cr["VarA"]);
            Assert.AreEqual("2", cr["VarB"]);
            Assert.AreEqual("-2", cr["VarC"]);
            Assert.AreEqual("-2", cr["VarD"]);
            Assert.AreEqual("4", cr["VarE"]);
        }
        [Test]
        [TestCase("2 * -2", "-4")]
        [TestCase("2 + 2 * 2", "6")]
        [TestCase("(2 + 2) * 2", "8")]
        [TestCase("(-2 + 2) * 2", "0")]
        [TestCase("8 * 3 % 5", "4")]
        [TestCase("(2 / 3) * 7 + ((5 - 7) * (21 % 8))", "-5.33333333333333333338")]
        [TestCase("2 - 3", "-1")]
        [TestCase("-2 - 3", "-5")]
        public void ExecuteAssignVarCommand_WhenCalled_SimplifyExpected(string command, string expected)
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand($"VarA = {command}");
            Assert.That(cr["VarA"], Is.EqualTo(expected));
        }     
    }
}