using ComputorV2;
using NUnit.Framework;
using System;

namespace ComputorV2Tests.ConsoleReaderTests.Unit
{
    public class ConsoleReaderUnitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void IsValidVarName_valid()
        {
            var actual = ConsoleReader.IsValidVarName("lalala");
            Assert.IsTrue(actual);

            actual = ConsoleReader.IsValidVarName("varA");
            Assert.IsTrue(actual);

            actual = ConsoleReader.IsValidVarName("  \t \r varA ");
            Assert.IsTrue(actual);
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