namespace RabbitMQ.FSharp

open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Text

module Client =
    let connectToRabbitMqServerAt address = 
        let factory = new ConnectionFactory(HostName = address)
        factory.CreateConnection()

    let openChannelOn (connection:IConnection) = connection.CreateModel()

    let private declareQueue (channel:IModel) queueName = channel.QueueDeclare( queueName, false, false, false, null )

    let private publishToQueue (channel:IModel) queueName (message:string) =
        let body = Encoding.UTF8.GetBytes(message)
        channel.BasicPublish("", queueName, null, body)
        
    let createQueueReader channel queue = 
        declareQueue channel queue |> ignore
        
        fun () -> 
            let ea = channel.BasicGet(queue, true)
            if ea <> null then
                let body = ea.Body
                let message = Encoding.UTF8.GetString(body)
                Some message
            else
                None

    let createQueueWriter channel queue =
        declareQueue channel queue |> ignore
        publishToQueue channel queue

    let createQueueConsumer channel queueName =
        let consumer = new QueueingBasicConsumer(channel) 
        channel.BasicConsume(queueName, true, consumer) |> ignore

        fun () ->
            let ea = consumer.Queue.Dequeue()
            let body = ea.Body
            Encoding.UTF8.GetString(body)
