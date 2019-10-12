namespace EntropyMathematics

open Utils.SparseTypes

type index = int

type DataDeque(dataPoints : (double * FastCountSet<index>) Deque) =
    let getOccurencess (value : double, indices : FastCountSet<index>) = (value, double(indices.Count))
    member deque.MinValuePair   = dataPoints |> Deque.head |> getOccurencess
    member deque.MaxValuePair   = dataPoints |> Deque.last |> getOccurencess
    member deque.SecondMinValue = dataPoints |> Deque.tail |> Deque.head |> fst
    member deque.SecondMaxValue = dataPoints |> Deque.init |> Deque.last |> fst
    member deque.IsSingle       = dataPoints |> Deque.isSingle
    member deque.FunctionValue =  dataPoints.FunctionValue

    member deque.CombineMin () =
        let (minValue, minIndices)             = dataPoints.Head
        let (secondMinValue, secondMinIndices) = dataPoints.Tail.Head
        dataPoints
        |> Deque.tail
        |> Deque.replaceMin (secondMinValue, FastCountSet.Union minIndices secondMinIndices)
        |> DataDeque
    member deque.CombineMax () =
        let (maxValue, maxIndices)             = dataPoints.Last
        let (secondMaxValue, secondMaxIndices) = dataPoints.Init.Last
        dataPoints
        |> Deque.init
        |> Deque.replaceMax (secondMaxValue, FastCountSet.Union maxIndices secondMaxIndices)
        |> DataDeque
    member deque.ChangeMinValue(newMinValue) =
        if abs(deque.SecondMinValue - newMinValue) < 0.000000001 then
            deque.CombineMin()
        else
            dataPoints 
            |> Deque.replaceMin (newMinValue, snd dataPoints.Head)
            |> DataDeque
    member deque.ChangeMaxValue(newMaxValue) =
        if abs(deque.SecondMaxValue - newMaxValue) < 0.000000001 then
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
                        indices.ItemsSet
                        |> Set.iter (fun index -> vector.[index] <- value))
        vector
        
    static member OfVector (dimension : int, func, vector : Vector) : DataDeque =
        vector.Enumerate (dimension)
        |> Seq.indexed
        |> Seq.groupBy (fun (i, value) -> value)
        |> Seq.map (fun (value, indicesValues) -> (value, indicesValues |> Seq.map fst |> Set |> FastCountSet.Create))
        |> Seq.toArray
        |> Array.sortBy fst
        |> RangeQuery.create func
        |> Deque.createOfRangeQueryArray
        |> DataDeque

    static member AssertRunTime = function
        | true  -> ()
        | false -> failwith "Assertun Failed!"

    member deque.IncreaseEntropy (averageEntropy : double, l1Distance : double) : DataDeque =
        if deque.IsSingle then
            deque
        else
            let (minValue, minAmount) = deque.MinValuePair
            let (maxValue, maxAmount) = deque.MaxValuePair
            DataDeque.AssertRunTime (maxValue >= averageEntropy)
            DataDeque.AssertRunTime (minValue <= averageEntropy)
            let secondMinValue = min averageEntropy (deque.SecondMinValue)
            let secondMaxValue = max averageEntropy (deque.SecondMaxValue)
            let minDiff = secondMinValue - minValue
            let maxDiff = maxValue - secondMaxValue
            DataDeque.AssertRunTime (maxDiff >= 0.0)
            DataDeque.AssertRunTime (minDiff >= 0.0)
            let minL1Distance = minDiff * minAmount
            let maxL1Distance = maxDiff * maxAmount
            let applicableL1Distance = min minL1Distance maxL1Distance
            let twiceApplicableL1Distance = 2.0 * applicableL1Distance
            if l1Distance < twiceApplicableL1Distance then
                let minChange = l1Distance / (2.0 * minAmount)
                let maxChange = l1Distance / (2.0 * maxAmount)
                deque.ChangeMinValue(minValue + minChange).ChangeMaxValue(maxValue - maxChange)
            elif minL1Distance < maxL1Distance then
                let maxChange = minL1Distance / maxAmount
                deque.CombineMin().ChangeMaxValue(maxValue - maxChange).IncreaseEntropy(averageEntropy, l1Distance - twiceApplicableL1Distance)
            elif minL1Distance > maxL1Distance then
                let minChange = maxL1Distance / minAmount
                deque.CombineMax().ChangeMinValue(minValue + minChange).IncreaseEntropy(averageEntropy, l1Distance - twiceApplicableL1Distance)
            elif secondMinValue = averageEntropy || secondMaxValue = averageEntropy then
                DataDeque.AssertRunTime (deque.SecondMinValue = maxValue)
                deque.ChangeMinValue(averageEntropy).CombineMax()
            else // Should almost never happen....
                deque.CombineMin().CombineMax().IncreaseEntropy(averageEntropy, l1Distance - twiceApplicableL1Distance)

    member deque.IncreaseEntropySketchValue (l1Distance : double) : DataDeque =
        let (maxValue, maxAmount) = deque.MaxValuePair
        if deque.IsSingle then
            deque.ChangeMaxValue (maxValue - l1Distance / maxAmount)
        else
            let secondMaxValue = deque.SecondMaxValue
            let maxDiff = maxValue - secondMaxValue
            let maxL1DistanceSpace = maxDiff * maxAmount
            if maxL1DistanceSpace >= l1Distance then
                deque.ChangeMaxValue (maxValue - l1Distance / maxAmount)
            else
                deque.CombineMax().IncreaseEntropySketchValue(l1Distance - maxL1DistanceSpace)
                