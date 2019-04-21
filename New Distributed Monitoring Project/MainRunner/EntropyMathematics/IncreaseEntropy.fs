namespace EntropyMathematics

open System
open Utils.SparseTypes
open BinarySearch

module IncreaseEntropy =

    let public l1IncreaseEntropyTo (point : Vector,
                                    dimension : int,
                                    desiredEntropy : double,
                                    xLinePivot : double,
                                    epsilon : double) : Vector =
        let increaseEntropy (dataVector : DataVector, step) = dataVector.IncreaseEntropy step
        let pointOk (dataVector : DataVector) = dataVector.FunctionValue <= desiredEntropy
        let entropyFunc = Entropy.genEntropyLowerBoundFunc xLinePivot
        let calcEntropy (value, indices : FastCountSet<int>) = entropyFunc(value) * double(indices.Count)
        let dataVector = DataVector.OfVector(dimension, calcEntropy, point)
        let resultDataVector = binarySteps(epsilon, epsilon, dataVector, increaseEntropy, pointOk)
        resultDataVector.ToVector()