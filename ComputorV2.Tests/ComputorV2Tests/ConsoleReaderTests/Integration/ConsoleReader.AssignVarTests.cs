using NUnit.Framework;
using ComputorV2;
using System;
using System.Collections.Generic;

namespace ComputorV2.Tests
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
        public void SimpleTest()
        {
            var cr = new ConsoleReader(
                 new Dictionary<string, Expression>
                 {
                    {"somevar", new Expression(
                        new List<RPNToken> {
                            new RPNToken{str = "7", tokenType = TokenType.DecimalNumber}
                        },
                        false) },
                     {"someothervar", new Expression(
                        new List<RPNToken> {
                            new RPNToken{str = "5", tokenType = TokenType.DecimalNumber}
                        },
                        false) }
                 }
                 );
            cr.ExecuteAssignVarCommand(null);
        }
        [Test]
        public void SimpleTest2()
        {
            var cr = new ConsoleReader(
                 new Dictionary<string, Expression>
                 {
                    {"somevar", new Expression(
                        new List<RPNToken> {
                            new RPNToken{str = "7", tokenType = TokenType.DecimalNumber}
                        },
                        false) },
                     {"someothervar", new Expression(
                        new List<RPNToken> {
                            new RPNToken{str = "5", tokenType = TokenType.DecimalNumber}
                        },
                        false) }
                 }
                 );
            cr.ExecuteAssignVarCommand(null);
        }
    }
}