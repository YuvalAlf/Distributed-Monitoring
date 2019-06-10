using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monitoring.Utils.DataDistributing;

namespace UnitTests
{
    [TestClass]
    public class HorizontalGeographicalDistributingUnitTests
    {
        public HorizontalDistributing horizontal = new HorizontalDistributing(10, 100, 10);

        [TestMethod]
        public void Test1()
        {
            Assert.AreEqual(0, horizontal.NodeOf(10));
            Assert.AreEqual(9, horizontal.NodeOf(100));
            Assert.ThrowsException<ArgumentException>(() => horizontal.NodeOf(101));
            Assert.ThrowsException<ArgumentException>(() => horizontal.NodeOf(9));
        }
    }
}
