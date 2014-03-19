// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MessageContracts
open PerformanceCounters
open System.IO
open System.Runtime.Serialization
open System.Runtime.Serialization.Formatters.Binary
open RabbitMQ.FSharp.Client

let BinaryEncode a =
    use ms = new MemoryStream()
    let bf = new BinaryFormatter()
    bf.Serialize(ms, a)
    ms.GetBuffer()


let cpuPoller (publisher: MailboxProcessor<Message>) name counter = 
    MailboxProcessor.Start(
        fun mbox -> 
            let rec loop() = 
                async{
                    let! msg = mbox.Receive()
                    let value = counter ()
                    publisher.Post (DataSet(Map.ofList ["%CPU", value.ToString() ] ))
                    return! loop()
                }
            loop()
    )

[<EntryPoint>]
let main argv = 
    let connection = connectToRabbitMqServerAt "amqp://192.168.1.139/"
    let channel = openChannelOn connection

    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore

    let cpuCounter = GetPerformanceCounter "Processor" "% Processor Time"
    let queueWriter = createQueueWriter channel "fsharp-queue"
    while true do
        let value = cpuCounter ()
        let msg = sprintf "%%CPU=%f" value
        printfn "%s" msg
        queueWriter msg
        System.Threading.Thread.Sleep(100)

    0 // return an integer exit code
