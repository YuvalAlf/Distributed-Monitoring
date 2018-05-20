using System;
using System.Runtime.Remoting.Services;
using Entropy;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class EntropyTestsFromAbove
    {
        private static Vector<double> CreateVec(params double[] values) => values.ToVector();

        public void TestDistanceFromInside(Vector<double> vector, double expectedEntropy, double threshold)
        {
            Assert.AreEqual(1.0, vector.Sum(), 0.000000001);
            var entropy = EntropyFunction.ComputeEntropy(vector);
            Assert.AreEqual(expectedEntropy, entropy, EntropyMath.Approximation);
            var closestPoint = EntropyMath.ClosestL1PointFromAbove(threshold, vector);
            var closestPointEntropy = EntropyFunction.LowerBoundConvexBoundEntropy(closestPoint);
            Assert.AreEqual(threshold, closestPointEntropy, 2 * EntropyMath.Approximation);
        }

        [TestMethod]
        public void DistanceInsideTest1()
        {
            var vec = CreateVec(0.1, 0.1, 0.1, 0.1, 0.2, 0.4);
            var entropy = 1.609437912;
            var threshold = 1.579437912;
            TestDistanceFromInside(vec, entropy, threshold);
        }

        [TestMethod]
        public void DistanceInsideTest2()
        {
            var vec = CreateVec(0, 0, 0.05, 0.1, 0.15, 0.2, 0.2, 0.3);
            var entropy = 1.669580127;
            var threshold = 1.589580127;
            TestDistanceFromInside(vec, entropy, threshold);
        }

        [TestMethod]
        public void DistanceInsideTes3()
        {
            var vec = CreateVec(0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1);
            var entropy = 2.302585093;
            var threshold = 1.802585093;
            TestDistanceFromInside(vec, entropy, threshold);
        }
    }
}
