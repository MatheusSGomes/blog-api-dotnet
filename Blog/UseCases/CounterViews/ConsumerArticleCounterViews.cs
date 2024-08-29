using System.Text;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Blog.CounterViews;

public class ConsumerArticleCounterViews
{
    public static string Template => "/consumer";
    public static string[] Methods => new string[] { HttpMethods.Post.ToString() };
    public static Delegate Handle => ReceiveMessage;

    [AllowAnonymous]
    public static void ReceiveMessage(ApplicationDbContext context)
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

        consumer.Received += async void (objectModel, deliverEventArgs) =>
        {
            // TODO: Ajuste problema "Cannot access a disposed context instance."
            var body = deliverEventArgs.Body.ToArray();
            var articleId = Encoding.UTF8.GetString(body);

            var article = await context.Articles.FindAsync(articleId);

            if (article != null)
            {
                article.CountViews += 1;
                context.SaveChanges();
            }

            Console.WriteLine($" [x] Recebido: {articleId}");
        };

        channel1.BasicConsume(
            queue: "articleCounterViews",
            autoAck: true,
            consumer: consumer);
    }
}
