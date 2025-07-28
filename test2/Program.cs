
using System;
using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;


// Create a connection to the RabbitMQ server
ConnectionFactory factory = new();
factory.Uri = new Uri(uriString: "amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Sender App";
string exchangeName = "topic_logs";
//string queueName = "DemoListsQueue";

using (var connection = factory.CreateConnection())
{
    // Create a channel to communicate with the server
    using (var channel = connection.CreateModel())
    {
        // Declare an exchange of type "topic" that will route messages based on the routing key pattern
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

        // Get the routing key and the message to send from the command line arguments or use a default one
        var routingKey = "demo.info";
        //var message = messages.Length > 1 ? string.Join(" ", messages.Skip(1).ToArray()) : "Hello World!";
        //channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        //channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);

        for (int i = 0; i < 60; i++)
        {
            var body = Encoding.UTF8.GetBytes(s: $"Message #{i}");

            // Publish the message to the exchange with the routing key
            channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, i);
            Thread.Sleep(500);
        }
    }
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
