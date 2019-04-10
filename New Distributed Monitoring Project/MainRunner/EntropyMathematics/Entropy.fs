
namespace EntropyMathematics

open System
open MathNet.Numerics.LinearAlgebra

module Entropy =
    open MathNet.Numerics

    let valueEntropy x =
        if x <= 0.0 then 0.0
        else Math.Log x
    
    let arrayEntropy = Array.sumBy valueEntropy
    let seqEntropy seq = seq |> Seq.sumBy valueEntropy
    let vecEntropy (vec : Vector<double>) = vec |> Seq.sumBy valueEntropy
        
            
    let increaseEntropy (l1Distance : double, pVector : double Vector) =
        let indexedPVector = pVector.ToArray() |> Array.indexed |> Array.sortBy snd
        let changeAtIndex i change =
            let (index, value) = indexedPVector.[i]
            indexedPVector.[i] <- (index, value + change)
        let valueAt i =
            snd indexedPVector.[i]
            
        let average = 1.0 / double(pVector.Count)
        let rec increase minIndexEnd maxIndexEnd l1Distance =
            if l1Distance <= 0.0 || minIndexEnd >= maxIndexEnd then
                ()
            elif valueAt(minIndexEnd).AlmostEqual(valueAt(minIndexEnd + 1), 0.000001) then
                increase (minIndexEnd + 1) (maxIndexEnd) (l1Distance)
            elif valueAt(maxIndexEnd).AlmostEqual(valueAt(maxIndexEnd - 1), 0.000001) then
                increase (minIndexEnd) (maxIndexEnd - 1) (l1Distance)
            else
                let minVal = valueAt minIndexEnd
                let maxVal = valueAt maxIndexEnd
                let secondMinVal = min average (valueAt (minIndexEnd + 1))
                let secondMaxVal = max average (valueAt (maxIndexEnd - 1))
                if Precision.AlmostEqualRelative(minVal, average, 0.00000001) && Precision.AlmostEqualRelative(maxVal, average, 0.00000001) then
                    ()
                else
                    let minDiff = secondMinVal - minVal
                    let maxDiff = maxVal - secondMaxVal
                    let amountOfMin = double(1 + minIndexEnd)
                    let amountOfMax = double(indexedPVector.Length - maxIndexEnd)
                    let sumOfChanges = (min amountOfMin amountOfMax) * (min minDiff maxDiff)
                    let l1Change = min l1Distance sumOfChanges
                    for i = 0 to minIndexEnd do
                        changeAtIndex i (0.5 * l1Change / amountOfMin)
                    for i = indexedPVector.Length - 1 downto maxIndexEnd do
                        changeAtIndex i -(0.5 * l1Change / amountOfMax)
                    increase minIndexEnd maxIndexEnd (l1Distance - l1Change)



        increase 0 (indexedPVector.Length - 1) l1Distance
        for (i, newValue) in indexedPVector do
            pVector.[i] <- newValue
        pVector