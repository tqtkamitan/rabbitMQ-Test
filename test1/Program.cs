
using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

string exchangeName = "topic_logs";
//string queueName = "DemoListsQueue";
// Create a connection to the RabbitMQ server
var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
{
    // Create a channel to communicate with the server
    using (var channel = connection.CreateModel())
    {
        // Declare an exchange of type "topic" that will route messages based on the routing key pattern
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

        // Declare a temporary queue that will be deleted when the connection closes
        //var queueName = channel.QueueDeclare().QueueName;

        // Bind the queue to the exchange with the topic "demo.*"
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "demo.info");
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        // Create a consumer that will receive messages from the queue
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            // Get the message body as a string
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            Console.WriteLine(" [x] Received '{0}':'{1}'", routingKey, message);
        };

        // Start consuming messages from the queue
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine(" [*] Waiting for logs. To exit press CTRL+C");
        Console.ReadLine();
    }
}

//To run this example, you need to have RabbitMQ server running on your local machine. You also need to install the RabbitMQ.Client NuGet package in your project. You can run multiple instances of this program to create multiple consumers that will receive the same message from the "topic_logs" exchange. To send a message to the exchange, you can use another program like this:
