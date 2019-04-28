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

    

