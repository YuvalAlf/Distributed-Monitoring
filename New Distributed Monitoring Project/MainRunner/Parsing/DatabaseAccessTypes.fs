namespace Parsing

open System

type DatabaseAccess = {TableId : int
                       UserId  : int}
                       
type TimedDatabaseAccess = { DatabaseAccess : DatabaseAccess
                             Timestamp      : int64 }
                             

module TimedDataAccess =
    open System
    open Parsings
    open System.IO
    open Utils.SparseTypes


    let tryParseTimedDatabaseAccess (csvLine : string) : TimedDatabaseAccess option =
        match csvLine.Split (",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) with
        | [| userIdString; tableIdString; timestampString |] ->
            optionWorkflow {
                let! userId = convert 0 (Int32.TryParse) userIdString
                let! tableId = convert 0 (Int32.TryParse) tableIdString
                let! timestamp = convert 0L (Int64.TryParse) timestampString
                return {DatabaseAccess = {TableId = tableId; UserId = userId}; Timestamp = timestamp}
            }
        | _ -> None

    let parseTimedDatabaseAccesses (csvPath : string) : TimedDatabaseAccess seq =
        File.ReadLines(csvPath)
        |> Seq.choose tryParseTimedDatabaseAccess

    let collectTimstampsOfData (timedDatabaseAccesses : TimedDatabaseAccess seq) : (DatabaseAccess list) seq =
        seq {
            let mutable list = []
            let dummyElement = {DatabaseAccess = {TableId = 1; UserId = 1}; Timestamp = -1L}
            let dataWithDummyLastElement = Seq.append timedDatabaseAccesses [dummyElement]
            let accessPairs = dataWithDummyLastElement |> Seq.pairwise
            for access1, access2 in accessPairs do
                list <- access1.DatabaseAccess :: list
                if access1.Timestamp <> access2.Timestamp then
                    yield list
                    list <- []
        }
    
    let accumalteVectorCounts (numOfNodes : int) (data : DatabaseAccess list) : Vector array =
        let vectors = Array.init numOfNodes (fun _ -> new Vector())
        let mutable node = -1
        for {TableId = tableId; UserId = userId} in data do
            //node <- (node + 1) % numOfNodes
            node <- userId % numOfNodes
            vectors.[node].[tableId] <- vectors.[node].[tableId] + 1.0
        vectors
            
    let createVectorCountsSequence (numOfNodes : int, csvPath : string) : (Vector[]) seq =
        csvPath
        |> parseTimedDatabaseAccesses
        |> collectTimstampsOfData
        |> Seq.map (accumalteVectorCounts numOfNodes)
        


