// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text
open MyConsumer
open MyMailboxProcessor
open System.Reactive.Linq
open MessageContracts
open StreamListener
open RabbitMQ.FSharp.Client


let connection = connectToRabbitMqServerAt "amqp://192.168.137.160/"


let CreateRabbitMqEventStream queueName =
    let channel = openChannelOn connection
    let consumer = createQueueConsumer channel queueName
    let queue = seq{ while true do yield (consumer ()) }
    (queue.ToObservable (), fun () -> 
                    printfn "%A" (connection.Endpoint.HostName)
                    channel.Close()
                    connection.Close() )

let typeAListener = { Query = Some(Observable.filter( 
                                    fun m -> 
                                        match m with
                                        | DataSet(d) -> System.Double.Parse( d.["%CPU"] )> 30.0));
                      Action = typeAMailbox.Post }

let stringListener = { Query = None; Action = fun m -> printfn "%s" m}

[<EntryPoint>]
let main argv = 

    let (queueSubject, cleanUpRabbitMq) = CreateRabbitMqEventStream "fsharp-queue"
    
    let attachListener stream listener =
        match listener.Query with
        | Some(query) -> stream |> query 
        | None -> stream
        |> Observable.subscribe ( listener.Action)
    let attachListenerToMainQueue = attachListener queueSubject

    attachListenerToMainQueue stringListener |> ignore

    while true do ()

    cleanUpRabbitMq ()
    0 // return an integer exit code
