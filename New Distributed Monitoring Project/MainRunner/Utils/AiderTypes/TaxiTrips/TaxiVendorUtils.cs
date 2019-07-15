using System;
using Utils.TypeUtils;

namespace Utils.AiderTypes.TaxiTrips
{
    public static class TaxiVendorUtils
    {
        public static string AsString(this TaxiVendor @this)
        {
            switch (@this)
            {
                case TaxiVendor.CMT:
                    return "CMT";
                case TaxiVendor.VTS:
                    return "VTS";
                default:
                    throw new ArgumentException();
            }
        }
        public static Maybe<TaxiVendor> FromBinary(byte @byte)
        {
            switch (@byte)
            {
                case (byte)TaxiVendor.VTS:
                    return Maybe.Some(TaxiVendor.VTS);
                case (byte)TaxiVendor.CMT:
                    return Maybe.Some(TaxiVendor.CMT);
                default:
                    return Maybe.None<TaxiVendor>();
            }
        }
        public static Maybe<TaxiVendor> TryParse(string text)
        {
            switch (text)
            {
                case "VTS":
                    return Maybe.Some(TaxiVendor.VTS);
                case "CMT":
                    return Maybe.Some(TaxiVendor.CMT);
                default:
                    return Maybe.None<TaxiVendor>();
            }
        }

        public static byte ToBinary(this TaxiVendor @this) => (byte)@this;
    }
}