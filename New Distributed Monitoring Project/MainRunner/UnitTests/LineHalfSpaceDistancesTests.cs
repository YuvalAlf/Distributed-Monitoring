using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace UnitTests
{
    [TestClass]
    public class LineHalfSpaceDistancesTests
    {
        private static Vector<double> CreateVec(params double[] values) => values.ToVector();

        [TestMethod]
        public void DistanceOutsideTest1()
        {
            var a = 1;
            var b = 4;
            var constantPart = 2;
            var threshold = 12;
            var halfPlane = LineHalfPlane.Create(CreateVec(a, b), constantPart, threshold);
            var closestL1 = halfPlane.ClosestPointL1(CreateVec(0, 0));
            var closestL2 = halfPlane.ClosestPointL2(CreateVec(0, 0));
            
        }


    }
}
