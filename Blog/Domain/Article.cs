using Flunt.Notifications;
using Flunt.Validations;

namespace Blog.Domain;

public class Article : Notifiable<Notification>
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; } = [];

    public Article(string title, string content, Guid categoryId)
    {
        var contract = new Contract<Article>()
            .IsNotNullOrEmpty(title, "title", "Título não pode ficar em branco")
            .IsNotNullOrEmpty(content, "content", "Conteúdo não pode ficar em branco")
            .IsNotNullOrEmpty(categoryId.ToString(), "CategoryId", "Selecione pelo menos 1 categoria");

        AddNotifications(contract.Notifications);

        Title = title;
        Content = content;
        CategoryId = categoryId;
    }
}
