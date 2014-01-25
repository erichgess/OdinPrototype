// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open MessageContracts

[<EntryPoint>]
let main argv = 
    let t = TypeA(1)

    printfn "%s" (t.XmlEncode())
    0 // return an integer exit code
