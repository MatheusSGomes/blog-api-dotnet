namespace Blog.UseCases.Articles;

public record ArticleResponse(Guid id, string Title, string Content, string CategoryName);
