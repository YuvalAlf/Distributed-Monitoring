namespace Parsings

type OptionWorkflow() =
    member this.Bind(x, f) = 
        match x with
        | None -> None
        | Some a -> f a

    member this.Return(x) = 
        Some x
   

[<AutoOpen>]
module FsharpUtils =    
    let optionWorkflow = new OptionWorkflow()
    let convert defaultValue convertionFunc dataToConvert =
        let result = ref defaultValue
        match convertionFunc(dataToConvert, result) with
        | false -> None
        | true -> Some !result

module Map =
    let values (map : Map<'a,'b>) =
        map
        |> Map.toSeq
        |> Seq.map snd

module Seq =
    let evenChuncks (numOfChuncks : int) (sequence : (string * int64) seq) : (string * double * int) seq =
        let array = sequence |> Seq.toArray
        let elementCount = array.Length
        let chunckBaseSize = elementCount / numOfChuncks
        let reminder = elementCount - chunckBaseSize * numOfChuncks
        let chunckSize = chunckBaseSize + reminder
        seq {
            for chunck = 0 to numOfChuncks - 1 do    
                for i = 0 to chunckBaseSize - 1 do
                    let str, value = array.[chunckBaseSize * chunck + i]
                    yield (str, double(value), chunck)
          //  for i = 1 to reminder do
           //     let str, value = array.[elementCount - i]
           //     for chunck = 0 to numOfChuncks - 1 do
            //        yield (str, double(value) / double(numOfChuncks), chunck)
        }

    

