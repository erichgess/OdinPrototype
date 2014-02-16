module MyMailboxProcessor

open MessageContracts

let typeAMailbox = MailboxProcessor.Start(
                        fun mbox ->
                            let rec loop() = 
                                async{
                                    let! msg = mbox.Receive()

                                    match msg with
                                    | TypeA(n, i) -> printfn "%s: %f" n i
                                    | _ -> printfn "Nothing"
                                    return! loop()
                                }
                            loop()
                        )

let typeBMailbox = MailboxProcessor.Start(
                        fun mbox ->
                            let rec loop() = 
                                async{
                                    let! msg = mbox.Receive()

                                    match msg with
                                    | TypeB(n) -> printfn "%s" n
                                    | _ -> printfn "Nothing"
                                    return! loop()
                                }
                            loop()
                        )

let typeFunctionMailbox = MailboxProcessor.Start(
                            fun mbox ->
                                let rec loop() = 
                                    async{
                                        let! msg = mbox.Receive()

                                        match msg with
                                        | TypeFunction(f) -> f ()
                                        | _ -> ()
                                        return! loop()
                                    }
                                loop()
                            )