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
        public void AssignVar_Null_NothingHappens()
        {
            new ConsoleReader().ExecuteAssignVarCommand(null);
        }
        [Test]
        public void AssignVar_EmptyString_NothingHappens()
        {
            new ConsoleReader().ExecuteAssignVarCommand("");
        }


        [Test]
        public void AssignVar_BigDecimal_VarAssigned()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 999999999999999999999.991");

            var expected = "999999999999999999999.991";
            var actual = cr["VaRa"];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AssignVar_BigDecimalWithZeroes_SimplifyExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 00000000000000000000.0000000000000000000001");

            var expected = "0.0000000000000000000001";
            var actual = cr["VarA"];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AssignVar_SumWithExistingVar_ValidResultExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 0.0000000000000000000001");
            cr.ExecuteAssignVarCommand("VarB = vARa + 7");

            var expected = "7.0000000000000000000001";
            var actual = cr["VarB"];

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AssignVar_DivideByZero_NullExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 7.777777");
            cr.ExecuteAssignVarCommand("VarB = vARa - 7.777777");
            cr.ExecuteAssignVarCommand("VarC = 20 /      varb");

            var actual = cr["VarC"];

            Assert.IsNull(actual);
        }

        [Test]
        public void AssignVar_ManyBracketTests_AllOkExpected()
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
        public void AssignVar_ManySimpleTests_AllOkExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarA = 2 * -2");
            cr.ExecuteAssignVarCommand("VarB = 2 + 2 * 2");
            cr.ExecuteAssignVarCommand("VarC = (2 + 2) * 2");
            cr.ExecuteAssignVarCommand("VarD = (-2 + 2) * 2");
            cr.ExecuteAssignVarCommand("VarE = 8 * 3 % 5");
            cr.ExecuteAssignVarCommand("VarF = (2 / 3) * 7 + ((5 - 7) * (21 % 8))");
            cr.ExecuteAssignVarCommand("VarG = 2 - 3");

            Assert.AreEqual("-1", cr["VarG"]);
            Assert.AreEqual("-4", cr["VarA"]);
            Assert.AreEqual("6", cr["VarB"]);
            Assert.AreEqual("8", cr["VarC"]);
            Assert.AreEqual("0", cr["VarD"]);
            Assert.AreEqual("4", cr["VarE"]);
            Assert.AreEqual("-5.33333333333333333338", cr["VarF"]);
        }
        [Test]
        public void AssignVar_P2_p_m_M3__FourExpected()
        {
            var cr = new ConsoleReader();
            cr.ExecuteAssignVarCommand("VarG = 2 + --3");

            Assert.AreEqual("5", cr["VarG"]);
        }
    }
}