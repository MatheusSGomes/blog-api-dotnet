using Blog.Domain;
using Blog.Exception;
using Blog.Infrastructure;
using Blog.Infrastructure.Messaging;
using Blog.UseCases.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Articles;

public class ArticleGetById
{
    public static string Template => "/article/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(Guid id, ApplicationDbContext context)
    {
        var article = await context.Articles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null)
            return Results.NotFound(ResourceErrorMessages.ARTICLE_NOT_FOUND);

        var category = await context.Categories.FindAsync(article.CategoryId);

        string categoryName = category != null ? category.Name : "";

        var tags = article.Tags.Select(t => new TagResponse(t.Id, t.Name)).ToList();
        var response = new ArticleResponse(article.Id, article.Title, article.Content, categoryName, tags);

        IncrementCounterViewsArticle.SendMessage(article.Id);

        // article.CountViews += 1;
        // context.SaveChanges();

        return Results.Ok(response);
    }
}
