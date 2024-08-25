using System.Text.Json;
using Blog.Infrastructure;
using Blog.UseCases.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
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
    public static async Task<IResult> Action(ApplicationDbContext context, IConnectionMultiplexer muxer, [FromQuery] int page = 1, [FromQuery] int rows = 10)
    {
        var _redis = muxer.GetDatabase();

        var keyName = $"articles:{page},{rows}";
        var redisResponseJson = await _redis.StringGetAsync(keyName);

        List<ArticleResponse> articleResponses = null;

        if (string.IsNullOrEmpty(redisResponseJson))
        {
            var articles = await context.Articles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .Skip((page - 1) * rows)
                .Take(rows)
                .ToListAsync();

            articleResponses = articles.Select(a =>
            {
                var tagsResponse = a.Tags.Select(t => new TagResponse(t.Id, t.Name)).ToList();
                return new ArticleResponse(a.Id, a.Title, a.Content, a.Category.Name, tagsResponse);
            }).ToList();

            var serializedArticles = JsonSerializer.Serialize(articleResponses);

            var setTask = _redis.StringSetAsync(keyName, serializedArticles);
            var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3 * 60)); //  3 minutes

            await Task.WhenAll(setTask, expireTask);
        }

        var response = redisResponseJson.HasValue
            ? JsonSerializer.Deserialize<List<ArticleResponse>>(redisResponseJson)
            : articleResponses;

        return Results.Ok(response);
    }
}
