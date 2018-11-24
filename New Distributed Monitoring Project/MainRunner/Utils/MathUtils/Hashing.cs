using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils
{
    public static class Hashing
    {
        public static int ComputeHash(this HashAlgorithm hash, int a, int b, int c)
        {
            var input = BitConverter.GetBytes(a).Concat(BitConverter.GetBytes(b)).Concat(BitConverter.GetBytes(c)).ToArray();
            var output = hash.ComputeHash(input);
            return Math.Abs(BitConverter.ToInt32(output, 0));
        }
    }
}
