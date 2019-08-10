using NUnit.Framework;
using ComputorV2;
using System;

namespace Tests
{
    public class ConsoleReaderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsValidVarName_valid()
        {
            var expected = true;

            var actual = ConsoleReader.IsValidVarName("lalala");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName("varA");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName("  \t \r varA ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValidVarName_invalid()
        {
            var expected = false;

            var actual = ConsoleReader.IsValidVarName("");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName(" lalala1 ");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName("name name");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName(" 100500");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName("i");
            Assert.AreEqual(expected, actual);

            actual = ConsoleReader.IsValidVarName("   \t    \r I  ");
            Assert.AreEqual(expected, actual);
        }
    }
}