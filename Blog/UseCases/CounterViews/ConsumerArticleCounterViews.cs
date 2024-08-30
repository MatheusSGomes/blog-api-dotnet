using System.Text;
using Blog.Infrastructure;
using Blog.UseCases.CounterViews;
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
        var consumer = new RabbitMQConsumer(context);
        consumer.Start();
    }
}
