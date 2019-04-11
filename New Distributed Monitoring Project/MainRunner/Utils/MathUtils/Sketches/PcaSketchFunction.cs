using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Accord.Statistics.Models.Regression.Linear;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.TypeUtils;

namespace Utils.MathUtils.Sketches
{
   /* [Serializable]
    public sealed class PcaSketchFunction : SketchFunction
    {
        private MultivariateLinearRegression Pca { get; }
        private MultivariateLinearRegression InversePca { get; }

        public PcaSketchFunction(MultivariateLinearRegression pca, MultivariateLinearRegression inversePca)
        {
            Pca = pca;
            InversePca = inversePca;
        }

        public override (Vector<double> sketch, Vector<double> epsilon, InvokedIndices indices) Sketch(Vector<double> vector, int dimension)
        {
            var indices = new HashSet<int>();
            var pcaVector = Pca.Transform(vector.ToArray());
            var sketchArray = new double[vector.Count];
            var sketchData = pcaVector.Index().PartialSortBy(dimension / 2, pair => -Math.Abs(pair.Value));
            foreach (var indexValuePair in sketchData)
            {
                indices.Add(indexValuePair.Key);
                sketchArray[indexValuePair.Key] = indexValuePair.Value;
            }

            var sketchVector = InversePca.Transform(sketchArray).ToVector();
            var epsilon = vector - sketchVector;
            var mse = epsilon.Sum(x => x * x);
            return (sketchVector, epsilon, new InvokedIndices(indices));
        }
    }*/
}
