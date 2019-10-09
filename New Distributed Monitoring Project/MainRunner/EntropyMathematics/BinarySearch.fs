namespace EntropyMathematics

module BinarySearch =

    let binarySteps<'data> (epsilon : double,
                            startingData : 'data,
                            move : 'data * double -> 'data,
                            dataOk : 'data -> bool) : 'data =
        let rec binaryStep (data, step) =
            if not <| dataOk(move(data, epsilon * 2.0)) then
                data
            else
                let movedData = move(data, step)
                if not <| dataOk(movedData) then
                    if step = epsilon then
                        data
                    else
                        binaryStep(data, epsilon)
                else
                    binaryStep(movedData, step * 2.0)
        binaryStep (startingData, epsilon)
