namespace Parsing

open System.IO
open System
open System.Collections.Generic
open Utils.TypeUtils
open Utils.AiderTypes
open Parsings
open Utils.SparseTypes

type StockValue = { Time : DateTime
                    Volume : int
                    OpenValue : float32 } with
    static member TryParse (line : string) : StockValue Option =
        let tokens = line.Split ' '
        if tokens.Length <= 5 then
            None
        else
            optionWorkflow {
                let! time = convert (DateTime.MinValue) (DateTime.TryParse) tokens.[0]
                let! openValue = convert 0.0f (Single.TryParse) tokens.[1]
                let! volume = convert 0 (Int32.TryParse) tokens.[5]
                return {Time = time; Volume = volume; OpenValue = openValue}
            }
    
[<Sealed>] 
type ActiveStock(stream : StreamReader, currentStockValue : StockValue) =
    static member Init (path : string) =
        let stream = File.OpenText path
        stream.ReadLine() |> ignore
        let currentValue = StockValue.TryParse(stream.ReadLine()) |> Option.get
        new ActiveStock(stream, currentValue)

    member this.CurrentStockValue = currentStockValue

    member this.IsAtDate (date : DateTime) = currentStockValue.Time.Date.Equals (date.Date)

    member this.MoveNext () : ActiveStock Option =
        if stream.EndOfStream then
            stream.Close()
            None
        else
            let nextStockValue = StockValue.TryParse(stream.ReadLine()) |> Option.get
            Some (new ActiveStock(stream, nextStockValue))

    member this.MoveToDate (date : DateTime) : ActiveStock Option =
        match this.MoveNext() with
        | None -> None
        | Some (activeStock) ->
            if activeStock.CurrentStockValue.Time < date then
                activeStock.MoveToDate date
            else
                Some activeStock

    interface IDisposable with 
        member this.Dispose() =
            stream.Dispose()

type StocksManager (activeStocks : Map<string, ActiveStock>,
                    currentDate : DateTime,
                    minAmountAtDay : int) =
    static member Init (stockFilePathes : string array, startingDate, minAmountAtDay : int) =
        let addStock (map : Map<string, ActiveStock>) (path : string) =
            let name = Path.GetFileNameWithoutExtension path
            let stock = ActiveStock.Init path
            map.Add(name, stock)
        
        let activeStocks = stockFilePathes |> Array.fold addStock (Map.empty)

        new StocksManager (activeStocks, startingDate, minAmountAtDay)

    member this.MoveNext() : StocksManager Option =
        if activeStocks.Count = 0 then
            None
        else
            let nextDate = currentDate + (TimeSpan.FromDays 1.0)
            let mutable newActiveStocks : Map<string, ActiveStock> = Map.empty
            activeStocks
            |> Map.iter (fun name activeStock ->
                            match activeStock.MoveToDate nextDate with
                            | Some newStock -> newActiveStocks <- newActiveStocks.Add(name, newStock)
                            | _             -> ())
            let activeStocksAtDate =
                newActiveStocks
                |> Map.values
                |> Seq.where (fun stock -> stock.IsAtDate currentDate)
                |> Seq.length
            
            let newStockManager = new StocksManager(newActiveStocks, nextDate, minAmountAtDay)

            if activeStocksAtDate >= minAmountAtDay then
                Some(newStockManager)
            else
                newStockManager.MoveNext()

    member this.ToProbabilityVectors (numOfNodes : int, stocksIndices : Map<string, int>) =
        let vectors = Vector.Init (numOfNodes)
        activeStocks
        |> Map.toSeq
        |> Seq.where (fun (n, s) -> s.IsAtDate currentDate)
        |> Seq.sortBy (fun (n, s) -> s.CurrentStockValue.OpenValue)
        |> Seq.map (fun (n, s) -> (n, s.CurrentStockValue.Volume))
        |> Seq.evenChuncks numOfNodes
        |> Seq.iter (fun (name, value, node) -> vectors.[node].[stocksIndices.[name]] <- value)

        vectors
    
    member this.GetProbabilityVectors (numOfNodes, stocksIndices, amount) : (StocksManager * Vector[] list) =
        if amount = 0 then
            (this, [])
        else
            let probabilityVector = this.ToProbabilityVectors(numOfNodes, stocksIndices)
            let nextManager = this.MoveNext() |> Option.get
            let (resultManager, vectors) = nextManager.GetProbabilityVectors(numOfNodes, stocksIndices, amount - 1)
            (resultManager, probabilityVector :: vectors)

    interface IDisposable with
        member this.Dispose () =
            activeStocks
            |> Map.iter (fun _ stock -> (stock :> IDisposable).Dispose())


type StocksProbabilityWindow(stocksIndices : Map<string, int>, stocksManager : StocksManager, window : WindowedStatistics) =
    let mutable stocksManager = Some(stocksManager)

    static member Init (dirPath : string, startingDate : DateTime, minAmountAtDay : int, numOfNodes : int, windowSize : int) : StocksProbabilityWindow =
        let files = Directory.EnumerateFiles (dirPath) |> Seq.toArray
        let names = files |> Array.map (Path.GetFileNameWithoutExtension) |> Array.sort
        let stocksIndices = 
            names 
            |> Array.indexed 
            |> Array.fold (fun (map : Map<string, int>) (index, name) -> map.Add(name, index)) (Map.empty)
        let initStocksManager = StocksManager.Init(files, startingDate, minAmountAtDay)
        let (stocksManager, initProbabilityVectors) = initStocksManager.GetProbabilityVectors (numOfNodes, stocksIndices, windowSize)
        let window = WindowedStatistics.Init(initProbabilityVectors)

        StocksProbabilityWindow(stocksIndices, stocksManager, window)
    
    member this.CurrentProbabilityVector () : Vector[] = window.CurrentNodesProbabilityVectors()
    member this.CurrentChangeProbabilityVector () : Vector[] = window.GetChangeProbabilityVectors()
    member this.MoveNext () : bool =
       match stocksManager with
       | None -> false
       | Some manager ->
            stocksManager <- manager.MoveNext()
            match stocksManager with
            | None -> false
            | _    -> true

    member this.VectorLength = stocksIndices |> Map.count

    interface IDisposable with
        member this.Dispose () = 
            match stocksManager with
            | None -> ()
            | Some manager -> (manager :> IDisposable).Dispose()
    