using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;

namespace ClassLibrary1
{
    public static class EigenvaluesExtensions
    {
        public static (Vector<double> Eigenvector, double Eigenvalue) PowerIterationMethod(this Matrix<double> @this, double epsilon, Random rnd)
        {

            for (int i = 0; i < @this.ColumnCount; i++)
            {
                for (int j = 0; j < @this.ColumnCount; j++)
                    Console.Write(@this[i, j] + " ");
                Console.WriteLine();
            }

            Console.WriteLine();
            var eigens = @this.Evd().EigenValues.Select(e => e.Real).OrderByDescending(x => x).ToArray();


            var size        = @this.ColumnCount;
            var rndVec      = Vector<double>.Build.Random(size).Normalize(2);
            var eigenvector = rndVec;
            var change      = 0.0;
            do
            {
                var lastEigenvector = eigenvector;
                eigenvector = (@this * eigenvector).Normalize(2);
                change      = (eigenvector - lastEigenvector).L1Norm();
            } while (change > epsilon * size);

            var eigenvalue = eigenvector * @this * eigenvector;
            return (eigenvector, eigenvalue);
        }

        public static (Vector<double> Eigenvector, double Eigenvalue) PowerIterationMethod2(this Matrix<double> @this, double epsilon, Vector<double> orthogonalVector, Random rnd)
        {
            var maxEigenvalue  = @this.PowerIterationMethod(epsilon, rnd).Eigenvalue;

            @this = @this.Clone();
            for (int i = 0; i < @this.ColumnCount; i++)
                @this[i, i] += maxEigenvalue;
            var newEigens = @this.Evd().EigenValues.Select(e => e.Real).OrderByDescending(x => x).ToArray();

            var size        = @this.ColumnCount;
            var rndVec      = Vector<double>.Build.Random(size).Normalize(2);
            var eigenvector = rndVec;
            var change      = 0.0;
            do
            {
                var lastEigenvector = eigenvector;
                eigenvector = @this * eigenvector;
                var angle = (eigenvector * orthogonalVector);
                eigenvector -= angle * orthogonalVector;
                eigenvector =  eigenvector.Normalize(2);
                var expected = newEigens.Skip(1).First();
                var eigen = eigenvector * @this * eigenvector;
                change      =  (eigenvector - lastEigenvector).L1Norm();
            } while (change > epsilon);

            var eigenvalue = eigenvector * @this * eigenvector;
            return (eigenvector, eigenvalue - maxEigenvalue);
        }
    }
}
