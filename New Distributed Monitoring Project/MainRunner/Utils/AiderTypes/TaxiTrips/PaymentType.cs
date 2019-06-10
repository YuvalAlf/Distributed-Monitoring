using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace Utils.AiderTypes.TaxiTrips
{
    public enum PaymentType : byte
    {
        Cash = 0,
        Credit = 1
    }

    public static class PaymentTypeUtils
    {
        public static Maybe<PaymentType> FromBinary(byte @byte)
        {
            switch (@byte)
            {
                case (byte)PaymentType.Credit:
                    return Maybe.Some(PaymentType.Credit);
                case (byte)PaymentType.Cash:
                    return Maybe.Some(PaymentType.Cash);
                default:
                    return Maybe.None<PaymentType>();
            }
        }
        public static Maybe<PaymentType> TryParse(string text)
        {
            switch (text)
            {
                case "CSH":
                    return Maybe.Some(PaymentType.Cash);
                case "CRD":
                    return Maybe.Some(PaymentType.Credit);
                default:
                    return Maybe.None<PaymentType>();
            }
        }

        public static byte ToBinary(this PaymentType @this) => (byte)@this;
    }
}
