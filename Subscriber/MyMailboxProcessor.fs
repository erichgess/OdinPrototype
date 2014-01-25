module MyMailboxProcessor

open MessageContracts

let printMailbox = [    MailboxProcessor.Start(
                                fun mbox ->
                                    let rec loop() = 
                                        async{
                                            let! msg = mbox.Receive()

                                            match msg with
                                            | TypeA(i) -> printfn "Mailbox: %f" i
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
                                            | TypeA(i) -> printfn "Mailbox: %f" i
                                            | TypeB(s) -> printfn "Mailbox: %s" s

                                            return! loop()
                                        }
                                    loop()
                                );]
