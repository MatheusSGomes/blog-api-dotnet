using System.Text;
using System.Text.Json;
using Blog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Blog.Infrastructure.Messaging;

public class IncrementCounterViewsArticle 
{
    public static void SendMessage(Guid articleId, int increment = 1)
    {
        var articleMessage = new ArticleMessageJson(articleId, increment);
        var articleMsgSerialized = JsonSerializer.Serialize(articleMessage);
        var body = Encoding.UTF8.GetBytes(articleMsgSerialized);

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "articleCounterViews",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: "articleCounterViews",
            basicProperties: null,
            body: body);

        Console.WriteLine($" [x] Enviado: {increment}");
    }

    public static void ReceiveMessage()
    {
        ApplicationDbContext context = new ApplicationDbContext();

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

            var jsonMsg = JsonSerializer.Deserialize<ArticleMessageJson>(message);

            // CounterViews counterViews = null;
            // counterViews = context.CounterViews.Where(cv => cv.ArticleId == jsonMsg.articleId).FirstOrDefault();
            //
            // if (counterViews == null)
            // {
            //     counterViews = new CounterViews
            //     {
            //         ArticleId = jsonMsg.articleId,
            //         Counter = 1
            //     };
            //     context.CounterViews.Add(counterViews);
            // }
            // else
            // {
            //     counterViews.Counter = counterViews.Counter += 1;
            //     context.CounterViews.Update(counterViews);
            // }
            //
            // context.SaveChanges();

            Console.WriteLine($" [x] Recebido: {message}");
        };

        channel1.BasicConsume(
            queue: "articleCounterViews",
            autoAck: true,
            consumer: consumer);
    }
}
