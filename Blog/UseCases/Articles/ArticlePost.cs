using Blog.Domain;
using Blog.Infrastructure;
using Blog.UseCases.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Articles;

public class ArticlePost
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action([FromBody] ArticleRequest request, ApplicationDbContext context)
    {
        var article = new Article(request.Title, request.Content, request.CategoryId);

        if (!article.IsValid)
            return Results.BadRequest(article.Notifications);
        
        var tags = context.Tags.Where(tag => request.Tags.Contains(tag.Id)).ToList();

        if (tags.Any())
            article.Tags = tags;

        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();

        var category = await context.Categories.FindAsync(request.CategoryId);
        var categoryName = category != null ? category.Name : "";

        var tagsResponse = tags.Select(t => new TagResponse(t.Id, t.Name)).ToList();
        var response = new ArticleResponse(article.Id, article.Title, article.Content, categoryName, tagsResponse);
        return Results.Created("/article", response);
    }
}
