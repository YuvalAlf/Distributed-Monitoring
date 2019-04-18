namespace EntropyMathematics

open System
open Utils.SparseTypes

module Entropy =

    let rec binaryStepsIncreaseEntropy (epsilon : double,
                                        desiredEntropy : double,
                                        step : double,
                                        dataVector : DataVector) : DataVector =
        if dataVector.IncreaseEntropy(epsilon * 2.0).Entropy > desiredEntropy then
            dataVector
        else
            let increasedDataVector = dataVector.IncreaseEntropy(step)
            if increasedDataVector.Entropy > desiredEntropy then
                binaryStepsIncreaseEntropy(epsilon, desiredEntropy, epsilon, dataVector)
            else
                binaryStepsIncreaseEntropy(epsilon, desiredEntropy, step * 2.0, increasedDataVector)
            

    let public l1IncreaseEntropyTo (point : Vector,
                                    dimension : int,
                                    desiredEntropy : double,
                                    lowerBoundEntropy : Func<double, double>,
                                    epsilon : double) : Vector =
        let dataVector = DataVector.OfVector(dimension, lowerBoundEntropy, point)
        let resultDataVector = binaryStepsIncreaseEntropy(epsilon, desiredEntropy, epsilon, dataVector)
        resultDataVector.ToVector()