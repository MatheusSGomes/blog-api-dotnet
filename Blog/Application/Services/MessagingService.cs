using System.Text;
using Blog.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace Blog.Application.Services;

public class MessagingService
{
    private readonly IRabbitMqService _rabbitMqService;

    public MessagingService(IRabbitMqService rabbitMqService)
    {
        _rabbitMqService = rabbitMqService;
    }

    public void SendMessageToQueue(string message)
    {
        _rabbitMqService.SendMessage(message);
    }

    public string ReceiveMessageFromQueue()
    {
        // return _rabbitMqService.ReceiveMessage();
        return "A fazer";
    }
}
