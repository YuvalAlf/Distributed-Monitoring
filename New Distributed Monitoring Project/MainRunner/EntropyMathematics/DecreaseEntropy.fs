namespace EntropyMathematics

open System
open Utils.SparseTypes
open BinarySearch

module DecreaseEntropy =
    type index = int

    let vectorToDeque (vector : Vector, dimension : int, evalEntropy : double -> double) : Deque<index * double> =
        vector.Enumerate(dimension)
        |> Array.ofSeq
        |> Array.indexed
        |> Array.sortBy snd
        |> RangeQuery.create (snd >> evalEntropy) 
        |> Deque.createOfRangeQueryArray

    let decreaseEntropy(deque : Deque<index * double>, l1Distance : double) : Deque<index * double> =
        let halfDistance = l1Distance / 2.0
        let changeValue (index, value) change = (index, value + change)
        deque
        |> Deque.replaceMin (changeValue deque.Head (-halfDistance))
        |> Deque.replaceMax (changeValue deque.Last (+halfDistance))

    let dequeToVector (deque : Deque<index * double>) : Vector =
        let resultVector = new Vector()
        deque 
        |> Deque.toSeq
        |> Seq.iter (fun (i, value) -> resultVector.[i] <- value)

        resultVector
        

    let public l1DecreaseEntropy (point : Vector,
                                  dimension : int,
                                  desiredEntropy : double,
                                  xLinePivot : double,
                                  epsilon : double) =
        let entropyFunc = Entropy.genEntropyLowerBoundFunc xLinePivot
        let dequeData = vectorToDeque (point, dimension, entropyFunc)
        let pointOk (data : Deque<index * double>) = data.FunctionValue >= desiredEntropy

        let resultData = binarySteps (epsilon, dequeData, decreaseEntropy, pointOk)
        DataDeque.AssertRunTime(pointOk(resultData))
        let vector = resultData |> dequeToVector
        DataDeque.AssertRunTime (abs(vector.Sum() - 1.0) <= 0.0001)
        vector

