using System;
using System.Collections.Generic;
using System.IO;
using Utils.TypeUtils;

namespace Utils.AiderTypes.TaxiTrips
{
    public sealed class TaxiTripEntry : IComparable<TaxiTripEntry>
    {
        public float PickupLongtitude { get; }
        public float PickupLatitude { get; }
        public float DropoffLongtitude { get; }
        public float DropoffLatitude { get; }
        public int NumOfPassangers { get; }
        public DateTime PickupTime { get; }
        public DateTime DropoffTime { get; }
        public PaymentType PaymentType { get; }
        public TaxiVendor TaxiVendor { get; }
        public float TripDistance { get; }
        public float TripTime { get; }
        public float Tip { get; }
        public float FareAmount { get; }
        public float TotalPayment { get; }

        public TaxiTripEntry(float pickupLongtitude, float pickupLatitude, float dropoffLongtitude, float dropoffLatitude, int numOfPassangers, DateTime pickupTime, DateTime dropoffTime, PaymentType paymentType, TaxiVendor taxiVendor, float tripDistance, float tripTime, float tip, float fareAmount, float totalPayment)
        {
            PickupLongtitude = pickupLongtitude;
            PickupLatitude = pickupLatitude;
            DropoffLongtitude = dropoffLongtitude;
            DropoffLatitude = dropoffLatitude;
            NumOfPassangers = numOfPassangers;
            PickupTime = pickupTime;
            DropoffTime = dropoffTime;
            PaymentType = paymentType;
            TaxiVendor = taxiVendor;
            TripDistance = tripDistance;
            TripTime = tripTime;
            Tip = tip;
            FareAmount = fareAmount;
            TotalPayment = totalPayment;
        }

        public int CompareTo(TaxiTripEntry other) => this.PickupTime.CompareTo(other.PickupTime);

        public static IEnumerable<TaxiTripEntry> FromBinary(BinaryReader binaryReader)
        {
            while (!binaryReader.IsEOF())
                yield return TaxiTripEntry.SingleFromBinary(binaryReader);
        }

        private static TaxiTripEntry SingleFromBinary(BinaryReader binaryReader)
        {
            var pickupLongtitude  = binaryReader.ReadSingle();
            var pickupLatitude    = binaryReader.ReadSingle();
            var dropoffLongtitude = binaryReader.ReadSingle();
            var dropoffLatitude   = binaryReader.ReadSingle();
            var numOfPassangers   = binaryReader.ReadInt32();
            var pickupTime        = DateTime.FromBinary(binaryReader.ReadInt64());
            var dropoffTime       = DateTime.FromBinary(binaryReader.ReadInt64());
            var paymentType       = PaymentTypeUtils.FromBinary(binaryReader.ReadByte()).ValueUnsafe;
            var taxiVendor        = TaxiVendorUtils.FromBinary(binaryReader.ReadByte()).ValueUnsafe;
            var tripDistance      = binaryReader.ReadSingle();
            var tripTime          = binaryReader.ReadSingle();
            var tip               = binaryReader.ReadSingle();
            var fareAmount        = binaryReader.ReadSingle();
            var totalPayment      = binaryReader.ReadSingle();

            return new TaxiTripEntry(pickupLongtitude, pickupLatitude, dropoffLongtitude, dropoffLatitude,
                                     numOfPassangers, pickupTime, dropoffTime, paymentType, taxiVendor, tripDistance,
                                     tripTime, tip, fareAmount,
                                     totalPayment);
        }

        public void ToBinary(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(PickupLongtitude);
            binaryWriter.Write(PickupLatitude);
            binaryWriter.Write(DropoffLongtitude);
            binaryWriter.Write(DropoffLatitude);
            binaryWriter.Write(NumOfPassangers);
            binaryWriter.Write(PickupTime.ToBinary());
            binaryWriter.Write(DropoffTime.ToBinary());
            binaryWriter.Write(PaymentType.ToBinary());
            binaryWriter.Write(TaxiVendor.ToBinary());
            binaryWriter.Write(TripDistance);
            binaryWriter.Write(TripTime);
            binaryWriter.Write(Tip);
            binaryWriter.Write(FareAmount);
            binaryWriter.Write(TotalPayment);
        }

        public static Maybe<TaxiTripEntry> TryParse(string tripDataCsvLine, string tripFareCsvLine)
        {
            var dataTokens = tripDataCsvLine.SplitCsv();
            var fareTokens = tripFareCsvLine.SplitCsv();
            return
                (dataTokens.Length == 14)
                .BindMaybe(() => (fareTokens.Length == 11)
                .BindMaybe(() => dataTokens[0].Equals(fareTokens[0])
                .BindMaybe(() => TaxiVendorUtils.TryParse(dataTokens[2])
                .Bind(vendor => dataTokens[5].TryParseDateTime()
                .Bind(pickupDateTime => dataTokens[6].TryParseDateTime()
                .Bind(dropoffDateTime => dataTokens[7].TryParseInt()
                .Bind(passengerCount => dataTokens[8].TryParseFloat()
                .Bind(tripTime => dataTokens[9].TryParseFloat()
                .Bind(tripDistance => dataTokens[10].TryParseFloat()
                .Bind(pickupLongtitude => dataTokens[11].TryParseFloat()
                .Bind(pickupLatitude => dataTokens[12].TryParseFloat()
                .Bind(dropoffLongtitude => dataTokens[13].TryParseFloat()
                .Bind(dropoffLatitude => pickupLongtitude.InRange(-75f, -70f)
                .BindMaybe(() => dropoffLongtitude.InRange(-75f, -70f)
                .BindMaybe(() => pickupLatitude.InRange(35f, 45f)
                .BindMaybe(() => dropoffLatitude.InRange(35f, 45f)
                .BindMaybe(() => PaymentTypeUtils.TryParse(fareTokens[4])
                .Bind(paymentType => fareTokens[5].TryParseFloat()
                .Bind(fareAmount => fareTokens[8].TryParseFloat()
                .Bind(tipAmount => fareTokens[10].TryParseFloat()
                .Map(totalAmount => new TaxiTripEntry(pickupLongtitude, pickupLatitude, dropoffLongtitude, dropoffLatitude, passengerCount, pickupDateTime, dropoffDateTime, paymentType, vendor, tripDistance, tripTime, tipAmount, fareAmount, totalAmount))))))))))))))))))))));
        }

        public static string CsvHeader()
        {
            return "PickupLongtitude"
                  .ConcatCsv("PickupLatitude")
                  .ConcatCsv("DropoffLongtitude")
                  .ConcatCsv("DropoffLatitude")
                  .ConcatCsv("NumOfPassangers")
                  .ConcatCsv("PickupTime")
                  .ConcatCsv("DropoffTime")
                  .ConcatCsv("PaymentType")
                  .ConcatCsv("TaxiVendor")
                  .ConcatCsv("TripDistance")
                  .ConcatCsv("TripTime")
                  .ConcatCsv("Tip")
                  .ConcatCsv("FareAmount")
                  .ConcatCsv("TotalPayment");
        }

        public string AsCsv()
        {
            return PickupLongtitude.ToString()
                  .ConcatCsv(PickupLatitude.ToString())
                  .ConcatCsv(DropoffLongtitude.ToString())
                  .ConcatCsv(DropoffLatitude.ToString())
                  .ConcatCsv(NumOfPassangers.ToString())
                  .ConcatCsv(PickupTime.ToString("F").Replace(',', ' '))
                  .ConcatCsv(DropoffTime.ToString("F").Replace(',', ' '))
                  .ConcatCsv(PaymentType.AsString())
                  .ConcatCsv(TaxiVendor.AsString())
                  .ConcatCsv(TripDistance.ToString())
                  .ConcatCsv(TripTime.ToString())
                  .ConcatCsv(Tip.ToString())
                  .ConcatCsv(FareAmount.ToString())
                  .ConcatCsv(TotalPayment.ToString());
        }
    }
}
