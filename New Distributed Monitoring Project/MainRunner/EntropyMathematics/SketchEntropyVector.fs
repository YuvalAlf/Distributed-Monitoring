namespace EntropyMathematics

open Utils.SparseTypes
open System

type SketchEntropyVector(dataDeque : DataDeque, dimension : double) =
    member d.FunctionValue = Math.Log(dimension) - Math.Log(dataDeque.FunctionValue)

    member d.ToVector () : Vector =
        dataDeque.ToVector ()
    
    member d.IncreaseEntropySketchValue l1Distance =
        SketchEntropyVector(dataDeque.IncreaseEntropySketchValue l1Distance, dimension)

    static member OfVector (dimension : int, vector : Vector) : SketchEntropyVector =
        let calcExp (value : double, count : FastCountSet<_>) = Math.Exp(value) * double(count.Count)
        let deque = DataDeque.OfVector(dimension, calcExp, vector)
        SketchEntropyVector (deque, double(dimension))

