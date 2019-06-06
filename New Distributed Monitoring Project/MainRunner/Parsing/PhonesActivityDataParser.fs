namespace Parsing

open System.IO
open System
open System.Collections.Generic
open Utils.TypeUtils
open Utils.AiderTypes

[<Sealed>]
type PhonesActivityDataParser(baseDirPath : string, 
                              dayDirName  : string, 
                              restDaysDirsNames : string list,
                              currentTimestampFileName : string,
                              nextTimestampsFileNames : string list) =
                      
    member m.NextTimestamp() : Maybe<PhonesActivityDataParser> * Func<IEnumerable<PhoneActivityEntry>> =
        let getDaysPhoneData () =
            let filePath = Path.Combine(baseDirPath, dayDirName, currentTimestampFileName)
            seq {
                use file = File.OpenRead filePath
                use binaryReader = new BinaryReader(file)
                assert(binaryReader.BaseStream <> null)
                yield! PhoneActivityEntry.FromBinary binaryReader
            }
        let nextMilano : Maybe<PhonesActivityDataParser> =
            match nextTimestampsFileNames with
            | newTimestampFileName :: newNextTimestampsFileNames ->
                Maybe.Some(PhonesActivityDataParser(baseDirPath, dayDirName, restDaysDirsNames, newTimestampFileName, newNextTimestampsFileNames))                    
            | [] -> match restDaysDirsNames with
                    | [] -> Maybe.None<_>()
                    | newDayDirName :: newRestDaysDirsNames ->
                        let newTimestampFileName, newNextTimestampsFileNames = PhonesActivityDataParser.GetTimestampFiles(baseDirPath, newDayDirName)
                        Maybe.Some(PhonesActivityDataParser(baseDirPath, newDayDirName, newRestDaysDirsNames, newTimestampFileName, newNextTimestampsFileNames))

        (nextMilano, new Func<_>(getDaysPhoneData))

    static member private GetTimestampFiles (baseDirPath, dayDirName) =
        let dayDirPath = Path.Combine(baseDirPath, dayDirName)
        match Directory.EnumerateFiles dayDirPath |> Seq.toList with
        | [] -> failwith <| sprintf "Directory %s contains no inner files" baseDirPath
        | currentTimestampFileName :: nextTimestampsFileNames ->
            (currentTimestampFileName, nextTimestampsFileNames)

    static member Create (baseDirPath : string) =
        match Directory.EnumerateDirectories baseDirPath |> Seq.toList with
        | [] -> failwith <| sprintf "Directory %s contains no inner directories" baseDirPath
        | dayDirName :: restDaysDirsNames ->
            let currentTimestampFileName, nextTimestampsFileNames = PhonesActivityDataParser.GetTimestampFiles(baseDirPath, dayDirName)
            PhonesActivityDataParser(baseDirPath, dayDirName, restDaysDirsNames, currentTimestampFileName, nextTimestampsFileNames)

