using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace Utils.AiderTypes.TaxiTrips
{
    public enum TaxiVendor : byte
    {
        VTS = 0,
        CMT = 1
    }

    public static class TaxiVendorUtils
    {
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
