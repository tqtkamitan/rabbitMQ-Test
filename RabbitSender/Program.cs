using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri(uriString:"amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Sender App";

IConnection cnn = factory.CreateConnection();

IModel channel = cnn.CreateModel();

//string exchangeName = "DemoExhange";
string exchangeName = "DemoListExhanges";
string routingKey = "demo-routing-key";
//string queueName = "DemoQueue";
//string queueName = "DemoListsQueue";

//channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
//channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
//channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
//channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);

for (int i = 0; i < 60; i++)
{
    Console.WriteLine($"Sending Message {i}");
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(s: $"Message #{i}");
    channel.BasicPublish(exchangeName, routingKey, basicProperties: null, messageBodyBytes);
    Thread.Sleep(1000);
}

channel.Close();
cnn.Close();