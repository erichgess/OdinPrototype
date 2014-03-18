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

//let CreateConnectionFactory () = new ConnectionFactory(Uri = "amqp://192.168.1.111/")
//let GetConnection (factory:ConnectionFactory) = factory.CreateConnection ()
//let GetChannel (connection:IConnection) = connection.CreateModel()
//
//let PublishMessage (channel:IModel) (message) =
//    let encodedMessage = message
//    channel.BasicPublish ( "", "fsharp-queue", null, encodedMessage )

//let publisher () = MailboxProcessor.Start(
//                                fun mbox ->
//                                    let connectionFactory = CreateConnectionFactory ()
//                                    let connection = GetConnection connectionFactory
//                                    let channel = GetChannel connection
//
//                                    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore
//
//                                    let rec loop() = 
//                                        async{
//                                            let! msg = mbox.Receive()
//                                            printfn "%A" msg
//                                            PublishMessage channel (msg |> BinaryEncode )
//                                            return! loop()
//                                        }
//                                    loop()
//                                );

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
    let connection = connectToRabbitMqServerAt "amqp://192.168.137.44/"
    let channel = openChannelOn connection

    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore

    let cpuCounter = GetPerformanceCounter "Processor" "% Processor Time"
    //let cpuBox =  [ cpuPoller mbox "%CPU" cpuCounter;]
    let queueWriter = createQueueWriter channel "fsharp-queue"
    while true do
        let value = cpuCounter ()
        //let msg = DataSet(Map.ofList ["%CPU", value.ToString() ] )
        let msg = sprintf "%%CPU=%f" value
        printfn "%s" msg
        queueWriter msg

//        cpuBox |> List.iter (fun m -> m.Post () )
        //System.Threading.Thread.Sleep(10)

    0 // return an integer exit code
