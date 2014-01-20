// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MyConsumer

let CreateConnectionFactory () = new ConnectionFactory()
let GetConnection (factory:ConnectionFactory) = factory.CreateConnection ()
let GetChannel (connection:IConnection) = connection.CreateModel()

let Consume (channel:IModel) queue = 
    let consumer = new MyConsumer(channel)
    channel.BasicConsume(queue, true, consumer) |> ignore
    consumer

[<EntryPoint>]
let main argv = 
    let connectionFactory = CreateConnectionFactory ()
    let connection = GetConnection connectionFactory
    let channel = GetChannel connection
    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore

    Consume channel "fsharp-queue" |> ignore

    while true do ()

    channel.Close()
    connection.Close()

    0 // return an integer exit code
