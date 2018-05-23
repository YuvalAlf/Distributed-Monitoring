using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entropy;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class EntropyTestsFromBelow
    {
        private static Vector<double> CreateVec(params double[] values) => values.ToVector();

        public void TestDistanceFromOutside(Vector<double> vector, double expectedEntropy, double threshold, double expectedDistance)
        {
            Assert.AreEqual(1.0, vector.Sum(), 0.000000001);
            var entropy = EntropyFunction.ComputeEntropy(vector);
            Assert.AreEqual(expectedEntropy, entropy, EntropyMath.Approximation);
            var closestPoint = EntropyMath.ClosestL1PointFromBelow(threshold, vector);
            var closestPointEntropy = EntropyFunction.ComputeEntropy(closestPoint);
         //   Assert.AreEqual(threshold, closestPointEntropy, 2 * EntropyMath.Approximation);
            var distance = (closestPoint - vector).L1Norm();
            Assert.AreEqual(expectedDistance, distance, EntropyMath.Approximation);
        }

        [TestMethod]
        public void DistanceOutsideTest1()
        {
            var vec = CreateVec(0.1, 0.2, 0.3, 0.4);
            var entropy = 1.279854226;
            var threshold = 1.379854226;
            var distance = 0.286630683;
            TestDistanceFromOutside(vec, entropy, threshold, distance);
        }
        [TestMethod]
        public void DistanceOutsideTest2()
        {
            var vec = CreateVec(0.05, 0.05, 0.05, 0.05, 0.1, 0.1, 0.1, 0.1, 0.2, 0.2);
            var entropy = 2.163955657;
            var threshold = 2.263955657;
            var distance = 0.190701889;
            TestDistanceFromOutside(vec, entropy, threshold, distance);
        }
        [TestMethod]
        public void DistanceOutsideTest3()
        {
            var vec = CreateVec(1, 0,0,0, 0,0,0,0, 0,0,0,0);
            var entropy = 0.0;
            var threshold = 0.5;
            var distance = 0.17225069;
            TestDistanceFromOutside(vec, entropy, threshold, distance);
        }
        
    }
}
