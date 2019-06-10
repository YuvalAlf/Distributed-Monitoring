using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monitoring.Utils.DataDistributing;

namespace UnitTests
{
    [TestClass]
    public class GridGeographicalDistributingUnitTests
    {
        [TestMethod]
        public void Test1()
        {
            var gridDistributing = new GridDistributing(1, 81, 9);
            Assert.AreEqual(1, 1 + gridDistributing.NodeOf(1));
            Assert.AreEqual(1, 1 + gridDistributing.NodeOf(2));
            Assert.AreEqual(1, 1 + gridDistributing.NodeOf(3));
            Assert.AreEqual(2, 1 + gridDistributing.NodeOf(4));
            Assert.AreEqual(2, 1 + gridDistributing.NodeOf(5));
            Assert.AreEqual(2, 1 + gridDistributing.NodeOf(6));
            Assert.AreEqual(3, 1 + gridDistributing.NodeOf(7));
            Assert.AreEqual(3, 1 + gridDistributing.NodeOf(8));
            Assert.AreEqual(3, 1 + gridDistributing.NodeOf(9));
            Assert.AreEqual(1, 1 + gridDistributing.NodeOf(10));
            Assert.AreEqual(1, 1 + gridDistributing.NodeOf(11));
            Assert.AreEqual(1, 1 + gridDistributing.NodeOf(12));
            Assert.AreEqual(2, 1 + gridDistributing.NodeOf(13));
            Assert.AreEqual(9, 1 + gridDistributing.NodeOf(81));
            Assert.AreEqual(9, 1 + gridDistributing.NodeOf(80));
            Assert.AreEqual(9, 1 + gridDistributing.NodeOf(79));
            Assert.AreEqual(8, 1 + gridDistributing.NodeOf(78));
            Assert.AreEqual(8, 1 + gridDistributing.NodeOf(77));
            Assert.AreEqual(8, 1 + gridDistributing.NodeOf(76));
            Assert.AreEqual(7, 1 + gridDistributing.NodeOf(75));
        }
    }
}
