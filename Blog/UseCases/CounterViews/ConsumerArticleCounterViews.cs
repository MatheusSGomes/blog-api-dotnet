using Blog.Infrastructure;
using Blog.Infrastructure.Messaging;
using Microsoft.AspNetCore.Authorization;

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
