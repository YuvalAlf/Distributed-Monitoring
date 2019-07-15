using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxiTripsDataParsing;

namespace UnitTests
{
    [TestClass]
    public class CityRegionTests
    {
        [TestMethod]
        public void Test1()
        {
            var cityRegion = new CityRegion(0, 2.0, 0, 1.0);
            Assert.AreEqual(0, cityRegion.Get(3, 0, 0.2));
            Assert.AreEqual(1, cityRegion.Get(3, 1.0, 0.3));
            Assert.AreEqual(2, cityRegion.Get(3, 1.5, 0.1));
            Assert.AreEqual(4, cityRegion.Get(3, 1.0, 0.5));
            Assert.AreEqual(7, cityRegion.Get(3, 1.22, 0.8673));
        }
    }
}
