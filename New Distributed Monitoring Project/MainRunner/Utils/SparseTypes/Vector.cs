using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;

namespace Utils.SparseTypes
{
    public sealed partial class Vector
    {
        #region Basic Operations

        public Dictionary<int, double> IndexedValues { get; } = new Dictionary<int, double>();

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
                if (value == 0.0)
                    IndexedValues.Remove(index);
                else
                    IndexedValues[index] = value;
            }
        }

        public int CountNonZero() => IndexedValues.Count;

        public Vector Clone()
        {
            var vector = new Vector();
            foreach (var keyValuePair in IndexedValues)
                vector.IndexedValues.Add(keyValuePair.Key, keyValuePair.Value);
            return vector;
        }
        
        public void CopyTo(Vector to)
        {
            to.IndexedValues.Clear();
            foreach (var keyValuePair in this.IndexedValues)
                to.IndexedValues.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public double[] ToArray(int dimension)
        {
            var array = new double[dimension];
            foreach (var keyValuePair in this.IndexedValues)
                array[keyValuePair.Key] = keyValuePair.Value;
            return array;
        }

        #endregion

        #region Enumerable

        public Vector Map(int dimension, Func<double, double> mapFunction)
        {
            var newVector = new Vector();
            var mapZero = mapFunction(0.0);
            for (int i = 0; i < dimension; i++)
            {
                var mappedValueToAdd = this.IndexedValues.ContainsKey(i) ? mapFunction(this[i]) : mapZero;
                newVector.IndexedValues.Add(i, mappedValueToAdd);
            }
            return newVector;
        }
        
        public (Vector, Vector) Halve(int indexToHalveAt)
        {
            var vector1 = new Vector();
            var vector2 = new Vector();
            foreach (var keyValuePair in IndexedValues)
                if (keyValuePair.Key < indexToHalveAt)
                    vector1.IndexedValues.Add(keyValuePair.Key, keyValuePair.Value);
                else
                    vector2.IndexedValues.Add(keyValuePair.Key - indexToHalveAt, keyValuePair.Value);
            return (vector1, vector2);
        }
        
        public Vector Concat(Vector other, int startingIndex)
        {
            var vector = this.Clone();
            foreach (var otherIndexedValue in other.IndexedValues)
                vector.IndexedValues.Add(startingIndex + otherIndexedValue.Key, otherIndexedValue.Value);
            return vector;
        }

        #endregion

        #region Norm

        public double L1Norm() => this.IndexedValues.Values.Sum(x => Math.Abs(x));

        public double L2Norm() => Math.Sqrt(this.IndexedValues.Values.Sum(x => x * x));

        public Func<Vector, double> DistL2FromVector() => v => this.Subtruct(v).L2Norm();

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

        #endregion

        #region Mathematical Operations

        #region Sumation

        private Vector Add(Vector other)
        {
            Vector sumVector = this.Clone();
            foreach (var keyValuePair in other.IndexedValues)
                sumVector[keyValuePair.Key] += keyValuePair.Value;
            return sumVector;
        }

        public static Vector operator+ (Vector v1, Vector v2) => v1.Add(v2);

        public void AddInPlace(Vector other)
        {
            foreach (var index in other.IndexedValues.Keys)
                this[index] += other[index];
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

        #endregion

        #region Subtruction

        private Vector Subtruct(Vector other)
        {
            Vector sumVector = this.Clone();
            foreach (var keyValuePair in other.IndexedValues)
                sumVector[keyValuePair.Key] -= keyValuePair.Value;
            return sumVector;
        }

        public static Vector operator -(Vector v1, Vector v2) => v1.Subtruct(v2);

        public static Vector operator -(Vector v) => (-1) * v;


        #endregion

        #region Multiply

        private Vector MulBy(double mulBy)
        {
            var newVector = new Vector();
            foreach (var keyValuePair in this.IndexedValues)
                newVector.IndexedValues[keyValuePair.Key] = keyValuePair.Value * mulBy;
            return newVector;
        }

        private double InnerProduct(Vector other)
        {
            var sumValue = 0.0;
            foreach (var index in this.IndexedValues.Keys.Intersect(other.IndexedValues.Keys))
                sumValue += this.IndexedValues[index] * other.IndexedValues[index];
            return sumValue;
        }

        public static Vector operator *(Vector v, double num) => v.MulBy(num);

        public static Vector operator *(double num, Vector v) => v.MulBy(num);

        public static double operator *(Vector v1, Vector v2) => v1.InnerProduct(v2);

        #endregion

        #region Division

        public void DivideInPlace(double divideBy)
        {
            foreach (var index in IndexedValues.Keys.ToArray())
                IndexedValues[index] /= divideBy;
        }

        private Vector Divide(double divideBy)
        {
            var newVector = new Vector();
            foreach (var keyValuePair in this.IndexedValues)
                newVector.IndexedValues.Add(keyValuePair.Key, keyValuePair.Value / divideBy);
            return newVector;
        }

        public static Vector operator /(Vector v, double divBy) => v.Divide(divBy);

        #endregion

        #region Data Extraction

        public int MinimumIndex()
        {
            return this.IndexedValues.MinBy(pair => pair.Value).First().Key;
        }

        public int MaximumIndex()
        {
            return this.IndexedValues.MaxBy(pair => pair.Value).First().Key;
        }

        public double Sum()
        {
            return this.IndexedValues.Values.Sum();
        }

        #endregion

        #endregion

    }
}
