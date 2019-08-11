using NUnit.Framework;
using ComputorV2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    class ExpressionTests
    {
        [SetUp]
        public void Setup()
        {
            ConsoleReader.AddManualVariable("somevar", new Expression("7"));
            ConsoleReader.AddManualVariable("someothervar", new Expression("2 + 3"));
        }

        [Test]
        public void Tokenize_EmptyString()
        {
            Assert.That(() => Expression.Tokenize(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Cannot tokenize null string"));
        }

        [Test]
        public void Tokenize_InvalidTokens()
        {
            Assert.That(() => Expression.Tokenize("abc(&)"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("The expression is invalid"));
        }

        [Test]
        public void Tokenize_ValidTokens()
        {
            string str = "-*( abc+ - * /   \t)";
            string[] expected = { "-", "*", "(", "abc", "+", "-", "*", "/", ")" };
            var actual = Expression.Tokenize(str).ToArray();
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void RecognizeLexems_SimpleTokens()
        {
            var expr = "abs ( - 2 ) + 3.5 * ( 1 )";
            var stringTokens = new List<string>(expr.Split(" "));
            var actual = Expression
                .RecognizeLexems(stringTokens)
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
            Assert.That(() => Expression
                .RecognizeLexems(stringTokens),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Invalid token: 'someunrealvar'"));
        }
        [Test]
        public void RecognizeLexems_ValidTokensWithVars()
        {
            var varList = new List<string> { "somevar", "someothervar" };
            var expr = "( 2 + somevar ) * someOtherVar ";
            var stringTokens = new List<string>(expr.Split(" "));
            var actual = Expression
                .RecognizeLexems(stringTokens)
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
                TokenType.BinOp,
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
            var rawActual = Expression
                .RecognizeLexems(stringTokens, funcParam);
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
    }
}
