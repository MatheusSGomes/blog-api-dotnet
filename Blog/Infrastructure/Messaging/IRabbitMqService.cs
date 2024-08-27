namespace Blog.Infrastructure.Messaging;

public interface IRabbitMqService
{
    void SendMessage(string message);
    void ReceiveMessage();
}
