module StreamListener

/// This type listens to a stream of events and pipes them into a mailbox processor, which can then act upon those events.
/// When using this type, you specify two things:  1. a sequence of IObservable queries which are used to query the incoming
/// stream of events and 2. a MailboxProcessor which will subscribe to the observable query you define.
open System

type StreamListener<'a> = { Query: 'a IObservable; Action: 'a MailboxProcessor}