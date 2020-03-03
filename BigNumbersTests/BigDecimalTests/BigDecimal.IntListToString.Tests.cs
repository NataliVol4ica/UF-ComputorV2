using System.Collections.Generic;
using BigNumbers;
using NUnit.Framework;

namespace BigNumbersTests.BigDecimalTests
{
    [TestFixture]
    public class BigDecimalIntListToStringTests
    {
        [Test]
        public void EmptyTest()
        {
            var actual = BigDecimal.IntListToString(new List<int>());
            var expected = "";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NullTest()
        {
            var actual = BigDecimal.IntListToString(null);
            var expected = "";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Num0598Dot20836610_Test()
        {
            var list = new List<int> {0, 1, 6, 6, 3, 8, 0, 2, 8, 9, 5, 0};
            var actual = BigDecimal.IntListToString(list, 4);
            var expected = "0598.20836610";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Num0Dot15_Test()
        {
            var list = new List<int> {5, 1, 0};
            var actual = BigDecimal.IntListToString(list, 1);
            var expected = "0.15";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Num56Dot7_Test()
        {
            var list = new List<int> {7, 6, 5};
            var actual = BigDecimal.IntListToString(list, 2);
            var expected = "56.7";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SimpleTest()
        {
            var list = new List<int> {5};
            var actual = BigDecimal.IntListToString(list, 1);
            var expected = "5";
            Assert.AreEqual(expected, actual);
        }
    }
}