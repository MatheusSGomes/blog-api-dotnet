using Blog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Blog.UseCases.Articles;

public class ArticleGetAll
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(ApplicationDbContext context)
    {
        var articles = await context.Articles.Include(a => a.Category).ToListAsync();

        var response = articles.Select(a => 
            new ArticleResponse(a.Id, a.Title, a.Content, a.Category.Name));

        return Results.Ok(response);
    }
}
