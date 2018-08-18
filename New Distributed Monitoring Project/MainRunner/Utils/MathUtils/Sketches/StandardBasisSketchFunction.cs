﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Utils.MathUtils.Sketches
{
    public sealed class StandardBasisSketchFunction : SketchFunction
    {
        public override (Vector<double> sketch, Vector<double> epsilon) Sketch(
            Vector<double> vector, int dimension, StrongBox<int> startIndex)
        {
            var sketch = Enumerable.Repeat(0.0, vector.Count).ToVector();
            for (int i = 0; i < dimension; i++)
            {
                sketch[startIndex.Value] = vector[startIndex.Value];
                startIndex.Value         = (startIndex.Value + 1) % vector.Count;
            }

            var epsilon = vector - sketch;
            return (sketch, epsilon);
        }
    }
}