using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entropy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class EntropyTestsFromBelow
    {
        private static Vector CreateVec(params double[] values) => values.ToVector();

        public void TestDistanceFromOutside(Vector vector, double expectedEntropy, double threshold, double expectedDistance, int dimension)
        {
            Assert.AreEqual(1.0, vector.Sum(), 0.000000001);
            var entropyFunction = new EntropyFunction(dimension);
            var entropy = entropyFunction.ComputeEntropy(vector);
            Assert.AreEqual(expectedEntropy, entropy, EntropyFunction.Approximation);
            var closestPoint = entropyFunction.ClosestL1PointFromBelow(threshold, vector);
            var closestPointEntropy = entropyFunction.ComputeEntropy(closestPoint);
            Assert.AreEqual(threshold, closestPointEntropy, 2 * EntropyFunction.Approximation);
            var distance = (closestPoint - vector).L1Norm();
            Assert.AreEqual(expectedDistance, distance, EntropyFunction.Approximation);
        }

        [TestMethod]
        public void DistanceOutsideTest1()
        {
            var dimension = 4;
            var vec = CreateVec(0.1, 0.2, 0.3, 0.4);
            var entropy = 1.279854226;
            var threshold = 1.379854226;
            var distance = 0.286630683;
            TestDistanceFromOutside(vec, entropy, threshold, distance, dimension);
        }
        [TestMethod]
        public void DistanceOutsideTest2()
        {
            var dimension = 10;
            var vec = CreateVec(0.05, 0.05, 0.05, 0.05, 0.1, 0.1, 0.1, 0.1, 0.2, 0.2);
            var entropy = 2.163955657;
            var threshold = 2.263955657;
            var distance = 0.190701889;
            TestDistanceFromOutside(vec, entropy, threshold, distance, dimension);
        }
    }
}
