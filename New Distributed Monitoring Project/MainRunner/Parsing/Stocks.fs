namespace Parsing

open System.IO
open System
open System.Collections.Generic
open Utils.TypeUtils
open Utils.AiderTypes
open Parsings
open Utils.SparseTypes

type StockValue = { Time      : DateTime
                    Volume    : int64
                    OpenValue : float32 } with
    static member TryParse (line : string) : StockValue Option =
        let tokens = line.Split ','
        if tokens.Length <= 5 then
            None
        else
            optionWorkflow {
                let! time = convert (DateTime.MinValue) (DateTime.TryParse) tokens.[0]
                let! openValue = convert 0.0f (Single.TryParse) tokens.[1]
                let! volume = convert 0L (Int64.TryParse) tokens.[5]
                return {Time = time.Date; Volume = volume; OpenValue = openValue}
            }
    
[<Sealed>] 
type ActiveStock(stream : StreamReader, currentStockValue : StockValue) =
    let dateEquals (d1 : DateTime) (d2 : DateTime) =
        d1.Day = d2.Day && d1.Month = d2.Month && d1.Year = d2.Year

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
            let line = stream.ReadLine()
            let nextStockValue = StockValue.TryParse(line) |> Option.get
            Some (new ActiveStock(stream, nextStockValue))

    member this.MoveToDate (date : DateTime) : ActiveStock Option =
        if this.CurrentStockValue.Time.Date >= date.Date then
            Some this
        else
            this.MoveNext()
            |> Option.bind (fun s -> s.MoveToDate(date))

    interface IDisposable with 
        member this.Dispose() =
            stream.Dispose()

type StocksManager (activeStocks : Map<string, ActiveStock>,
                    currentDate : DateTime,
                    minAmountAtDay : int) =
    static member Init (stockFilesPathes : string array, startingDate : DateTime, minAmountAtDay : int) =
        let addStock (map : Map<string, ActiveStock>) (path : string) =
            let name = Path.GetFileNameWithoutExtension path
            let stock = ActiveStock.Init path
            match stock.MoveToDate startingDate with
            | None -> map
            | Some stockAtDate -> map.Add(name, stockAtDate)
        
        let activeStocks = stockFilesPathes |> Array.fold addStock (Map.empty)

        new StocksManager (activeStocks, startingDate, minAmountAtDay)

    member this.MoveNext() : StocksManager Option =
        if activeStocks.Count = 0 then
            None
        else
            let nextDate = currentDate.AddDays 1.0
            let addStock (map : Map<string, ActiveStock>) (name : string) (stock : ActiveStock) =
                match stock.MoveToDate nextDate with
                | None -> map
                | Some stockAtDate -> map.Add(name, stockAtDate)

            let newActiveStocks = activeStocks |> Map.fold addStock (Map.empty)

            let activeStocksAtDate =
                newActiveStocks
                |> Map.values
                |> Seq.where (fun stock -> stock.IsAtDate nextDate)
                |> Seq.length
            
            let newStockManager = new StocksManager(newActiveStocks, nextDate, minAmountAtDay)

            if activeStocksAtDate >= minAmountAtDay then
                Some(newStockManager)
            else
                newStockManager.MoveNext()

    member this.ProbabilityVectors (numOfNodes : int, stocksIndices : Map<string, int>) =
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
            let probabilityVector = this.ProbabilityVectors(numOfNodes, stocksIndices)
            let nextManager = this.MoveNext() |> Option.get
            let (resultManager, vectors) = nextManager.GetProbabilityVectors(numOfNodes, stocksIndices, amount - 1)
            (resultManager, probabilityVector :: vectors)

    interface IDisposable with
        member this.Dispose () =
            activeStocks
            |> Map.iter (fun _ stock -> (stock :> IDisposable).Dispose())


type StocksProbabilityWindow(stocksIndices : Map<string, int>, initialStocksManager : StocksManager, window : WindowedStatistics, numOfNodes : int) =
    let mutable stocksManager = Some(initialStocksManager)

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

        new StocksProbabilityWindow(stocksIndices, stocksManager, window, numOfNodes)
    
    member this.CurrentProbabilityVector () : Vector[] = window.CurrentNodesProbabilityVectors()
    member this.CurrentChangeProbabilityVector () : Vector[] = window.GetChangeProbabilityVectors()
    member this.MoveNext () : bool =
       match stocksManager with
       | None -> false
       | Some manager ->
            stocksManager <- manager.MoveNext()
            match stocksManager with
            | None -> false
            | Some stock ->
                window.Move(stock.ProbabilityVectors(numOfNodes, stocksIndices))
                true

    member this.VectorLength = stocksIndices |> Map.count

    interface IDisposable with
        member this.Dispose () = 
            (initialStocksManager :> IDisposable).Dispose()
    