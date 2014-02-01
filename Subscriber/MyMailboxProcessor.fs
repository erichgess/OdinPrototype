module MyMailboxProcessor

open MessageContracts

let printMailbox = [    MailboxProcessor.Start(
                                fun mbox ->
                                    let rec loop() = 
                                        async{
                                            let! msg = mbox.Receive()

                                            match msg with
                                            | TypeA(n, i) -> printfn "%s: %f" n i
                                            | TypeB(s) -> printfn "Mailbox: %s" s
                                            return! loop()
                                        }
                                    loop()
                                );
                        MailboxProcessor.Start(
                                fun mbox ->
                                    let rec loop() = 
                                        async{
                                            let! msg = mbox.Receive()

                                            match msg with
                                            | TypeA(n, i) -> printfn "%s: %f" n i
                                            | TypeB(s) -> printfn "Mailbox: %s" s

                                            return! loop()
                                        }
                                    loop()
                                );]
