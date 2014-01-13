module MyConsumer

open RabbitMQ.Client
open System.Text

type MyConsumer (model:IModel) =
    member this.Model = model
    interface IBasicConsumer with
        member this.get_Model() = model
        member this.HandleBasicCancel(consumerTag:string) = ()
        member this.HandleBasicCancelOk(consumerTag:string) = ()
        member this.HandleBasicConsumeOk(consumerTag:string) = ()
        member this.HandleModelShutdown (model:IModel, reason:ShutdownEventArgs) = ()
        member this.HandleBasicDeliver (consumerTag:string, deliveryTag:uint64, redelivered:bool, exchange:string, routingKey:string, properties:IBasicProperties, body:byte[]) = 
            if body <> null then
                let message = Encoding.UTF8.GetString(body)
                printfn "%s" message
            ()

        member this.add_ConsumerCancelled  (value:Events.ConsumerCancelledEventHandler) = ()
        member this.remove_ConsumerCancelled  (value:Events.ConsumerCancelledEventHandler) = ()