using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Articles;

public class ArticleDelete
{
    public static string Template => "/article/{id}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var article = await context.Articles.FindAsync(id);

        if (article == null)
            return Results.NotFound(ResourceErrorMessages.ARTICLE_NOT_FOUND);

        context.Articles.Remove(article);
        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}
