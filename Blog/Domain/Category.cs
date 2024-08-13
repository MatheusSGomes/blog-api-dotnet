using Blog.Exception;
using Flunt.Notifications;
using Flunt.Validations;

namespace Blog.Domain;

public class Category: Notifiable<Notification>
{
    public Guid Id { get; set; }
    public string Name { get; private set; }

    public Category(string name)
    {
        var contract = new Contract<Category>()
            .IsNotNullOrEmpty(name, "name", ResourceErrorMessages.CATEGORY_NULL_OR_EMPTY);

        AddNotifications(contract);

        Name = name;
    }
}
