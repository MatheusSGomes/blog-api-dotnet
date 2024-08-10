namespace Blog.Domain;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Tag> Tags { get; set; }
}
