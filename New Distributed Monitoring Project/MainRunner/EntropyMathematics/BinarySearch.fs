namespace EntropyMathematics

module BinarySearch =

    let rec binarySteps<'data> (epsilon : double,
                                step : double,
                                data : 'data,
                                move : 'data * double -> 'data,
                                dataOk : 'data -> bool) : 'data =
        if not <| dataOk(move(data, epsilon * 2.0)) then
            data
        else
            let movedData = move(data, step)
            if not <| dataOk(movedData) then
                binarySteps(epsilon, epsilon, data, move, dataOk)
            else
                binarySteps(epsilon, step * 2.0, movedData, move, dataOk)

