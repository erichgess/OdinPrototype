module MyMailboxProcessor

open MessageContracts

let typeAMailbox = MailboxProcessor.Start(
                        fun mbox ->
                            let rec loop() = 
                                async{
                                    let! msg = mbox.Receive()

                                    match msg with
                                    | DataSet(m) when m.ContainsKey "%CPU" -> printfn "%%CPU: %s" (m.["%CPU"])
                                    | _ -> printfn "Nothing"
                                    return! loop()
                                }
                            loop()
                        )