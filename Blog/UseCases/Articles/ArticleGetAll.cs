using Blog.Domain;
using Blog.Infrastructure;
using Blog.UseCases.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace Blog.UseCases.Articles;

[Authorize(Policy = "NameClaimPolicy")]
public class ArticleGetAll
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    /// <param name="context"></param>
    /// <param name="page"></param>
    /// <param name="rows"></param>
    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context, [FromQuery] int page = 1, [FromQuery] int rows = 10)
    {
        var articles = await context.Articles
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToListAsync();

        var response = articles.Select(a =>
        {
            var tags = a.Tags.Select(t => new TagResponse(t.Id, t.Name)).ToList();
            return new ArticleResponse(a.Id, a.Title, a.Content, a.Category.Name, tags);
        });

        return Results.Ok(response);
    }
}
