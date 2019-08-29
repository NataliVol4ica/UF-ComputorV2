//using ComputorV2.ExternalConnections;
//using NUnit.Framework;
//using System;

//namespace ComputorV2Tests.ConsoleReaderTests.Unit
//{
//    public class ConsoleReaderUnitTests
//    {
//        [SetUp]
//        public void Setup()
//        {
//        }

//        [Test]
//        public void IsValidVarName_valid()
//        {
//            var actual = ConsoleProcessor.IsValidVarName("lalala");
//            Assert.IsTrue(actual);

//            actual = ConsoleProcessor.IsValidVarName("varA");
//            Assert.IsTrue(actual);

//            actual = ConsoleProcessor.IsValidVarName("  \t \r varA ");
//            Assert.IsTrue(actual);
//        }

//        [Test]
//        public void IsValidVarName_invalid()
//        {
//            var expected = false;

//            var actual = ConsoleProcessor.IsValidVarName("");
//            Assert.AreEqual(expected, actual);

//            actual = ConsoleProcessor.IsValidVarName(" lalala1 ");
//            Assert.AreEqual(expected, actual);

//            actual = ConsoleProcessor.IsValidVarName("name name");
//            Assert.AreEqual(expected, actual);

//            actual = ConsoleProcessor.IsValidVarName(" 100500");
//            Assert.AreEqual(expected, actual);

//            actual = ConsoleProcessor.IsValidVarName("i");
//            Assert.AreEqual(expected, actual);

//            actual = ConsoleProcessor.IsValidVarName("   \t    \r I  ");
//            Assert.AreEqual(expected, actual);
//        }
//    }
//}