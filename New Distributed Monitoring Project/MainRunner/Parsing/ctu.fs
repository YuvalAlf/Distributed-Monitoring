﻿namespace Parsing

open Utils.SparseTypes
open System
open System.IO
open InternetCapturesParsing
open System.Security.Cryptography
open Utils.MathUtils
open System.Collections.Generic

type IndicesList = System.Collections.Generic.LinkedList<int>

type CtuManager(binFilePath : string, numOfNodes : int) =
    static let hashAlgorithm = MD5.Create() :> HashAlgorithm
    let binaryReader = new BinaryReader(File.OpenRead binFilePath)
    let pairsEnumerator =
        let pairs =
            InternetCaptureEntry.FromBinary binaryReader
            |> Seq.pairwise
        pairs.GetEnumerator()

    member val DidFinish =  false with get, set
    
    member ctu.Move () =
        let vectors = Vector.Init numOfNodes
        let mutable addOperations =  new Dictionary<int, IndicesList>(numOfNodes)
        [0 .. numOfNodes - 1] |> List.iter (fun node -> addOperations.Add(node, new IndicesList()))
        let rec fill10Minutes () : unit =
            match pairsEnumerator.MoveNext() with
            | false ->
                ctu.DidFinish <- true
            | true ->
                let currentCaptue, nextCapture = pairsEnumerator.Current
                let denominator = Math.Ceiling(256.0 / double(numOfNodes))
                let node = int(double(currentCaptue.SourceIP.Byte3) / denominator)
                let mutable index = hashAlgorithm.ComputeHash(currentCaptue.SourceIP.Address, currentCaptue.DestinationIP.Address)
                if index = Int32.MinValue then
                    index <- 5 // any number suffices, sine ABS of MinInt overflows
                index <- Math.Abs index
                addOperations.[node].AddLast(index) |> ignore
                if currentCaptue.Timestamp.Minute / 10 = nextCapture.Timestamp.Minute / 10 then
                    fill10Minutes ()
        let fillVectors () : unit =
            for node = 0 to numOfNodes - 1 do
                for index in addOperations.[node] do
                    vectors.[node].[index] <- vectors.[node].[index] + 1.0
        let rec balance () : unit =
            let minAmount = addOperations.Values |> Seq.map (fun lst -> lst.Count) |> Seq.min
            let maxAmount = addOperations.Values |> Seq.map (fun lst -> lst.Count) |> Seq.max
            if maxAmount >= minAmount + 2 then
                let minList = addOperations.Values |> Seq.minBy (fun lst -> lst.Count)
                let maxList = addOperations.Values |> Seq.maxBy (fun lst -> lst.Count)
                minList.AddLast(maxList.Last.Value) |> ignore
                maxList.RemoveLast() |> ignore
                balance ()

        fill10Minutes()
        balance()
        fillVectors()
        vectors
    
    member ctu.GetVectors amount =
        seq { for i = 1 to amount do yield ctu.Move() }
    interface IDisposable with
        member this.Dispose () = 
            pairsEnumerator.Dispose()
            binaryReader.Close()

type CtuProbabilityWindow(ctuManager : CtuManager, window : WindowedStatistics) =
    static member Init (path : string, numOfNodes : int, windowSize : int) : CtuProbabilityWindow =
        let ctuManager = new CtuManager(path, numOfNodes)
        let window = WindowedStatistics.Init(ctuManager.GetVectors windowSize)
        new CtuProbabilityWindow(ctuManager, window)
    member this.CurrentProbabilityVector () : Vector[] = window.CurrentNodesProbabilityVectors()
    member this.CurrentChangeProbabilityVector () : Vector[] = window.GetChangeProbabilityVectors()
    member this.MoveNext () : bool =
        if ctuManager.DidFinish then
            false
        else
            window.Move(ctuManager.Move())
            true    
    interface IDisposable with
        member this.Dispose () = 
            (ctuManager :> IDisposable).Dispose()

