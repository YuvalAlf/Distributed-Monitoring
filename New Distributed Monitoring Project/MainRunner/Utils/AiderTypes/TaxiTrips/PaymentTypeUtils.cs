using System;
using Utils.TypeUtils;

namespace Utils.AiderTypes.TaxiTrips
{
    public static class PaymentTypeUtils
    {
        public static string AsString(this PaymentType @this)
        {
            switch (@this)
            {
                case PaymentType.Credit:
                    return "Credit";
                case PaymentType.Cash:
                    return "Cash";
                default:
                    throw new ArgumentException();
            }
        }
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