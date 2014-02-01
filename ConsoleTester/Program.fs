// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open MessageContracts
open FSharp.Data

[<EntryPoint>]
let main argv = 
    let hw = @"Hello
    world"
    printfn "%s" hw

    let x = 1
    match x with
    | x when x < 0 -> printfn "1"
    | _ -> printfn "0"
    0 // return an integer exit code
