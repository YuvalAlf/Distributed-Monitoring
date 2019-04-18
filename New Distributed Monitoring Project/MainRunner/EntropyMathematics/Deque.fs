namespace EntropyMathematics

open System.Collections
open System.Collections.Generic

type Deque<'T> =
    | OneValue of 'T
    | Values of min : 'T * minIndex : int * data : 'T array * maxIndex : int * max : 'T

    static member CreateOfArray (data : 'T array) =
        match data.Length with
        | 0 -> failwith "Error"
        | 1 -> OneValue data.[0]
        | l -> Values (data.[0], 1, data, l-2, data.[l-1])
    member deque.Head =
        match deque with
        | OneValue value -> value
        | Values (min, _, _, _, _) -> min
    member deque.Last =
        match deque with
        | OneValue value -> value
        | Values (_, _, _, _, max) -> max

    member deque.Tail =
        match deque with
        | OneValue value -> failwith "Error"
        | Values (min, minIndex, data, maxIndex, max) ->
            if minIndex > maxIndex then
                OneValue max
            else
                Values (data.[minIndex], minIndex + 1, data, maxIndex, max)
    member deque.Init =
        match deque with
        | OneValue value -> failwith "Error"
        | Values (min, minIndex, data, maxIndex, max) ->
            if minIndex > maxIndex then
                OneValue min
            else
                Values (min, minIndex, data, maxIndex - 1, data.[maxIndex])

    member deque.IsSingle =
        match deque with
        | OneValue  _ -> true
        | Values    _ -> false

    member deque.ToSeq () =
        seq{
            match deque with
            | OneValue value -> yield value
            | Values (min, minIndex, data, maxIndex, max) ->
                yield min
                for i = minIndex to maxIndex do
                    yield data.[i]
                yield max
        }
        
    member deque.ReplaceMin newMin =
        match deque with
        | OneValue _ -> OneValue newMin
        | Values (_, minIndex, data, maxIndex, max) ->
            Values (newMin, minIndex, data, maxIndex, max)
    member deque.ReplaceMax newMax =
        match deque with
        | OneValue _ -> OneValue newMax
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
    let inline createOfArray (arr : 'a array) = Deque.CreateOfArray arr


  (*   let (|Cons|Nil|) (q : Deque<'T>) = match q.TryUncons with Some(a,b) -> Cons(a,b) | None -> Nil

    let (|Conj|Nil|) (q : Deque<'T>) = match q.TryUnconj with Some(a,b) -> Conj(a,b) | None -> Nil

    let inline conj (x : 'T) (q : Deque<'T>) = (q.Conj x) 

    let inline cons (x : 'T) (q : Deque<'T>) = q.Cons x 

    let empty<'T> = Deque<'T>(List.Empty, List.Empty)

    let fold (f : ('State -> 'T -> 'State)) (state : 'State) (q : Deque<'T>) = 
        let s = List.fold f state q.front
        List.fold f s (List.rev q.rBack)

    let foldBack (f : ('T -> 'State -> 'State)) (q : Deque<'T>) (state : 'State) =  
        let s = List.foldBack f (List.rev q.rBack) state 
        (List.foldBack f q.front s)

    let inline head (q : Deque<'T>) = q.Head

    let inline tryHead (q : Deque<'T>) = q.TryHead

    let inline initial (q : Deque<'T>) = q.Initial 

    let inline tryInitial (q : Deque<'T>) = q.TryInitial 

    let inline isEmpty (q : Deque<'T>) = q.IsEmpty

    let inline last (q : Deque<'T>) = q.Last

    let inline tryLast (q : Deque<'T>) = q.TryLast

    let inline length (q : Deque<'T>) = q.Length

    let ofCatLists (xs : 'T list) (ys : 'T list) = Deque<'T>(xs, (List.rev ys))

    let ofList (xs : 'T list) = Deque<'T>(xs, [])

    let ofSeq (xs:seq<'T>) = Deque<'T>((List.ofSeq xs), [])

    let inline rev (q : Deque<'T>) = q.Rev

    let singleton (x : 'T) = Deque<'T>([x], List.Empty)

    let inline tail (q : Deque<'T>) = q.Tail 

    let inline tryTail (q : Deque<'T>) = q.TryTail 

    let inline uncons (q : Deque<'T>) = q.Uncons

    let inline tryUncons (q : Deque<'T>) = q.TryUncons

    let inline unconj (q : Deque<'T>) = q.Unconj

    let inline toSeq (q : Deque<'T>) = q :> seq<'T>

    let inline tryUnconj (q : Deque<'T>) = q.TryUnconj*)