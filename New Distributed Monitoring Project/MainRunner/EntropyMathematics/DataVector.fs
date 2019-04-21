namespace EntropyMathematics

open Utils.SparseTypes

type DataVector(dataDeque : DataDeque, averageEntropy : double) =
    member d.FunctionValue = dataDeque.FunctionValue

    member d.IncreaseEntropy (l1Distance : double) : DataVector =
        let newDataDeque = dataDeque.IncreaseEntropy(averageEntropy, l1Distance)
        DataVector (newDataDeque, averageEntropy)

    member d.ToVector () : Vector =
        dataDeque.ToVector ()

    static member OfVector (dimension : int, calcEntropy, vector : Vector) : DataVector =
        let deque = DataDeque.OfVector(dimension, calcEntropy, vector)
        DataVector (deque, 1.0 / double(dimension))