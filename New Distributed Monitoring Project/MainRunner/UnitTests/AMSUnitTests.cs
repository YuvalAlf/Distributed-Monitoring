using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecondMomentSketch;
using SecondMomentSketch.Hashing;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class AMSUnitTests
    {
        private MersenneTwister rnd = new MersenneTwister(999);

        private double[] GenArrayOfFrequencies(int arraySize)
        {
            return ArrayUtils.Init(arraySize, _ => rnd.NextDouble() * 100);
        }


        [TestMethod]
        public void TestAMS()
        {
            var width = 101;
            var height = 13;
            var vectorLength = width * height;
            var array = GenArrayOfFrequencies(1000);
            var ams = new SecondMoment(width, height);
            var realF2 = array.Sum(f => f * f);
            var fourwise = FourwiseIndepandantFunction.Init(rnd);
            var hashFunction       = FourwiseIndepandantFunction.Init(rnd);
            var hashFunctionsTable = HashFunctionTable.Init(1, vectorLength, hashFunction);
            var resultAmsVector = fourwise.TransformToAMSSketch(ArrayUtils.Init(array.ToVector()), vectorLength, hashFunctionsTable);
            var amsF2 = ams.Compute(resultAmsVector[0]);
            var ratio = amsF2 / realF2;
            Assert.AreEqual(ams, amsF2);
        }
    }
}
