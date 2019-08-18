using NUnit.Framework;
using ComputorV2;
using System;
using System.Collections.Generic;
using System.Linq;
using BigNumbers;

namespace ComputorV2Tests.ExpressionProcessorTests
{
    class ExpressionProcessorShuttingYardTests
    {
        [Test]
        public void ShuttingYard_NullList_ExpectNull()
        {
            var actual = ExpressionProcessor.ShuttingYardAlgorithm(null);
            Assert.IsNull(actual);
        }

        [Test]
        public void ShuttingYard_EmptyList_ExpectNull()
        {
            var actual = ExpressionProcessor.ShuttingYardAlgorithm(new List<RPNToken>());
            Assert.IsNull(actual);
        }

        [Test]
        public void ShuttingYard_SingleTokenWithNumber_ExpectNumber()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(new List<RPNToken> {
                NewDec("1")
            });
            var actual = String.Join(" ", actualQueue);
            var expected = "1";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShuttingYard_SingleTokenWithBracket_ExpectException()
        {
            var tokenList = new List<RPNToken> {
                NewOpBr()
            };
            Assert.That(() => ExpressionProcessor.ShuttingYardAlgorithm(tokenList),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Unexpected token '('"));
        }

        [Test]
        public void ShuttingYard_SingleTokenWithTwoNumbers_ExpectException()
        {
            var tokenList = new List<RPNToken> {
                NewDec("5"), NewDec("7")
            };
            Assert.That(() => ExpressionProcessor.ShuttingYardAlgorithm(tokenList),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Cannot calculate this expression. Remaining RPN buffer contains extra numbers."));
        }
        [Test]
        public void ShuttingYard_MinusZero_ExpectOk()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(
                new List<RPNToken> {
                    NewUnOp("-"), NewDec("0")
                }
            );
            var actual = String.Join(" ", actualQueue);
            var expected = "0 -";
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShuttingYard_MinusFive_ExpectOk()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(
                new List<RPNToken> {
                    NewUnOp("-"), NewDec("5")
                }
            );
            var actual = String.Join(" ", actualQueue);
            var expected = "5 -";
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShuttingYard_P2_p_P2_ExpectOk()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(
                new List<RPNToken> {
                    NewDec("2"), NewUnOp("+"), NewDec("3")
                }
            );
            var actual = String.Join(" ", actualQueue);
            var expected = "2 3 +";
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShuttingYard_M2_p_M2_ExpectOk()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(
                  new List<RPNToken> {
                   NewUnOp("-"),  NewDec("2"), NewUnOp("-"), NewDec("3")
                  }
              );
            var actual = String.Join(" ", actualQueue);
            var expected = "2 - 3 - +";
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShuttingYard_P2_m_M2_ExpectOk()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(
                  new List<RPNToken> {
                  NewDec("2"), NewUnOp("+"), NewUnOp("-"), NewUnOp("-"), NewDec("2")
                  }
              );
            var actual = String.Join(" ", actualQueue);
            var expected = "2 2 - - +";
            Assert.AreEqual(expected, actual);
        }
        [Test]
        public void ShuttingYard_Brackets_ExpectOk()
        {
            var actualQueue = ExpressionProcessor.ShuttingYardAlgorithm(
                   new List<RPNToken> {
                   NewOpBr(),  NewDec("1"), NewBinOp("+"), NewDec("2"), NewClBr(), NewBinOp("/"), NewDec("3")
                   }
               );
            var actual = String.Join(" ", actualQueue);
            var expected = "1 2 + 3 /";
            Assert.AreEqual(expected, actual);
        }



        public RPNToken NewOpBr()
        {
            return new RPNToken { str = "(", tokenType = TokenType.OBracket };
        }
        public RPNToken NewClBr()
        {
            return new RPNToken { str = ")", tokenType = TokenType.CBracket };
        }

        public RPNToken NewBinOp(string str)
        {
            return new RPNToken { str = str, tokenType = TokenType.BinOp };
        }
        public RPNToken NewUnOp(string str)
        {
            return new RPNToken { str = str, tokenType = TokenType.UnOp };
        }
        public RPNToken NewDec(string str)
        {
            return new RPNToken { str = str, tokenType = TokenType.DecimalNumber };
        }
        public RPNToken NewFunc(string str)
        {
            return new RPNToken { str = str, tokenType = TokenType.Function };
        }
    }
}
