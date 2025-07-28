using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new Uri(uriString: "amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Receiver2 App";

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
var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queueName, exchangeName, routingKey, arguments: null);
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
    var body = args.Body.ToArray();

    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine("Message receives: " + message);

    channel.BasicAck(args.DeliveryTag, multiple: false);
};

string consumertag = channel.BasicConsume(queueName, autoAck: false, consumer);

Console.ReadLine();

channel.BasicCancel(consumertag);
channel.Close();
cnn.Close();