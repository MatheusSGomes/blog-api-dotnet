using Blog.Domain;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Articles;

public class ArticlePost
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromBody] ArticleRequest request, ApplicationDbContext context)
    {
        var article = new Article
        {
            Title = request.Title,
            Content = request.Content,
            CategoryId = request.CategoryId,
        };
        context.Articles.Add(article);
        context.SaveChanges();
        return Results.Created("/article", article);
    }
}
