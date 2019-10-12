namespace EntropyMathematics

type Deque<'T> =
    | OneValue of value : 'T * func : ('T -> double)
    | Values of min : 'T * minIndex : int * data : 'T RangeQueryArray * maxIndex : int * max : 'T // Inclusive

    static member CreateOfRangeQueryArray (data : 'T RangeQueryArray) =
        match data.Length with
        | 0 -> failwith "Error"
        | 1 -> OneValue (data.[0], data.Func)
        | l -> Values (data.[0], 1, data, l-2, data.[l-1])
    member deque.FunctionValue =
        match deque with
        | OneValue (value, func) -> func value
        | Values (min, minIndex, data, maxIndex, max) ->
            (data.Func min) + (data.Func max) + data.ValueOfRange(minIndex, maxIndex)
    member deque.Head =
        match deque with
        | OneValue (value, _) -> value
        | Values (min, _, _, _, _) -> min
    member deque.Last =
        match deque with
        | OneValue (value, _) -> value
        | Values (_, _, _, _, max) -> max

    member deque.Tail =
        match deque with
        | OneValue (value, _) -> failwith "Error"
        | Values (min, minIndex, data, maxIndex, max) ->
            if minIndex > maxIndex then
                OneValue (max, data.Func)
            else
                Values (data.[minIndex], minIndex + 1, data, maxIndex, max)
    member deque.Init =
        match deque with
        | OneValue (value, _) -> failwith ("Error, value = " + value.ToString())
        | Values (min, minIndex, data, maxIndex, max) ->
            if minIndex > maxIndex then
                OneValue (min, data.Func)
            else
                Values (min, minIndex, data, maxIndex - 1, data.[maxIndex])

    member deque.IsSingle =
        match deque with
        | OneValue  (_,_)       -> true
        | Values    (_,_,_,_,_) -> false

    member deque.ToSeq () =
        seq {
            match deque with
            | OneValue (value, _) -> yield value
            | Values (min, minIndex, data, maxIndex, max) ->
                yield min
                for i = minIndex to maxIndex do
                    yield data.[i]
                yield max
        }
        
    member deque.ReplaceMin newMin =
        match deque with
        | OneValue (_, func) -> OneValue (newMin, func)
        | Values (_, minIndex, data, maxIndex, max) ->
            Values (newMin, minIndex, data, maxIndex, max)
    member deque.ReplaceMax newMax =
        match deque with
        | OneValue (_, func) -> OneValue (newMax, func)
        | Values (min, minIndex, data, maxIndex, _) ->
            Values (min, minIndex, data, maxIndex, newMax)

[<AutoOpen>]
module Deque =
    let inline head (q : 'a Deque) = q.Head
    let inline last (q : 'a Deque) = q.Last
    let inline tail (q : 'a Deque) = q.Tail
    let inline init (q : 'a Deque) = q.Init
    let inline isSingle (q : 'a Deque) = q.IsSingle
    let inline toSeq (q : 'a Deque) = q.ToSeq()
    let inline replaceMin newMin (q : 'a Deque) = q.ReplaceMin newMin
    let inline replaceMax newMax (q : 'a Deque) = q.ReplaceMax newMax
    let inline createOfRangeQueryArray (arr : 'a RangeQueryArray) = Deque.CreateOfRangeQueryArray arr