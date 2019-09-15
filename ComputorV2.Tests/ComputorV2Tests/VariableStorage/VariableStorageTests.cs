using ComputorV2;
using NUnit.Framework;

namespace ComputorV2Tests.ConsoleReaderTests.Unit
{
    public class VariableStorageTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("lalala", true)]
        [TestCase("varA",true)]
        [TestCase("  \t \r varA ",true)]
        [TestCase("", false)]
        [TestCase(" lalala1 ", false)]
        [TestCase("name name", false)]
        [TestCase(" 100500", false)]
        [TestCase("i", false)]
        [TestCase("I", false)]
        public void IsValidVarName_valid(string varname, bool expected)
        {
            var actual = VariableStorage.IsValidVarName(varname);
            Assert.AreEqual(expected, actual);
        }
    }
}