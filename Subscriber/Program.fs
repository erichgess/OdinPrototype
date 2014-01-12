// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text

[<EntryPoint>]
let main argv = 
    let connectionFactory = new ConnectionFactory()
    let connection = connectionFactory.CreateConnection()
    let channel = connection.CreateModel()

    channel.QueueDeclare("fsharp-queue", false, false, false, null)

    let result = channel.BasicGet("fsharp-queue", true)

    if result <> null then
        let message = Encoding.UTF8.GetString(result.Body)
        printfn "%s" message

    channel.Close()
    connection.Close()

    0 // return an integer exit code
