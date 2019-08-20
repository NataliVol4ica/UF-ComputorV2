using NUnit.Framework;
using ComputorV2;
using System;
namespace ComputorV2Tests.ExpressionProcessorTests
{
    class ExpressionProcessorTokenizeTests
    {
        [Test]
        public void Tokenize_EmptyString()
        {
            Assert.That(() => ExpressionProcessor.Tokenize(null),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("Cannot tokenize null string"));
        }

        [Test]
        public void Tokenize_InvalidTokens()
        {
            Assert.That(() => ExpressionProcessor.Tokenize("abc(&)"),
                Throws.TypeOf<ArgumentException>()
                .With.Message.EqualTo("The expression is invalid"));
        }

        [Test]
        public void Tokenize_ValidTokens()
        {
            string str = "-*( abc+ - * /   \t)";
            string[] expected = { "-", "*", "(", "abc", "+", "-", "*", "/", ")" };
            var actual = ExpressionProcessor.Tokenize(str).ToArray();
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }        
    }
}
