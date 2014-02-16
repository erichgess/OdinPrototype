module MyConsumer

open RabbitMQ.Client
open System
open System.Text
open MyMailboxProcessor
open MessageContracts
open System.Reactive.Subjects


type MyConsumer (model:IModel) =
    let mutable model = model
    let mutable subject = new Subject<Message>()

    member this.Subject with get() = subject and set(value) = subject <- value

    interface IBasicConsumer with
        member this.get_Model() = model
        member this.HandleBasicCancel(consumerTag:string) = ()
        member this.HandleBasicCancelOk(consumerTag:string) = ()
        member this.HandleBasicConsumeOk(consumerTag:string) = ()
        member this.HandleModelShutdown (model:IModel, reason:ShutdownEventArgs) = ()
        member this.HandleBasicDeliver (consumerTag:string, deliveryTag:uint64, redelivered:bool, exchange:string, routingKey:string, properties:IBasicProperties, body:byte[]) = 
            if body <> null then
                let message = Message.Decode(body)
                printfn "\t\tFrom Queue: %A" message
                subject.OnNext(message)
            ()

        member this.add_ConsumerCancelled  (value:Events.ConsumerCancelledEventHandler) = ()
        member this.remove_ConsumerCancelled  (value:Events.ConsumerCancelledEventHandler) = ()