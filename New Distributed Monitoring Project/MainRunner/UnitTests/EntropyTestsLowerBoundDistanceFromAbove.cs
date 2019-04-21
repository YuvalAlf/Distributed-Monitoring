using System;
using System.Runtime.Remoting.Services;
using Entropy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace UnitTests
{
    
    [TestClass]
    public class EntropyTestsLowerBoundDistanceFromAbove
    {
        private static Vector CreateVec(params double[] values) => values.ToVector();

        public void TestDistanceFromInside(Vector vector, double expectedEntropy, double threshold, int dimension)
        {
            Assert.AreEqual(1.0, vector.Sum(), 0.000000001);
            var entropyFunction = new EntropyFunction(dimension);
            var entropy = entropyFunction.ComputeEntropy(vector);
            Assert.AreEqual(expectedEntropy, entropy, EntropyFunction.Approximation);
            var closestPoint = entropyFunction.ClosestL1PointFromAbove(threshold, vector);
            var closestPointEntropy = entropyFunction.ComputeEntropy(closestPoint);
            Assert.AreEqual(threshold, closestPointEntropy, 2 * EntropyFunction.Approximation);
        }

        [TestMethod]
        public void DistanceInsideTest1()
        {
            var dimension = 6;
            var vec = CreateVec(0.1, 0.1, 0.1, 0.1, 0.2, 0.4);
            var entropy = 1.609437912;
            var threshold = 1.579437912;
            TestDistanceFromInside(vec, entropy, threshold, dimension);
        }
        /*
        [TestMethod]
        public void DistanceInsideTest2()
        {
            var dimension = 8;
            var vec = CreateVec(0, 0, 0.05, 0.1, 0.15, 0.2, 0.2, 0.3);
            var entropy = 1.669580127;
            var threshold = 1.589580127;
            TestDistanceFromInside(vec, entropy, threshold, dimension);
        }

        [TestMethod]
        public void DistanceInsideTes3()
        {
            var dimension = 10;
            var vec = CreateVec(0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1, 0.1);
            var entropy = 2.302585093;
            var threshold = 1.802585093;
            TestDistanceFromInside(vec, entropy, threshold, dimension);
        }*/
    }
}
