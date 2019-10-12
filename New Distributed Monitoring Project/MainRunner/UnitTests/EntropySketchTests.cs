using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntropyMathematics;
using EntropySketch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class EntropySketchTests
    {
        [TestMethod]
        public void TestLogSumExp()
        {
            var array    = ArrayUtils.Init(2.8, 9.9, 11.3, -3.4);
            var expected = Math.Log(array.Select(Math.Exp).Sum());
            var result   = array.LogSumExp();
            Assert.AreEqual(expected, result, 0.000001);
        }

        [TestMethod]
        public void TestDistanceIncrease1()
        {
            var dimension             = 4;
            var entropySketch         = new EntropySketchFunction(dimension);
            var befVector             = ArrayUtils.Init(2.8, 9.9, 11.3, -3.4).ToVector();
            var afVector              = ArrayUtils.Init(2.8, 5, 5, -3.4).ToVector();
            var befEntropySketchValue = entropySketch.ComputeEntropySketch(befVector);
            var afEntropySketchValue  = entropySketch.ComputeEntropySketch(afVector);
            var closestVector =
                IncreaseEntropySketch.l1IncreaseEntropySketchTo(befVector, dimension, afEntropySketchValue,
                                                                Approximations.ApproximationEpsilon);
            var resultMSE = closestVector.DistL2FromVector()(afVector);
            Assert.AreEqual(resultMSE, 0.0, 0.00001);
        }
        [TestMethod]
        public void TestDistanceIncrease2()
        {
            var dimension     = 5;
            var entropySketch = new EntropySketchFunction(dimension);
            var befVector     = ArrayUtils.Init(-1.0, -2.0, -3.0, -4.0, -5.0).ToVector();
            var afVector = ArrayUtils.Init(-3.0, -3.0, -3.0, -4.0, -5.0).ToVector();
            var befEntropySketchValue = entropySketch.ComputeEntropySketch(befVector);
            var afEntropySketchValue  = entropySketch.ComputeEntropySketch(afVector);
            var closestVector =
                IncreaseEntropySketch.l1IncreaseEntropySketchTo(befVector, dimension, afEntropySketchValue,
                                                                Approximations.ApproximationEpsilon);
            var resultMSE = closestVector.DistL2FromVector()(afVector);
            Assert.AreEqual(resultMSE, 0.0, 0.00001);
        }

        [TestMethod]
        public void TestDistanceDecrease1()
        {
            var dimension             = 4;
            var entropySketch         = new EntropySketchFunction(dimension);
            var befVector             = ArrayUtils.Init(2.8, 9.9, 11.3, -3.4).ToVector();
            var afVector              = ArrayUtils.Init(2.8, 9.9, 23.4, -3.4).ToVector();
            var befEntropySketchValue = entropySketch.ComputeEntropySketch(befVector);
            var afEntropySketchValue  = entropySketch.ComputeEntropySketch(afVector);
            var distL1                = (afVector - befVector).L1Norm();
            var distance =
                DecreaseEntropySketch.l1DecreaseEntropySketchTo(befVector, dimension, afEntropySketchValue,
                                                                Approximations.ApproximationEpsilon);
            var resultMSE = Math.Pow(distance - distL1, 2);
            Assert.AreEqual(resultMSE, 0.0, 0.00001);
        }

        [TestMethod]
        public void TestDistanceDecrease2()
        {
            var dimension             = 5;
            var entropySketch         = new EntropySketchFunction(dimension);
            var befVector             = ArrayUtils.Init(-1.0, -2.0, -3.0, -4.0, -5.0).ToVector();
            var afVector              = ArrayUtils.Init(-0.5, -2.0, -3.0, -4.0, -5.0).ToVector();
            var befEntropySketchValue = entropySketch.ComputeEntropySketch(befVector);
            var afEntropySketchValue  = entropySketch.ComputeEntropySketch(afVector);
            var distL1                = (afVector - befVector).L1Norm();
            var distance =
                DecreaseEntropySketch.l1DecreaseEntropySketchTo(befVector, dimension, afEntropySketchValue,
                                                                Approximations.ApproximationEpsilon);
            var resultMSE = Math.Pow(distance - distL1, 2);
            Assert.AreEqual(resultMSE, 0.0, 0.00001);
        }
    }
}
