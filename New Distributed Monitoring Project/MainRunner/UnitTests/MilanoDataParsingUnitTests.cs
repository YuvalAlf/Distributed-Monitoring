using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MilanoPhonesDataParsing;
using Utils.AiderTypes;

namespace UnitTests
{
    [TestClass]
    public class MilanoDataParsingUnitTests
    {
        [TestMethod]
        public void TestBinary1()
        {
            using (var memmoryStream = new MemoryStream(new byte[1024], true))
            {
                var phoneActivity = new PhoneActivityEntry(35, 200, 0.23434);
                using (var binaryWriter = new BinaryWriter(memmoryStream))
                using (var binaryReader = new BinaryReader(memmoryStream))
                {
                    phoneActivity.ToBinary(binaryWriter);
                    binaryWriter.Flush();
                    memmoryStream.Position = 0;
                    var deserialized = PhoneActivityEntry.FromBinary(binaryReader).First();
                    Assert.AreEqual(phoneActivity, deserialized);
                }
            }
        }
        [TestMethod]
        public void TestBinary2()
        {
            using (var memmoryStream = new MemoryStream(new byte[1024], true))
            {
                var phoneActivity = new PhoneActivityEntry(1000, 10000, -123.12344);
                using (var binaryWriter = new BinaryWriter(memmoryStream))
                using (var binaryReader = new BinaryReader(memmoryStream))
                {
                    phoneActivity.ToBinary(binaryWriter);
                    binaryWriter.Flush();
                    memmoryStream.Position = 0;
                    var deserialized = PhoneActivityEntry.FromBinary(binaryReader).First();
                    Assert.AreEqual(phoneActivity, deserialized);
                }
            }
        }
    }
}
