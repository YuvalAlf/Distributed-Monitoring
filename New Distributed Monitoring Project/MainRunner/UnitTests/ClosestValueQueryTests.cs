using System;
using EntropyMathematics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class ClosestValueQueryTests
    {
        [TestMethod]
        public void GeneralTest()
        {
            var query = ClosestValueQuery.Init(ArrayUtils.Init(1,6,9,20, -20));
            Assert.AreEqual(1, query.GetClosestSmallerValue(5));
            Assert.AreEqual(20, query.GetClosestSmallerValue(100));
            Assert.AreEqual(9, query.GetClosestSmallerValue(9));
            Assert.AreEqual(9, query.GetClosestSmallerValue(10));
            Assert.AreEqual(9, query.GetClosestSmallerValue(13));
            Assert.AreEqual(-20, query.GetClosestSmallerValue(-5));
            Assert.ThrowsException<IndexOutOfRangeException>(() => query.GetClosestSmallerValue(-100));
        }
        [TestMethod]
        public void ExponensialGeneratorTest()
        {
            var query = ClosestValueQuery.InitExponential(2, 65556, 2.0);
            Assert.AreEqual(0, query.GetClosestSmallerValue(1));
            Assert.AreEqual(0, query.GetClosestSmallerValue(0));
            Assert.AreEqual(2, query.GetClosestSmallerValue(3));
            Assert.AreEqual(2, query.GetClosestSmallerValue(2));
            Assert.AreEqual(8, query.GetClosestSmallerValue(15));
            Assert.AreEqual(16, query.GetClosestSmallerValue(16));
            Assert.AreEqual(256, query.GetClosestSmallerValue(333));
            Assert.AreEqual(512, query.GetClosestSmallerValue(777));
            Assert.AreEqual(512, query.GetClosestSmallerValue(512));
            Assert.AreEqual(512, query.GetClosestSmallerValue(1023));
            Assert.ThrowsException<IndexOutOfRangeException>(() => query.GetClosestSmallerValue(-100));
        }
        
    }
}
