namespace EntropyMathematics

open System

type Tree<'t when 't :> IComparable<'t>> = { Data : 't array
                                             Left : Option<Tree<'t>>
                                             CurrentIndex : int
                                             Right : Option<Tree<'t>>} with
    member tree.Count () =
        let getCount (treeOption : Tree<'t> option) = 
            treeOption 
            |> Option.map (fun t -> t.Count())
            |> Option.defaultValue 0
        1 + getCount(tree.Left) + getCount(tree.Right)
    member tree.CurrentValue = tree.Data.[tree.CurrentIndex]
    member tree.GetClosestSmallerValueIndex(value : 't) =
        let x = tree.CurrentValue
        match tree.Left, tree.CurrentValue.CompareTo(value), tree.Right with
        | _,              0, _ -> tree.CurrentIndex
        | None,           d, _ when d > 0 -> if tree.CurrentIndex = 0 then raise(IndexOutOfRangeException()) else tree.CurrentIndex - 1
        | Some(leftNode), d, _ when d > 0 -> leftNode.GetClosestSmallerValueIndex(value)
        | _,              d, None when d < 0 -> tree.CurrentIndex
        | _,              d, Some(rightNode) when d < 0 -> rightNode.GetClosestSmallerValueIndex(value)
        | otherwise -> failwith "Programming Error"

    static member InitClosestValueQuery(values : 't array) : Tree<'t> =
        let values = values |> Array.sortWith (fun x y -> (x :> IComparable<'t>).CompareTo(y))
        let rec createTreeRec (startingIndex : int) (endingIndex : int) : Option<Tree<'t>> = 
            if startingIndex > endingIndex then
                None
            else
                let nodeIndex = startingIndex + (endingIndex - startingIndex) / 2
                let left = createTreeRec startingIndex (nodeIndex - 1)
                let right = createTreeRec (nodeIndex + 1) endingIndex
                Some ({Data = values; Left = left; CurrentIndex = nodeIndex; Right = right})

        createTreeRec 0 (values.Length - 1)
        |> Option.get

    static member InitExponentialClosestValueQuery (startValue : int64, max : int64, mulFactor : double) =
        Tree.InitClosestValueQuery
            [|
                yield 0L
                let generator value =
                    let mutable nextValue = Math.Floor(double(value) * mulFactor) |> int64
                    if value = nextValue then
                        nextValue <- nextValue + 1L
                    if value > max then None else Some(value, nextValue)
                yield! (startValue |> Seq.unfold generator)

            |]

module ClosestValueQuery =
    let Init<'t when 't :> IComparable<'t>> values = Tree<'t>.InitClosestValueQuery values
    let InitExponential (startValue : int64, max : int64, mulFactor : double) =
        Tree<int64>.InitExponentialClosestValueQuery (startValue, max, mulFactor)