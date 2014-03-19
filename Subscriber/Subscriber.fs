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

let connection = connectToRabbitMqServerAt "amqp://192.168.1.139/"

let CreateRabbitMqEventStream queueName =
    let channel = openChannelOn connection
    let consumer = createQueueConsumer channel queueName
    let queue = seq{ while true do yield (consumer ()) }
    (queue.ToObservable (), fun () ->
                    printfn "%A" (connection.Endpoint.HostName)
                    channel.Close())

let typeAListener = { Query = Some(Observable.filter(
                                    fun m ->
                                        match m with
                                        | DataSet(d) -> System.Double.Parse( d.["%CPU"] )> 30.0));
                      Action = typeAMailbox.Post }

let stringListener = { Query = None; Action = fun m -> printfn "1 - %s" m}
let stringListener2 = { Query = None; Action = fun m -> printfn "2 - %s" m}

[<EntryPoint>]
let main argv =

    let (queueSubject, cleanUpReader) = CreateRabbitMqEventStream "fsharp-queue"
    let (queueSubject2,_) = CreateRabbitMqEventStream "fsharp-queue"

    let attachListener stream listener =
        match listener.Query with
        | Some(query) -> stream |> query
        | None -> stream
        |> Observable.subscribe ( listener.Action)

    let task1 = async{
                    attachListener queueSubject stringListener |> ignore
                }
    let task2 = async{
                    attachListener queueSubject2 stringListener2 |> ignore
                }
    Async.Start task1
    Async.Start task2

    printfn "Starting"

    while true do ()

    cleanUpReader ()
    0 // return an integer exit code