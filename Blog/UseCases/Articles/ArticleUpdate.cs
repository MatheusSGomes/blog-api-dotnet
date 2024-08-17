using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Articles;

public class ArticleUpdate
{
    public static string Template => "/article/{id}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    private static IResult Action([FromRoute] Guid id, [FromBody] ArticleRequest request, ApplicationDbContext context)
    {
        var article = context.Articles.Find(id);

        if (article == null)
            return Results.BadRequest(ResourceErrorMessages.ARTICLE_NOT_FOUND);

        var category = context.Categories.Find(request.CategoryId);

        if (category == null)
            return Results.BadRequest(ResourceErrorMessages.CATEGORY_NOT_FOUND);

        article.Title = request.Title;
        article.Content = request.Content;
        article.CategoryId = request.CategoryId;

        context.SaveChanges();
        
        return Results.Ok();
    }
}
