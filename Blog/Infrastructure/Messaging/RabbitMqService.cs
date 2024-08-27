using System.Text;
using RabbitMQ.Client;

namespace Blog.Infrastructure.Messaging;

public class RabbitMqService : IRabbitMqService
{
    public void SendMessage(string message)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "hello",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: "hello",
            basicProperties: null,
            body: body);
        
        Console.WriteLine($" [x] Sent: {message}");
    }

    public string ReceiveMessage()
    {
        throw new NotImplementedException();
    }
}
