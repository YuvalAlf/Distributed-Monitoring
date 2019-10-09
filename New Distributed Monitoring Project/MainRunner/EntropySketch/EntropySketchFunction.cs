using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace EntropySketch
{
    public sealed partial class EntropySketchFunction
    {
        public int               Dimension         { get; }
        public MonitoredFunction MonitoredFunction { get; }


        public EntropySketchFunction(int dimension)
        {
            Dimension         = dimension;
            MonitoredFunction = new MonitoredFunction(ComputeEntropySketch, UpperBound, LowerBound, GlobalVectorType.Average);
        }

        private double ComputeEntropySketch(Vector reducedVector)
            => -Math.Log(1.0 / Dimension * reducedVector.Enumerate(Dimension).Sum(yi => Math.Exp(yi)));


        private static HashAlgorithm Hash { get; } = MD5.Create();

        private static double GenF(int reductionIndex, int bucketIndex)
        {
            var random = new Random(Hash.ComputeHash(reductionIndex, bucketIndex));
            var rnd    = new Random(random.Next() ^ bucketIndex);
            var u1     = rnd.GenDoubleWithout(0.0, 1.0);
            var u2     = rnd.GenDoubleWithout(0.0, 1.0);
            var w1     = Math.PI * (u1 - 0.5);
            var w2     = -Math.Log(u2);
            return Math.Tan(w1) * (Math.PI / 2 - w1) + Math.Log(w2 * Math.Cos(w1) / (Math.PI / 2 - w1));
        }

        public Vector CollapseValue(double value, int bucketIndex) 
            => ArrayUtils.Init(Dimension, reductionIndex => value * GenF(reductionIndex, bucketIndex)).ToVector();

        public Vector CollapseProbabilityVector(Vector probabilityVector)
            => Vector.SumVector(probabilityVector.IndexedValues.Select(kp => CollapseValue(kp.Value, kp.Key)));
    }
}
