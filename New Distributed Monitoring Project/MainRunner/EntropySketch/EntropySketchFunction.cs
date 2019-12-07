using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.DataStructures;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;
using MersenneTwisterPrng = MathNet.Numerics.Random.MersenneTwister;
namespace EntropySketch
{
    public sealed partial class EntropySketchFunction
    {
        public int               Dimension         { get; }
        public MonitoredFunction MonitoredFunction { get; }


        public EntropySketchFunction(int dimension)
        {
            Dimension         = dimension;
            MonitoredFunction = new MonitoredFunction(ComputeEntropySketch, UpperBound, LowerBound, GlobalVectorType.Average, 1);
        }

        public double ComputeEntropySketch(Vector reducedVector)
            => Math.Log(Dimension) - reducedVector.Enumerate(Dimension).LogSumExp();

       // private Cache<MersenneTwisterPrng> MersenneTwisters { get; } = new Cache<MersenneTwisterPrng>(20000, i => new MersenneTwisterPrng(i));

        private static HashAlgorithm Hash { get; } = MD5.Create();

        private double GenF(int reductionIndex, MersenneTwisterPrng rnd)
        {
            var u1     = rnd.GenDoubleWithout(0.0, 1.0);
            var u2     = rnd.GenDoubleWithout(0.0, 1.0);
            var w1     = Math.PI * (u1 - 0.5);
            var w2     = -Math.Log(u2);
            return Math.Tan(w1) * (Math.PI / 2 - w1) + Math.Log(w2 * Math.Cos(w1) / (Math.PI / 2 - w1));
        }

        public Vector CollapseValue(double value, int bucketIndex)
        {
            var prng = new MersenneTwisterPrng(bucketIndex);
            return ArrayUtils.Init(Dimension, reductionIndex => value * GenF(reductionIndex, prng)).ToVector();
        }

        public Vector CollapseProbabilityVector(Vector probabilityVector)
            => Vector.SumVector(probabilityVector.IndexedValues.Select(kp => CollapseValue(kp.Value, kp.Key)));
    }
}
