namespace EntropyMathematics

open System
open Utils.SparseTypes

type index = int

type DataDeque(dataPoints : (double * Set<index>) Deque) =
    let getOccurencess (value : double, indices : Set<index>) = (value, double(indices.Count))
    member deque.MinValuePair()   = dataPoints |> Deque.head |> getOccurencess
    member deque.MaxValuePair()   = dataPoints |> Deque.last |> getOccurencess
    member deque.SecondMinValue() = dataPoints |> Deque.tail |> Deque.head |> fst
    member deque.SecondMaxValue() = dataPoints |> Deque.init |> Deque.last |> fst
    member deque.IsSingle() = dataPoints |> Deque.isSingle

    member deque.CalcEntropy (entropyOfValue : Func<double, double>) =
        dataPoints
        |> Deque.toSeq
        |> Seq.sumBy (fun (value, indices) -> double(indices.Count) * entropyOfValue.Invoke(value))

    member deque.CombineMin () =
        let (minValue, minIndices)             = dataPoints.Head
        let (secondMinValue, secondMinIndices) = dataPoints.Tail.Head
        dataPoints
        |> Deque.tail
        |> Deque.replaceMin (secondMinValue, Set.union minIndices secondMinIndices)
        |> DataDeque
    member deque.CombineMax () =
        let (maxValue, maxIndices)             = dataPoints.Last
        let (secondMaxValue, secondMaxIndices) = dataPoints.Init.Last
        dataPoints
        |> Deque.init
        |> Deque.replaceMax (secondMaxValue, Set.union maxIndices secondMaxIndices)
        |> DataDeque
    member deque.ChangeMinValue(newMinValue) =
        if abs(deque.SecondMinValue() - newMinValue) < 0.000000001 then
            deque.CombineMin()
        else
            dataPoints 
            |> Deque.replaceMin (newMinValue, snd dataPoints.Head)
            |> DataDeque
    member deque.ChangeMaxValue(newMaxValue) =
        if abs(deque.SecondMaxValue() - newMaxValue) < 0.000000001 then
            deque.CombineMax()
        else
            dataPoints 
            |> Deque.replaceMax (newMaxValue, snd dataPoints.Last)
            |> DataDeque

    member deque.ToVector () =
        let vector = new Vector ()
        dataPoints
        |> Deque.toSeq
        |> Seq.iter (fun (value, indices) ->
                        indices
                        |> Set.iter (fun index -> vector.[index] <- value))
        vector
        
    static member OfVector (dimension : int, vector : Vector) : DataDeque =
        vector.Enumerate (dimension)
        |> Seq.indexed
        |> Seq.groupBy (fun (i, value) -> value)
        |> Seq.map (fun (value, indicesValues) -> (value, indicesValues |> Seq.map fst |> Set))
        |> Seq.toArray
        |> Array.sortBy fst
        |> Deque.createOfArray
        |> DataDeque

    member deque.IncreaseEntropy (averageEntropy : double, l1Distance : double) : DataDeque =
        if deque.IsSingle() then
            deque
        else
            let (minValue, minAmount) = deque.MinValuePair()
            let (maxValue, maxAmount) = deque.MaxValuePair()
            let secondMinValue = min averageEntropy (deque.SecondMinValue())
            let secondMaxValue = max averageEntropy (deque.SecondMaxValue())
            let minDiff = secondMinValue - minValue
            let maxDiff = maxValue - secondMaxValue
            let minL1Distance = minDiff * minAmount
            let maxL1Distance = maxDiff * maxAmount
            let applicableL1Distance = min minL1Distance maxL1Distance
            let twiceApplicableL1Distance = 2.0 * applicableL1Distance
            if l1Distance < twiceApplicableL1Distance then
                let minChange = l1Distance * maxAmount / (minAmount + maxAmount)
                let maxChange = l1Distance * minAmount / (minAmount + maxAmount)
                deque.ChangeMinValue(minValue + minChange).ChangeMaxValue(maxValue - maxChange)
            elif minL1Distance < maxL1Distance then
                let maxChange = minL1Distance / maxAmount
                let minOperation = if secondMinValue = averageEntropy then deque.ChangeMinValue averageEntropy else deque.CombineMin()
                minOperation.ChangeMaxValue(maxValue - maxChange).IncreaseEntropy(averageEntropy, l1Distance - twiceApplicableL1Distance)
            elif minL1Distance > maxL1Distance then
                let minChange = maxL1Distance / minAmount
                let maxOperation = if secondMaxValue = averageEntropy then deque.ChangeMaxValue averageEntropy else deque.CombineMax()
                maxOperation.ChangeMinValue(minValue + minChange).IncreaseEntropy(averageEntropy, l1Distance - twiceApplicableL1Distance)
            else
                deque.CombineMin().CombineMax().IncreaseEntropy(averageEntropy, l1Distance - twiceApplicableL1Distance)