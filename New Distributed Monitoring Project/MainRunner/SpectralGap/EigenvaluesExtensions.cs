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
        public static Matrix<double> AddDiagonal(this Matrix<double> @this, double amount)
        {
            var matrix = @this.Clone();
            for (int i = 0; i < matrix.ColumnCount; i++)
                matrix[i, i] += amount;
            return matrix;
        }

        public static (Vector<double> Eigenvector, double Eigenvalue) PowerIterationMethod(this Matrix<double> @this, double epsilon, Random rnd)
        {
            var size        = @this.ColumnCount;
            var operatedMatrix = @this.AddDiagonal(size);
            var rndVec      = Vector<double>.Build.Random(size).Normalize(2);
            var eigenvector = rndVec;
            var change      = 0.0;
            
            do
            {
                var lastEigenvector = eigenvector;
                eigenvector = (operatedMatrix * eigenvector).Normalize(2);
                change      = (eigenvector - lastEigenvector).L1Norm();
            } while (change > epsilon * size);

            var eigenvalue = eigenvector * (operatedMatrix * eigenvector);
            return (eigenvector, eigenvalue - size);
        }
        public static (Vector<double> Eigenvector, double Eigenvalue) PowerIterationMethod(this Matrix<double> @this, double epsilon, Random rnd, Vector<double> orthogonalVector)
        {
            var size        = @this.ColumnCount;
            var operatedMatrix = @this.AddDiagonal(size);
            var rndVec      = Vector<double>.Build.Random(size).Normalize(2);
            var eigenvector = rndVec;
            var change      = 0.0;

            Vector<double> MatrixMul(Vector<double> vec)
            {
                var angle = orthogonalVector * vec;
                return operatedMatrix * vec - angle * orthogonalVector;
            }

            do
            {
                var lastEigenvector = eigenvector;
                eigenvector = MatrixMul(eigenvector).Normalize(2);
                change      = (eigenvector - lastEigenvector).L1Norm();
            } while (change > epsilon * size);

            var eigenvalue = eigenvector * MatrixMul(eigenvector);
            return (eigenvector, eigenvalue - size);
        }


        public static double SecondLargestEigenvalue(this Matrix<double> @this, Vector<double> orthogonalVector, double eigenvalue1, double epsilon, Random rnd)
        {
            var mulVector = Math.Sqrt(Math.Abs(eigenvalue1)) * orthogonalVector;
            var result1 = @this.PowerIterationMethod(epsilon, rnd, mulVector).Eigenvalue;
            return result1;
        }
        /*public static (Vector<double> Eigenvector, double Eigenvalue) PowerIterationMethod2(this Matrix<double> @this, double epsilon, Vector<double> orthogonalVector, Random rnd)
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
        }*/
    }
}
