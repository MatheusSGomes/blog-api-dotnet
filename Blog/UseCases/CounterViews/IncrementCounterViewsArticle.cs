using System.Text;
using System.Text.Json;
using Blog.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Blog.Infrastructure.Messaging;

public class IncrementCounterViewsArticle 
{
    public static void SendMessage(Guid articleId)
    {
        var body = Encoding.UTF8.GetBytes(articleId.ToString());

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

        Console.WriteLine($" [x] Enviado artigo: {articleId}");
    }
}
