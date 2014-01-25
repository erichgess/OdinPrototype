// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MessageContracts
open PerformanceCounters

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
    let cpuCounter = GetPerformanceCounter "Processor" |> Array.find ( fun cnt -> cnt.CounterName = "% Processor Time")
    while true do
        PublishMessage channel (TypeA(cpuCounter.NextValue()).Encode())
        PublishMessage channel (TypeB("string").Encode())
        System.Threading.Thread.Sleep(10)

    channel.Close()
    connection.Close()
    0 // return an integer exit code
