namespace EntropyMathematics

module Entropy =
    let genEntropyLowerBoundFunc (xLinePivot : double) =
        let calcEntropy x = - x * log(x) 
        let yLinePivot = calcEntropy xLinePivot
        let m = yLinePivot / xLinePivot
        fun x -> if x < xLinePivot then m * x else calcEntropy x 
        

