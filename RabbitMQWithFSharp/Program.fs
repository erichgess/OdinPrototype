// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open RabbitMQ.Client
open System.Text

let CreateConnectionFactory () = new ConnectionFactory()
let GetConnection (factory:ConnectionFactory) = factory.CreateConnection ()
let GetChannel (connection:IConnection) = connection.CreateModel()

let PublishMessage (channel:IModel) (message:string) =
    let encodedMessage = Encoding.UTF8.GetBytes(message)
    channel.BasicPublish ( "", "fsharp-queue", null, encodedMessage )

[<EntryPoint>]
let main argv = 
    let connectionFactory = CreateConnectionFactory ()
    let connection = GetConnection connectionFactory
    let channel = GetChannel connection

    channel.QueueDeclare( "fsharp-queue", false, false, false, null) |> ignore

    while true do
        PublishMessage channel "Hello"
        System.Threading.Thread.Sleep(5000)

    channel.Close()
    connection.Close()
    0 // return an integer exit code
