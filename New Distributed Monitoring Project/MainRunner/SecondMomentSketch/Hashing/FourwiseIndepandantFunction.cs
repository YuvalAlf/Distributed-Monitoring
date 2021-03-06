﻿using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.Random;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch.Hashing
{
    public sealed class FourwiseIndepandantFunction : HashGenerator
    {
        public int LargePrimeNumber { get; }

        public FourwiseIndepandantFunction(int largePrimeNumber) => LargePrimeNumber = largePrimeNumber;

        public static FourwiseIndepandantFunction Init(Random rnd)
        {
            var prime = rnd.GenLargePrime();
            return new FourwiseIndepandantFunction(prime);
        }

        public Func<int, int> GenerateHash(int nodeIndex, int amsVectorIndex)
        {
            //var rnd        = new Random((amsVectorIndex, nodeIndex).GetHashCode());
            var rnd        = new Random(amsVectorIndex);
            //var rnd        = new MersenneTwister(amsVectorIndex);
           // for (int i = 0; i < nodeIndex; i++)
            //    rnd.Next();
            //rnd = new MersenneTwister(rnd.Next());
            var randomNum0 = rnd.Next();
            var randomNum1 = rnd.Next();
            var randomNum2 = rnd.Next();
            var randomNum3 = rnd.Next();
            var randomNum4 = rnd.Next();


            int hashFunction(int itemId)
            {
                var value1 = (Int64) (itemId);
                var value2 = (value1 * value1) % LargePrimeNumber;
                var value3 = (value2 * value1) % LargePrimeNumber;
                var value4 = (value3 * value1) % LargePrimeNumber;
                var polynom = randomNum0 + randomNum1 * value1 + randomNum2 * value2 + randomNum3 * value3 + randomNum4 * value4;
                var hasedTo01 = (polynom % LargePrimeNumber) % 2;
                return ((int) hasedTo01 * 2 - 1);

            }

            return hashFunction;
        }

        public Vector[] TransformToAMSSketch(Vector[] countVectors, int vectorLength, HashFunctionTable[] hashFunctionsTables)
        {
            return countVectors.Zip(hashFunctionsTables,
                                    (countVector, hashFunctionTable) =>
                                    {
                                        var amsVector = new Vector();
                                        foreach (var valuePair in countVector.IndexedValues)
                                        {
                                            var itemId = valuePair.Key;
                                            var value  = valuePair.Value;

                                            for (int i = 0; i < vectorLength; i++)
                                                amsVector[i] += value * hashFunctionTable[i](itemId);
                                        }

                                        return amsVector;
                                    }).ToArray();
        }
    }
}
