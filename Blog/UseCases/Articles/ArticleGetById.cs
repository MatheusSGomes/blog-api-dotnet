using Blog.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Articles;

public class ArticleGetById
{
    public static string Template => "/article/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(Guid id, ApplicationDbContext context)
    {
        var article = context.Articles.Find(id);
        var category = context.Categories.Find(article.CategoryId);
        string categoryName = category != null ? category.Name : "";
        var response = new ArticleResponse(article.Title, article.Content, categoryName);
        return Results.Ok(response);
    }
}
