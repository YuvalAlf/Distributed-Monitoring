namespace EntropyMathematics

open Utils.SparseTypes

module IncreaseEntropySketch =
    open System

    let public l1IncreaseEntropySketchTo (point : Vector,
                                          dimension : int,
                                          desiredSketchEntropy : double,
                                          epsilon : double)  : Vector =
        let move (data : SketchEntropyVector, l1Distance : double) = data.IncreaseEntropySketchValue l1Distance
        let pointOk (data : SketchEntropyVector) = data.FunctionValue <= desiredSketchEntropy
        let vector = SketchEntropyVector.OfVector (dimension, point)
        BinarySearch.binarySteps(epsilon, vector, move, pointOk).ToVector()
