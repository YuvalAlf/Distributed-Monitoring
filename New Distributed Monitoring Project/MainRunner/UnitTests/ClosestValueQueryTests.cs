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
            int[] array = ArrayUtils.Init(1, 6, 9, 20, -20);
            var query = ClosestValueQuery.Init(array);
            Array.Sort(array);
            Assert.AreEqual(Array.IndexOf(array, 1), query.GetClosestSmallerValueIndex(5));
            Assert.AreEqual(Array.IndexOf(array, 20),  query.GetClosestSmallerValueIndex(100));
            Assert.AreEqual(Array.IndexOf(array, 9),   query.GetClosestSmallerValueIndex(9));
            Assert.AreEqual(Array.IndexOf(array, 9),   query.GetClosestSmallerValueIndex(10));
            Assert.AreEqual(Array.IndexOf(array, 9),   query.GetClosestSmallerValueIndex(13));
            Assert.AreEqual(Array.IndexOf(array, -20), query.GetClosestSmallerValueIndex(-5));
            Assert.ThrowsException<IndexOutOfRangeException>(() => query.GetClosestSmallerValueIndex(-100));
        }
        [TestMethod]
        public void ExponensialGeneratorTest()
        {
            var query = ClosestValueQuery.InitExponential(2, 65556, 2.0);
            var array = query.Data;
            Assert.AreEqual(Array.IndexOf(array, 0),   query.GetClosestSmallerValueIndex(1));
            Assert.AreEqual(Array.IndexOf(array, 0),   query.GetClosestSmallerValueIndex(0));
            Assert.AreEqual(Array.IndexOf(array, 2),   query.GetClosestSmallerValueIndex(3));
            Assert.AreEqual(Array.IndexOf(array, 2),   query.GetClosestSmallerValueIndex(2));
            Assert.AreEqual(Array.IndexOf(array, 8),   query.GetClosestSmallerValueIndex(15));
            Assert.AreEqual(Array.IndexOf(array, 16),  query.GetClosestSmallerValueIndex(16));
            Assert.AreEqual(Array.IndexOf(array, 256), query.GetClosestSmallerValueIndex(333));
            Assert.AreEqual(Array.IndexOf(array, 512), query.GetClosestSmallerValueIndex(777));
            Assert.AreEqual(Array.IndexOf(array, 512), query.GetClosestSmallerValueIndex(512));
            Assert.AreEqual(Array.IndexOf(array, 512), query.GetClosestSmallerValueIndex(1023));
            Assert.ThrowsException<IndexOutOfRangeException>(() => query.GetClosestSmallerValueIndex(-100));
        }
        
    }
}
