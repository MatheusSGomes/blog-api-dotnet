using System.Text;
using Blog.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Blog.Infrastructure.Messaging;

public class RabbitMQConsumer
{
    private readonly ApplicationDbContext _context;

    public RabbitMQConsumer(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Start()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel1 = connection.CreateModel();

        channel1.QueueDeclare(
            queue: "articlesQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new EventingBasicConsumer(channel1);

        consumer.Received += async void (objectModel, deliverEventArgs) =>
        {
            var body = deliverEventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            await IncrementArticleViewAsync(message);
        };

        channel1.BasicConsume(
            queue: "articleCounterViews",
            autoAck: true,
            consumer: consumer);
        
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }

    private async Task IncrementArticleViewAsync(string articleId)
    {
        var parseId = Guid.Parse(articleId);
        var article = await _context.Articles.FindAsync(parseId);

        if (article != null)
        {
            article.CountViews++;
            _context.Articles.Update(article);
            _context.SaveChanges();
        }
    }
}
