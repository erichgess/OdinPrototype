module MyMailboxProcessor

let printMailbox = MailboxProcessor.Start(
                                fun mbox ->
                                    let rec loop() = 
                                        async{
                                            let! msg = mbox.Receive()
                                            printfn "Mailbox: %s" msg
                                            return! loop()
                                        }
                                    loop()
                                )