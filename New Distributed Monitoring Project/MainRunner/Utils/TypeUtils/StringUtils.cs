using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using static Utils.TypeUtils.Maybe;

namespace Utils.TypeUtils
{
    public static class StringUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<int> TryParseInt(this string @this) 
            => int.TryParse(@this, out var result) == true ? Some(result) : None<int>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<float> TryParseFloat(this string @this) 
            => float.TryParse(@this, out var result) == true ? Some(result) : None<float>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<double> TryParseDouble(this string @this) 
            => double.TryParse(@this, out var result) == true ? Some(result) : None<double>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<DateTime> TryParseDateTime(this string @this) 
            => DateTime.TryParse(@this, out var result) == true ? Some(result) : None<DateTime>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Maybe<byte> TryParseByte(this string @this)
            => byte.TryParse(@this, out var result) == true ? Some(result) : None<byte>();


        public static string ConcatCsv(this string @this, string other) => @this + "," + other;

        private static readonly char[] csvComma = new[] {','};
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] SplitCsv(this string line) => line.Split(csvComma, StringSplitOptions.None);

        public static string AsCsvString(this double @this)
        {
            if (double.IsPositiveInfinity(@this))
                return "=\"+Infinity\"";
            if (double.IsNegativeInfinity(@this))
                return "=\"-Infinity\"";
            if (double.IsNaN(@this))
                return "NaN";
            return @this.ToString(CultureInfo.InvariantCulture);
        }
    }
}
