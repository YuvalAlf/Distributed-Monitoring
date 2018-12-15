using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;

namespace Utils.SparseTypes
{
    public sealed class Vector
    {
        public Dictionary<int, double> IndexedValues { get; } = new Dictionary<int, double>();

        public Vector Clone()
        {
            var vector = new Vector();
            foreach (var keyValuePair in IndexedValues)
                vector[keyValuePair.Key] = keyValuePair.Value;
            return vector;
        }

        public int CountNonZero() => IndexedValues.Count;

        public Vector Select(Func<double, double> mapFunction)
        {
            var newVector = new Vector();
            foreach (var keyValuePair in this.IndexedValues)
                newVector[keyValuePair.Key] = mapFunction(keyValuePair.Value);
            return newVector;
        }

        public Vector Select(Func<int, double, double> mapFunction)
        {
            var newVector = new Vector();
            foreach (var keyValuePair in this.IndexedValues)
                newVector[keyValuePair.Key] = mapFunction(keyValuePair.Key, keyValuePair.Value);
            return newVector;
        }

        public (Vector, Vector) Halve(int indexToHalveAt)
        {
            var vector1 = new Vector();
            var vector2 = new Vector();
            foreach (var keyValuePair in IndexedValues)
                if (keyValuePair.Key < indexToHalveAt)
                    vector1[keyValuePair.Key] = keyValuePair.Value;
                else
                    vector2[keyValuePair.Key] = keyValuePair.Value;
            return (vector1, vector2);
        }

        public Vector MulBy(double mulBy)
        {
            var newVector = new Vector();
            foreach (var keyValuePair in this.IndexedValues)
                newVector[keyValuePair.Key] = keyValuePair.Value * mulBy;

            return newVector;
        }

        public double InnerProduct(Vector other)
        {
            var sumValue = 0.0;
            foreach (var index in this.IndexedValues.Keys.Intersect(other.IndexedValues.Keys))
                sumValue += this[index] + other[index];
            return sumValue;
        }

        public Vector Add(Vector other)
        {
            Vector sumVector = new Vector();
            foreach (var index in this.IndexedValues.Keys.Union(other.IndexedValues.Keys).Distinct())
                sumVector[index] = this[index] + other[index];
            return sumVector;
        }

        public Vector Subtruct(Vector other)
        {
            Vector sumVector = new Vector();
            foreach (var index in this.IndexedValues.Keys.Union(other.IndexedValues.Keys).Distinct())
                sumVector[index] = this[index] - other[index];
            return sumVector;
        }

        public double L1Norm() => this.IndexedValues.Values.Sum(x => Math.Abs(x));
        public double L2Norm() => Math.Sqrt(this.IndexedValues.Values.Sum(x => x * x));

        public double Norm(int norm)
        {
            switch (norm)
            {
                case 1:
                    return this.L1Norm();
                case 2:
                    return this.L2Norm();
                default:
                    throw new NotImplementedException();
            }
        }


        public void AddInPlace(Vector other)
        {
            foreach (var index in other.IndexedValues.Keys)
                this[index] += other[index];
        }

        public void DivideInPlace(double divideBy)
        {
            foreach (var index in IndexedValues.Keys)
                IndexedValues[index] /= divideBy;
        }
        public Vector Divide(double divideBy)
        {
            var newVector = new Vector();
            foreach (var keyValuePair in this.IndexedValues)
                newVector[keyValuePair.Key] = keyValuePair.Value / divideBy;

            return newVector;
        }


        public double this[int index]
        {
            get
            {
                if (IndexedValues.TryGetValue(index, out double value))
                    return value;
                return 0;
            }
            set
            {
                if (value != 0.0)
                    IndexedValues[index] = value;
            } 
        }


        public static Vector SumVector(Vector[] vectors)
        {
            var sumVector = new Vector();
            vectors.ForEach(sumVector.AddInPlace);
            return sumVector;
        }

        public static Vector AverageVector(Vector[] vectors)
        {
            var sumVector = SumVector(vectors);
            sumVector.DivideInPlace(vectors.Length);
            return sumVector;
        }

        public void CopyTo(Vector to)
        {
            this.IndexedValues.Clear();
            foreach (var keyValuePair in to.IndexedValues)
                this[keyValuePair.Key] = keyValuePair.Value;
        }

        public double[] ToArray(int dimension)
        {
            var array = new double[dimension];
            foreach (var keyValuePair in this.IndexedValues)
                array[keyValuePair.Key] = keyValuePair.Value;
            return array;
        }

        public int MinimumIndex()
        {
            return this.IndexedValues.MinBy(pair => pair.Value).First().Key;
        }

        public int MaximumIndex()
        {
            return this.IndexedValues.MaxBy(pair => pair.Value).First().Key;
        }

        public Vector Concat(Vector other, int startingDimension)
        {
            var vector = this.Clone();
            foreach (var otherIndexedValue in other.IndexedValues)
                vector[startingDimension + otherIndexedValue.Key] = otherIndexedValue.Value;

            return vector;
        }

        public Func<Vector, double> DistL2FromVector() => v => this.Subtruct(v).L2Norm();
    }
}
