SimpleRabbitMQ
==============

A simple project in F# which sets up a RabbitMQ publisher and consumer in F#.


The application "Publisher" will write a message with the text "Hello" to a queue once every 5 seconds.

The application "Subscriber" receives messages sent by "Publisher" and writes the contents to the Console.
