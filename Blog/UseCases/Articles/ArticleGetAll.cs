using Blog.Domain;
using Blog.Infrastructure;
using Blog.UseCases.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Blog.UseCases.Articles;

[Authorize(Policy = "NameClaimPolicy")]
public class ArticleGetAll
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context)
    {
        var articles = await context.Articles
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .ToListAsync();

        var response = articles.Select(a =>
        {
            var tags = a.Tags.Select(t => new TagResponse(t.Id, t.Name)).ToList();
            return new ArticleResponse(a.Id, a.Title, a.Content, a.Category.Name, tags);
        });

        return Results.Ok(response);
    }
}
