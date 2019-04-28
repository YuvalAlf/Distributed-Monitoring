namespace Parsing

open Utils.SparseTypes
open System
open System.Collections.Generic

type StatisticsVectors(vectors: Vector[]) =
    member this.Vectors = vectors
    member this.NumOfNodes = vectors.Length
    member this.Item nodeIndex = vectors.[nodeIndex]

type WindowedStatistics(timedQueue : Queue<StatisticsVectors>) =
    let numOfNodes = timedQueue.Peek().NumOfNodes
    let addVectorsInPlace (addTo : Vector[], addFrom : Vector[]) : unit =
        Array.zip addTo addFrom
        |> Array.iter (fun (vectorToAddTo, vectorToAddFrom) -> vectorToAddTo.AddInPlace(vectorToAddFrom))
    let nodesCountVectors : Vector array =
        let countVectors = Vector.Init(numOfNodes)
        timedQueue
        |> Seq.iter (fun sVector -> addVectorsInPlace(countVectors, sVector.Vectors))
        countVectors
    let mutable maybeChangeVector : Option<Vector[]> = None
    
    member this.CurrentNodesCountVectors () : Vector[] = nodesCountVectors
    
    member this.GetChangeCountVectors () : Vector[] =
        match maybeChangeVector with
        | None -> failwith "Change-Vector not initilized"
        | Some changeVector -> changeVector

    member this.Move(newVectors : Vector array) : unit =
        let lastStatisticsVector = timedQueue.Dequeue()
        let newStatisticsVector = newVectors |> StatisticsVectors
        timedQueue.Enqueue(newStatisticsVector)

        let vectorsDiff =
            Array.zip newStatisticsVector.Vectors lastStatisticsVector.Vectors
            |> Array.map (fun (v1, v2) -> v1 - v2)

        addVectorsInPlace (nodesCountVectors, vectorsDiff)

        maybeChangeVector <- Some(vectorsDiff)
        ()
        
    static member Init (initVectors : Vector[] seq) : WindowedStatistics =
        initVectors
        |> Seq.map StatisticsVectors
        |> Queue
        |> WindowedStatistics