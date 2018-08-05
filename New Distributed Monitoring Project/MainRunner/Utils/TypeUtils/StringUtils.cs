using System.Globalization;
using System.Linq;

namespace Utils.TypeUtils
{
    public static class StringUtils
    {
        public static string ConcatCsv(this string @this, string other) => @this + "," + other;

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


        public static string CleanWord(this string word)
        {
            word = word.ToLower(CultureInfo.InvariantCulture);
            word = new string(word.Where(ch => char.IsLetter(ch) || ch == '\'').ToArray());
            return word;
        }
    }
}
