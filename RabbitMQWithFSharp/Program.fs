// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text

[<EntryPoint>]
let main argv = 
    let connectionFactory = new ConnectionFactory()
    let connection = connectionFactory.CreateConnection()
    let channel = connection.CreateModel()

    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore

    let message = Encoding.UTF8.GetBytes("Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.Now is the time for all good men to come to the aid of their country.")
    channel.BasicPublish("", "fsharp-queue", null, message);

    let messages = { 1 .. 1000 } |> Seq.map( fun i -> i.ToString() )
    messages |> Seq.map (fun m -> Encoding.UTF8.GetBytes(m))
             |> Seq.iter (fun m -> channel.BasicPublish("", "fsharp-queue", null, message); )

    channel.Close()
    connection.Close()
    0 // return an integer exit code
