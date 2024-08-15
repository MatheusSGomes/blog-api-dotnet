namespace Blog.UseCases.Articles;

public record ArticleRequest(string Title, string Content, Guid CategoryId, List<Guid> Tags);
