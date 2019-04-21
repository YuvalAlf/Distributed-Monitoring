namespace EntropyMathematics

type RangeQueryArray<'a>(array : 'a array, func : 'a -> double) =
    let queryValues =
            array
            |> Array.scan (fun acc value -> func(value) + acc) 0.0
            |> Array.skip 1

    member this.Item i = array.[i]

    member this.Func = func

    member this.Length = array.Length

    member this.ValueOfRange (startIndex : int, endIndex : int) = // Inclusive
        if startIndex <= 0 then
            queryValues.[endIndex]
        else
            queryValues.[endIndex] - queryValues.[startIndex - 1]

    static member Create (func, array) = RangeQueryArray(array, func)

    module RangeQuery =
        let create<'a> (func : 'a -> double) (array : 'a array) = 
            RangeQueryArray<'a>.Create (func, array)