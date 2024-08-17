using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Articles;

public class ArticleGetById
{
    public static string Template => "/article/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(Guid id, ApplicationDbContext context)
    {
        var article = await context.Articles.FindAsync(id);

        if (article == null)
            return Results.NotFound(ResourceErrorMessages.ARTICLE_NOT_FOUND);

        var category = await context.Categories.FindAsync(article.CategoryId);

        string categoryName = category != null ? category.Name : "";

        var response = new ArticleResponse(article.Id, article.Title, article.Content, categoryName);

        return Results.Ok(response);
    }
}
