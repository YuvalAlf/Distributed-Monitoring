namespace EntropyMathematics

open System
open Utils.SparseTypes

type DataVector(dataDeque : DataDeque, calcEntropy : Func<double, double>, averageEntropy : double) =
    let entropy = lazy(dataDeque.CalcEntropy calcEntropy)
    member d.Entropy = entropy.Value

    member d.IncreaseEntropy (l1Distance : double) : DataVector =
        let newDataDeque = dataDeque.IncreaseEntropy(averageEntropy, l1Distance)
        DataVector (newDataDeque, calcEntropy, averageEntropy)

    member d.ToVector () : Vector =
        dataDeque.ToVector ()

    static member OfVector (dimension : int, calcEntropy, vector : Vector) : DataVector =
        let deque = DataDeque.OfVector(dimension, vector)
        DataVector (deque, calcEntropy, 1.0 / double(dimension))