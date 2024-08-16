using Blog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Blog.UseCases.Articles;

public class ArticleGetAll
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(ApplicationDbContext context)
    {
        var articles = context.Articles.Include(a => a.Category).ToList();

        var response = articles.Select(a => 
            new ArticleResponse(a.Title, a.Content, a.Category.Name));

        return Results.Ok(response);
    }
}
