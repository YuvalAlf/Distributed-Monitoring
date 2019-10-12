namespace EntropyMathematics

open Utils.SparseTypes

module DecreaseEntropySketch =
    open System

    let public l1DecreaseEntropySketchTo (point : Vector,
                                          dimension : int,
                                          desiredSketchEntropy : double,
                                          epsilon : double)  : double =
        let maxEntry = point.MaximumIndex()
        let sumOfRest = 
            seq {0 ..  dimension - 1}
            |> Seq.except [maxEntry]
            |> Seq.map (fun index -> point.[index])
            |> Seq.sumBy Math.Exp

        let expThresh = Math.Exp(-desiredSketchEntropy)

        Math.Log(double(dimension) * expThresh - sumOfRest) - point.[maxEntry]


