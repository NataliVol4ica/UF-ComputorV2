﻿using System.Collections.Generic;
using NUnit.Framework;

namespace BigNumbers.Tests.BigDecimalTests
{
    [TestFixture]
    public class BigDecimalBigDecimalToIntListTest
    {
        public void CompareLists(List<int> expected, List<int>actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }

        [Test]
        public void NullTest()
        {
            List<int> actual = BigDecimal.BigDecimalToIntList(null, 0, 0);
            Assert.IsNull(actual);
        }

        [Test]
        public void Zero_Test()
        {
            var bf = new BigDecimal("0");
            List<int> actual = BigDecimal.BigDecimalToIntList(bf, 0, 0);
            List<int> expected = new List<int> { 0 };
            CompareLists(expected, actual);
        }
        [Test]
        public void Zero23_Test()
        {
            var bf = new BigDecimal("0");
            List<int> actual = BigDecimal.BigDecimalToIntList(bf, 2, 3);
            List<int> expected = new List<int> { 0, 0, 0, 0, 0 };
            CompareLists(expected, actual);
        }

        [Test]
        public void Digit23_Test()
        {
            var bf = new BigDecimal("5");
            List<int> actual = BigDecimal.BigDecimalToIntList(bf, 2, 3);
            List<int> expected = new List<int> { 0, 0, 0, 5, 0 };
            CompareLists(expected, actual);
        }

        [Test]
        public void Digits32_Test()
        {
            var bf = new BigDecimal("65.1");
            List<int> actual = BigDecimal.BigDecimalToIntList(bf, 3, 2);
            List<int> expected = new List<int> { 0, 1, 5, 6, 0 };
            CompareLists(expected, actual);
        }

        [Test]
        public void Digits13_Test()
        {
            var bf = new BigDecimal("65.1");
            List<int> actual = BigDecimal.BigDecimalToIntList(bf, 1, 3);
            List<int> expected = new List<int> { 0, 0, 1, 5, 6 };
            CompareLists(expected, actual);
        }
    }
}
