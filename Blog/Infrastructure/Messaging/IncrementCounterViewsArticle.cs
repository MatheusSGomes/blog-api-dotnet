using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Blog.Infrastructure.Messaging;

public class IncrementCounterViewsArticle 
{
    public static void SendMessage(int increment = 1)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "articleCounterViews",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(increment.ToString());

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: "articleCounterViews",
            basicProperties: null,
            body: body);

        Console.WriteLine($" [x] Enviado: {increment}");
    }

    public static void ReceiveMessage()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel1 = connection.CreateModel();

        channel1.QueueDeclare(
            queue: "articleCounterViews",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        Console.WriteLine(" [*] Waiting for messages.");

        var consumer = new EventingBasicConsumer(channel1);

        consumer.Received += (objectModel, deliverEventArgs) =>
        {
            var body = deliverEventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            Console.WriteLine($" [x] Recebido: {message}");
        };

        channel1.BasicConsume(
            queue: "articleCounterViews",
            autoAck: true,
            consumer: consumer);
    }
}
