namespace EntropyMathematics

open System
open Utils.SparseTypes
open BinarySearch

module DecreaseEntropy =

    let vectorToDeque (vector : Vector, dimension : int, evalEntropy : double -> double) : Deque<int * double> =
        vector.Enumerate(dimension)
        |> Array.ofSeq
        |> Array.indexed
        |> Array.sortBy snd
        |> RangeQuery.create (snd >> evalEntropy) 
        |> Deque.createOfArray

    let decreaseEntropy(deque : Deque<int * double>, l1Distance : double) : Deque<int * double> =
        let halfDistance = l1Distance / 2.0
        let changeValue (i, value) change = (i, value + change)
        deque
        |> Deque.replaceMin (changeValue deque.Head (-halfDistance))
        |> Deque.replaceMax (changeValue deque.Last (+halfDistance))

    let dequeToVector (deque : Deque<int * double>) : Vector =
        let resultVector = new Vector()

        deque 
        |> Deque.toSeq 
        |> Seq.map snd 
        |> Seq.iteri (fun i value -> resultVector.[i] <- value)

        resultVector
        

    let public l1DecreaseEntropy (point : Vector,
                                  dimension : int,
                                  desiredEntropy : double,
                                  xLinePivot : double,
                                  epsilon : double) =
        let entropyFunc = Entropy.genEntropyLowerBoundFunc xLinePivot
        let dequeData = vectorToDeque (point, dimension, entropyFunc)
        let pointOk (data : Deque<_>) = data.FunctionValue >= desiredEntropy

        let resultData = binarySteps (epsilon, epsilon, dequeData, decreaseEntropy, pointOk)
        
        resultData |> dequeToVector

