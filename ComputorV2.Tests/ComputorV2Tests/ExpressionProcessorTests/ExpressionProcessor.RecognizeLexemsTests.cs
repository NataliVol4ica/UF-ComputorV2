using NUnit.Framework;
using ComputorV2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputorV2Tests.ExpressionProcessorTests
{
    class ExpressionProcessorRecognizeLexemsTests
    {
        [Test]
        public void RecognizeLexems_SimpleTokens()
        {
            
            var expr = "abs ( - 2 ) + 3.5 * ( 1 )";
            var stringTokens = new List<string>(expr.Split(" "));
            var actual = ExpressionProcessor
                .RecognizeLexems(stringTokens, new ConsoleReader())
                .Select(t => t.tokenType)
                .ToList();
            var expected = new List<TokenType> {
                TokenType.Function,
                TokenType.OBracket,
                TokenType.UnOp,
                TokenType.DecimalNumber,
                TokenType.CBracket,
                TokenType.BinOp,
                TokenType.DecimalNumber,
                TokenType.BinOp,
                TokenType.OBracket,
                TokenType.DecimalNumber,
                TokenType.CBracket
            };
            Assert.AreEqual(actual.Count, expected.Count);
            for (int i = 0; i < expected.Count; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void RecognizeLexems_InvalidTokens()
        {
            var emptyList = new List<string>();
            var expr = "2 + someunrealvar ";
            var stringTokens = new List<string>(expr.Split(" "));
            Assert.That(() => ExpressionProcessor
                .RecognizeLexems(stringTokens, new ConsoleReader()),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Invalid token: 'someunrealvar'"));
        }
        [Test]
        public void RecognizeLexems_ValidTokensWithVars()
        {
            var varList = new List<string> { "somevar", "someothervar" };
            var expr = "( 2 + somevar ) * someOtherVar ";
            var stringTokens = new List<string>(expr.Split(" "));
            var actual = ExpressionProcessor
                .RecognizeLexems(stringTokens, GenerateCRWithSomeVars())
                .Select(t => t.tokenType)
                .ToList();
            var expected = new List<TokenType> {
                TokenType.OBracket,
                TokenType.DecimalNumber,
                TokenType.BinOp,
                TokenType.OBracket,
                TokenType.DecimalNumber,
                TokenType.CBracket,
                TokenType.CBracket,
                TokenType.BinOp,
                TokenType.OBracket,
                TokenType.DecimalNumber,
                TokenType.CBracket
            };
            Assert.AreEqual(actual.Count, expected.Count);
            for (int i = 0; i < expected.Count; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void RecognizeLexems_ValidTokensWithVarsAndFuncParam()
        {
            var funcParam = "y";
            var varList = new List<string> { "somevar", "someothervar" };
            var expr = $"someVar * {funcParam} + 2 ";
            var stringTokens = new List<string>(expr.Split(" "));
            var rawActual = ExpressionProcessor
                .RecognizeLexems(stringTokens, GenerateCRWithSomeVars(), funcParam);
            var actual = rawActual
                .Select(t => t.tokenType)
                .ToList();
            var expected = new List<TokenType> {
                TokenType.OBracket,
                TokenType.DecimalNumber,
                TokenType.CBracket,
                TokenType.BinOp,
                TokenType.FunctionParameter,
                TokenType.BinOp,
                TokenType.DecimalNumber
            };
            Assert.AreEqual(actual.Count, expected.Count);
            for (int i = 0; i < expected.Count; i++)
                Assert.AreEqual(expected[i], actual[i]);
            Assert.AreEqual("x", rawActual[4].str);
        }

        private ConsoleReader GenerateCRWithSomeVars()
        {
            return new ConsoleReader(
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
        }
    }
}
