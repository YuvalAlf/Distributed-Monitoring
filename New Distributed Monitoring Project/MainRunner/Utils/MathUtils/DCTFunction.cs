using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils
{
    /*
    public static class DCTFunction
    {
        public static void DCT(double[] vector)
        {
            if (vector == null)
                throw new NullReferenceException();
            var len     = vector.Length;
            var halfLen = len / 2;
            var temp    = new Complex[len];
            for (int i = 0; i < halfLen; i++)
            {
                temp[i]           = vector[i * 2];
                temp[len - 1 - i] = vector[i * 2 + 1];
            }
            if (len % 2 == 1)
                temp[halfLen] = vector[len - 1];
            temp.Transform(false);
            for (int i = 0; i < len; i++)
                vector[i] = (temp[i] * Complex.Exp(new Complex(0, -i * Math.PI / (len * 2)))).Real;
        }
        public static void IDCT(double[] vector)
        {
            if (vector == null)
                throw new NullReferenceException();
            int len = vector.Length;
            if (len > 0)
                vector[0] /= 2;
            Complex[] temp = new Complex[len];
            for (int i = 0; i < len; i++)
                temp[i] = vector[i] * Complex.Exp(new Complex(0, -i * Math.PI / (len * 2)));
            temp.Transform(false);

            int halfLen = len / 2;
            for (int i = 0; i < halfLen; i++)
            {
                vector[i * 2 + 0] = temp[i].Real;
                vector[i * 2 + 1] = temp[len - 1 - i].Real;
            }
            if (len % 2 == 1)
                vector[len - 1] = temp[halfLen].Real;
            for (int i = 0; i < len; i++)
                vector[i] /= 2;
        }
        public static void Transform(this Complex[] vector, bool inverse)
        {
            int n = vector.Length;
            if (n == 0)
                return;
            else if ((n & (n - 1)) == 0) // Is power of 2
                TransformRadix2(vector, inverse);
            else // More complicated algorithm for arbitrary sizes
                TransformBluestein(vector, inverse);
        }
        public static void TransformRadix2(Complex[] vector, bool inverse)
        {
            // Length variables
            int n = vector.Length;
            int levels = 0;  // compute levels = floor(log2(n))
            for (int temp = n; temp > 1; temp >>= 1)
                levels++;
            if (1 << levels != n)
                throw new ArgumentException("Length is not a power of 2");

            // Trigonometric table
            Complex[] expTable = new Complex[n / 2];
            double coef = 2 * Math.PI / n * (inverse ? 1 : -1);
            for (int i = 0; i < n / 2; i++)
                expTable[i] = Complex.Exp(new Complex(0, i * coef));

            // Bit-reversed addressing permutation
            for (int i = 0; i < n; i++)
            {
                int j = (int)((uint)ReverseBits(i) >> (32 - levels));
                if (j > i)
                {
                    Complex temp = vector[i];
                    vector[i] = vector[j];
                    vector[j] = temp;
                }
            }

            // Cooley-Tukey decimation-in-time radix-2 FFT
            for (int size = 2; size <= n; size *= 2)
            {
                int halfsize = size / 2;
                int tablestep = n / size;
                for (int i = 0; i < n; i += size)
                {
                    for (int j = i, k = 0; j < i + halfsize; j++, k += tablestep)
                    {
                        Complex temp = vector[j + halfsize] * expTable[k];
                        vector[j + halfsize] = vector[j] - temp;
                        vector[j] += temp;
                    }
                }
                if (size == n)  // Prevent overflow in 'size *= 2'
                    break;
            }
        }


          
        public static void TransformBluestein(Complex[] vector, bool inverse)
        {
            // Find a power-of-2 convolution length m such that m >= n * 2 + 1
            int n = vector.Length;
            if (n >= 0x20000000)
                throw new ArgumentException("Array too large");
            int m = 1;
            while (m < n * 2 + 1)
                m *= 2;

            // Trignometric table
            Complex[] expTable = new Complex[n];
            double coef = Math.PI / n * (inverse ? 1 : -1);
            for (int i = 0; i < n; i++)
            {
                int j = (int)((long)i * i % (n * 2));  // This is more accurate than j = i * i
                expTable[i] = Complex.Exp(new Complex(0, j * coef));
            }

            // Temporary vectors and preprocessing
            Complex[] avector = new Complex[m];
            for (int i = 0; i < n; i++)
                avector[i] = vector[i] * expTable[i];
            Complex[] bvector = new Complex[m];
            bvector[0] = expTable[0];
            for (int i = 1; i < n; i++)
                bvector[i] = bvector[m - i] = Complex.Conjugate(expTable[i]);

            // Convolution
            Complex[] cvector = new Complex[m];
            Convolve(avector, bvector, cvector);

            // Postprocessing
            for (int i = 0; i < n; i++)
                vector[i] = cvector[i] * expTable[i];
        }

         
        public static void Convolve(Complex[] xvector, Complex[] yvector, Complex[] outvector)
        {
            int n = xvector.Length;
            if (n != yvector.Length || n != outvector.Length)
                throw new ArgumentException("Mismatched lengths");
            xvector = (Complex[])xvector.Clone();
            yvector = (Complex[])yvector.Clone();
            Transform(xvector, false);
            Transform(yvector, false);
            for (int i = 0; i < n; i++)
                xvector[i] *= yvector[i];
            Transform(xvector, true);
            for (int i = 0; i < n; i++)  // Scaling (because this FFT implementation omits it)
                outvector[i] = xvector[i] / n;
        }

        private static int ReverseBits(int val)
        {
            int result = 0;
            for (int i = 0; i < 32; i++, val >>= 1)
                result = (result << 1) | (val & 1);
            return result;
        }
    }*/
}
