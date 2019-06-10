using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.AiderTypes;
using Utils.AiderTypes.TaxiTrips;

namespace UnitTests
{

    [TestClass]
    public class TaxiTripsParsing
    {
        [TestMethod]
        public void TestBinary()
        {

            using (var memmoryStream = new MemoryStream(new byte[1024], true))
            {
                var taxiTrip = new TaxiTripEntry(1f, 2f, 3f, 4f, 5, DateTime.Now, DateTime.UtcNow, PaymentType.Credit,
                                                 TaxiVendor.CMT, 22f, 33f, 44f, 55f, 66f);
                using (var binaryWriter = new BinaryWriter(memmoryStream))
                using (var binaryReader = new BinaryReader(memmoryStream))
                {
                    taxiTrip.ToBinary(binaryWriter);
                    binaryWriter.Flush();
                    memmoryStream.Position = 0;
                    var deserialized = TaxiTripEntry.FromBinary(binaryReader).First();
                    Assert.AreEqual(taxiTrip.PaymentType,       deserialized.PaymentType);
                    Assert.AreEqual(taxiTrip.PickupTime,        deserialized.PickupTime);
                    Assert.AreEqual(taxiTrip.DropoffLatitude,   deserialized.DropoffLatitude);
                    Assert.AreEqual(taxiTrip.DropoffLongtitude, deserialized.DropoffLongtitude);
                    Assert.AreEqual(taxiTrip.DropoffTime,       deserialized.DropoffTime);
                    Assert.AreEqual(taxiTrip.FareAmount,        deserialized.FareAmount);
                    Assert.AreEqual(taxiTrip.NumOfPassangers,   deserialized.NumOfPassangers);
                    Assert.AreEqual(taxiTrip.PickupLatitude,    deserialized.PickupLatitude);
                    Assert.AreEqual(taxiTrip.PickupLongtitude,  deserialized.PickupLongtitude);
                    Assert.AreEqual(taxiTrip.TaxiVendor,        deserialized.TaxiVendor);
                    Assert.AreEqual(taxiTrip.Tip,               deserialized.Tip);
                    Assert.AreEqual(taxiTrip.TotalPayment,      deserialized.TotalPayment);
                }
            }
        }

        [TestMethod]
        public void TestStringParsing()
        {
            var dataCsvLine = "2013000002,2013000002,VTS,1,,2013-01-01 00:00:00,2013-01-01 00:06:00,5,360,.98,-73.978325,40.778091,-73.981834,40.768639";
            var fareCsvLine = "2013000002,2013000002,VTS,2013-01-01 00:00:00,CSH,6,0.5,0.5,0,0,7";
            var result = TaxiTripEntry.TryParse(dataCsvLine, fareCsvLine);
            Assert.IsTrue(result.IsSome(out TaxiTripEntry taxiTrip));
            Assert.AreEqual(taxiTrip.PaymentType, PaymentType.Cash);
            Assert.AreEqual(taxiTrip.PickupTime, DateTime.Parse("2013-01-01 00:00:00"));
            Assert.AreEqual(taxiTrip.DropoffLatitude, 40.768639f, 0.000001f);
            Assert.AreEqual(taxiTrip.DropoffLongtitude, -73.9818340f, 0.000001f);
            Assert.AreEqual(taxiTrip.DropoffTime, DateTime.Parse("2013-01-01 00:06:00"));
            Assert.AreEqual(taxiTrip.FareAmount, 6);
            Assert.AreEqual(taxiTrip.NumOfPassangers, 5);
            Assert.AreEqual(taxiTrip.PickupLatitude, 40.778091f, 0.000001f);
            Assert.AreEqual(taxiTrip.PickupLongtitude, -73.978325f, 0.000001f);
            Assert.AreEqual(taxiTrip.TaxiVendor, TaxiVendor.VTS);
            Assert.AreEqual(taxiTrip.Tip, 0);
            Assert.AreEqual(taxiTrip.TotalPayment, 7);
        }
    }
}
