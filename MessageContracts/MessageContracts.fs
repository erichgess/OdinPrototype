namespace MessageContracts


open System.Runtime.Serialization
open System.IO
open System.Runtime.Serialization.Formatters.Binary

type Message =
    | TypeA of int
    | TypeB of string
    with
    static member Decode(packet : byte[]) =
        use ms = new MemoryStream(packet)
        let bf = new BinaryFormatter()
        bf.Deserialize(ms) 
        |> unbox<Message>

    member this.Encode() =
        use ms = new MemoryStream()
        let bf = new BinaryFormatter()
        bf.Serialize(ms, this)
        ms.GetBuffer()