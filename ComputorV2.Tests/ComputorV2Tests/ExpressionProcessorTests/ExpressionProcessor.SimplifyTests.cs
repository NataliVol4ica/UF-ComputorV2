using NUnit.Framework;
using ComputorV2;
using System;
using System.Collections.Generic;
using System.Linq;
using BigNumbers;

namespace ComputorV2Tests.ExpressionProcessorTests
{
    class ExpressionProcessorSimplifyTests
    {
        [Test]
        public void Simplify_NullList_ExpectNull()
        {
            var actual = ExpressionProcessor.Simplify(null);
            Assert.IsNull(actual);
        }

        [Test]
        public void Simplify_EmptyList_ExpectNull()
        {
            var actual = ExpressionProcessor.Simplify(new List<RPNToken>());
            Assert.IsNull(actual);
        }

        [Test]
        public void Simplify_SingleTokenWithNumber_ExpectNumber()
        {
            var actual = ExpressionProcessor.Simplify(new List<RPNToken> {
                new RPNToken{ str = "1", tokenType = TokenType.DecimalNumber}
            });
            var expected = new BigDecimal("1");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Simplify_SingleTokenWithBracket_ExpectException()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "(", tokenType = TokenType.OBracket}
            };
            Assert.That(() => ExpressionProcessor.Simplify(tokenList),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Unexpected token '('"));
        }

        [Test]
        public void Simplify_SingleTokenWithTwoNumbers_ExpectException()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "5", tokenType = TokenType.DecimalNumber},
                new RPNToken{ str = "7", tokenType = TokenType.DecimalNumber}
            };
            Assert.That(() => ExpressionProcessor.Simplify(tokenList),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Cannot calculate this expression. Remaining RPN buffer contains extra numbers."));
        }
        [Test]
        public void Simplify_MinusZero_ExpectZero()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "-", tokenType = TokenType.UnOp},
                new RPNToken{ str = "0", tokenType = TokenType.DecimalNumber}
            };
            var actual = ExpressionProcessor.Simplify(tokenList);
            var expected = new BigDecimal(0);
            Assert.AreEqual(expected, actual);

        }
        [Test]
        public void Simplify_MinusFive_ExpectMinusFive()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "-", tokenType = TokenType.UnOp},
                new RPNToken{ str = "5", tokenType = TokenType.DecimalNumber}
            };
            var actual = ExpressionProcessor.Simplify(tokenList);
            var expected = new BigDecimal(-5);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Simplify_P2_p_P2_Expect_4()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "2", tokenType = TokenType.DecimalNumber},
                new RPNToken{ str = "+", tokenType = TokenType.BinOp},
                new RPNToken{ str = "2", tokenType = TokenType.DecimalNumber}
            };
            var actual = ExpressionProcessor.Simplify(tokenList);
            var expected = new BigDecimal(4);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Simplify_M2_p_M2_Expect_M4()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "-", tokenType = TokenType.UnOp},
                new RPNToken{ str = "2", tokenType = TokenType.DecimalNumber},
                new RPNToken{ str = "+", tokenType = TokenType.BinOp},
                new RPNToken{ str = "-", tokenType = TokenType.UnOp},
                new RPNToken{ str = "2", tokenType = TokenType.DecimalNumber}
            };
            var actual = ExpressionProcessor.Simplify(tokenList);
            var expected = new BigDecimal(-4);
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void Simplify_Brackets_Expect_M2()
        {
            var tokenList = new List<RPNToken> {
                new RPNToken{ str = "(", tokenType = TokenType.OBracket},
                new RPNToken{ str = "(", tokenType = TokenType.OBracket},
                new RPNToken{ str = "-", tokenType = TokenType.UnOp},
                new RPNToken{ str = "(", tokenType = TokenType.OBracket},
                new RPNToken{ str = "2", tokenType = TokenType.DecimalNumber},
                new RPNToken{ str = ")", tokenType = TokenType.CBracket},
                new RPNToken{ str = ")", tokenType = TokenType.CBracket},
                new RPNToken{ str = ")", tokenType = TokenType.CBracket}
            };
            var actual = ExpressionProcessor.Simplify(tokenList);
            var expected = new BigDecimal(-2);
            Assert.AreEqual(expected, actual);
        }        
    }
}
