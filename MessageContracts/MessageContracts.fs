namespace MessageContracts

open System.Reflection
open Microsoft.FSharp.Reflection
open System.Runtime.Serialization
open System.IO
open System.Text  
open System.Xml
open System.Runtime.Serialization
open System.Runtime.Serialization.Formatters.Binary

[<KnownType("GetKnownTypes")>]
type Message =
    | DataSet of Map<string,string>   // Unfortunately, this doesn't work as I wanted.  In order for subscriber to deserialize a function it has to have a reference to the Publisher Assembly.
    with
    static member GetKnownTypes() =
        typedefof<Message>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic)
        |> Array.filter FSharpType.IsUnion

    static member Decode(packet : byte[]) =
        use ms = new MemoryStream(packet)
        let bf = new BinaryFormatter()
        let de = bf.Deserialize(ms) 
        de |> unbox<Message>

    member this.Encode() =
        use ms = new MemoryStream()
        let bf = new BinaryFormatter()
        bf.Serialize(ms, this)
        ms.GetBuffer()

    member this.XmlEncode() =
        let xmlSerializer = DataContractSerializer(typeof<Message>); 
        let sb = new StringBuilder()
        xmlSerializer.WriteObject(new XmlTextWriter(new StringWriter(sb)), this)
        sb.ToString()