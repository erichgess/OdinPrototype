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

[<EntryPoint>]
let main argv = 
    let connectionFactory = CreateConnectionFactory ()
    let connection = GetConnection connectionFactory
    let channel = GetChannel connection

    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore
    let cpuCounter = GetPerformanceCounter "Processor" "% Processor Time"
    while true do
        PublishMessage channel (TypeA( cpuCounter () ) |> BinaryEncode )
        System.Threading.Thread.Sleep(50)

    channel.Close()
    connection.Close()
    0 // return an integer exit code
