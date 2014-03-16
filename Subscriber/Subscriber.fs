// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MyConsumer
open MyMailboxProcessor
open System.Reactive.Linq
open MessageContracts
open StreamListener

let CreateConnectionFactory () = new ConnectionFactory(Uri = "amqp://192.168.1.111/")
let GetConnection (factory:ConnectionFactory) = factory.CreateConnection ()
let GetChannel (connection:IConnection) = connection.CreateModel()

let Consume (channel:IModel) queue = 
    let consumer = new MyConsumer(channel)
    channel.BasicConsume(queue, true, consumer) |> ignore
    consumer

let CreateRabbitMqEventStream queueName =
    let connectionFactory = CreateConnectionFactory ()
    let connection = GetConnection connectionFactory
    let channel = GetChannel connection
    channel.QueueDeclare( queueName, false, false, false, null) |> ignore

    let c = Consume channel queueName
    (c.Subject, fun () -> 
                    printfn "%A" (connection.Endpoint.HostName)
                    channel.Close()
                    connection.Close() )

let typeAListener = { Query = Some(Observable.filter( 
                                fun m -> 
                                    match m with
                                    | DataSet(d) -> System.Double.Parse( d.["%CPU"] )> 30.0));
                      Action = typeAMailbox.Post }

[<EntryPoint>]
let main argv = 

    let (queueSubject, cleanUpRabbitMq) = CreateRabbitMqEventStream "fsharp-queue"
    
    let attachListener stream listener =
        match listener.Query with
        | Some(query) -> stream |> query 
        | None -> stream
        |> Observable.subscribe ( listener.Action)
    let attachListenerToMainQueue = attachListener queueSubject

    attachListenerToMainQueue typeAListener |> ignore

    while true do ()

    cleanUpRabbitMq ()
    0 // return an integer exit code
