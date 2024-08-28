namespace Blog.Infrastructure.Messaging;

public record ArticleMessageJson(Guid articleId, int increment);
