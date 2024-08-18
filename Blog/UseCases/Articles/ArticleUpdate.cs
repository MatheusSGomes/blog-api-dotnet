using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Articles;

public class ArticleUpdate
{
    public static string Template => "/article/{id}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    private static async Task<IResult> Action([FromRoute] Guid id, [FromBody] ArticleRequest request, ApplicationDbContext context)
    {
        var article = await context.Articles.FindAsync(id);

        if (article == null)
            return Results.NotFound(ResourceErrorMessages.ARTICLE_NOT_FOUND);

        var category = await context.Categories.FindAsync(request.CategoryId);

        if (category == null)
            return Results.NotFound(ResourceErrorMessages.CATEGORY_NOT_FOUND);

        article.Title = request.Title;
        article.Content = request.Content;
        article.CategoryId = request.CategoryId;

        var tags = context.Tags.Where(tag => request.Tags.Contains(tag.Id)).ToList();

        if (tags.Any())
            article.Tags = tags;

        await context.SaveChangesAsync();

        return Results.Ok();
    }
}
