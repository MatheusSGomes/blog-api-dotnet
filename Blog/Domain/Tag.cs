using Blog.Exception;
using Flunt.Notifications;
using Flunt.Validations;

namespace Blog.Domain;

public class Tag : Notifiable<Notification>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Article> Articles { get; } = [];

    public Tag(string name)
    {
        var contract = new Contract<Tag>()
            .IsNotNullOrEmpty(name, "name", ResourceErrorMessages.TAG_NULL_OR_EMPTY);
        
        AddNotifications(contract);

        Name = name;
    }
}
