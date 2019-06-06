using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class BinaryUtils
    {
        public static bool IsLittleEndian = BitConverter.IsLittleEndian;

        public static byte[] ToBytes(this int @this)
        {
            byte[] intBytes = BitConverter.GetBytes(@this);
            if (IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }
        public static int ToInt(this byte[] @this, StrongBox<int> index)
        {
            const int intLength = sizeof(int);
            if (IsLittleEndian)
                Array.Reverse(@this, index.Value, intLength);
            var @int = BitConverter.ToInt32(@this, index.Value);
            index.Value += intLength;
            return @int;
        }

        public static byte[] ToBytes(this double @this)
        {
            byte[] doubleBytes = BitConverter.GetBytes(@this);
            if (IsLittleEndian)
                Array.Reverse(doubleBytes);
            return doubleBytes;
        }
        public static double ToDouble(this byte[] @this, StrongBox<int> index)
        {
            const int doubleLength = sizeof(double);
            if (IsLittleEndian)
                Array.Reverse(@this, index.Value, doubleLength);
            var @double = BitConverter.ToDouble(@this, index.Value);
            index.Value += doubleLength;
            return @double;
        }
    }
}
