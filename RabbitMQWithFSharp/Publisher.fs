// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MessageContracts
open PerformanceCounters
open System.IO
open System.Runtime.Serialization
open System.Runtime.Serialization.Formatters.Binary

let BinaryEncode a =
    use ms = new MemoryStream()
    let bf = new BinaryFormatter()
    bf.Serialize(ms, a)
    ms.GetBuffer()

let CreateConnectionFactory () = new ConnectionFactory()
let GetConnection (factory:ConnectionFactory) = factory.CreateConnection ()
let GetChannel (connection:IConnection) = connection.CreateModel()

let PublishMessage (channel:IModel) (message) =
    let encodedMessage = message
    channel.BasicPublish ( "", "fsharp-queue", null, encodedMessage )

let publisher () = MailboxProcessor.Start(
                                fun mbox ->
                                    let connectionFactory = CreateConnectionFactory ()
                                    let connection = GetConnection connectionFactory
                                    let channel = GetChannel connection

                                    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore

                                    let rec loop() = 
                                        async{
                                            let! msg = mbox.Receive()
                                            printfn "%A" msg
                                            PublishMessage channel (msg |> BinaryEncode )
                                            return! loop()
                                        }
                                    loop()
                                );

let cpuPoller (publisher: MailboxProcessor<Message>) = MailboxProcessor.Start(
                                                        fun mbox -> 
                                                            let cpuCounter = GetPerformanceCounter "Processor" "% Processor Time"

                                                            let rec loop() = 
                                                                async{
                                                                    let! msg = mbox.Receive()
                                                                    let cpu = cpuCounter ()
                                                                    publisher.Post (TypeA(cpu))
                                                                    return! loop()
                                                                }
                                                            loop()
                                                    )

[<EntryPoint>]
let main argv = 
    
    let cpuCounter = GetPerformanceCounter "Processor" "% Processor Time"

    let mbox = publisher()
    let cpuBox = cpuPoller mbox

    while true do
        cpuBox.Post ()
        System.Threading.Thread.Sleep(500)

    0 // return an integer exit code
