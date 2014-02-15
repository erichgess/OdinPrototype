// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MyConsumer
open MyMailboxProcessor
open System.Reactive.Linq
open MessageContracts

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

    let consumer = Consume channel "fsharp-queue"
    let aStream, bStream = consumer.Subject |> Observable.partition ( fun m -> match m with | TypeA(_) -> true | _ -> false)

    aStream          |> Observable.filter( fun m -> match m with | TypeA(m,a) when a > 40.0f -> true | _ -> false)
                     |> Observable.subscribe( typeAMailbox.Post )
                     |> ignore

    bStream          |> Observable.subscribe( typeBMailbox.Post )
                     |> ignore

    while true do ()

    channel.Close()
    connection.Close()

    0 // return an integer exit code
