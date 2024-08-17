using Blog.Domain;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Articles;

public class ArticlePost
{
    public static string Template => "/article";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromBody] ArticleRequest request, ApplicationDbContext context)
    {
        var article = new Article(request.Title, request.Content, request.CategoryId);

        if (!article.IsValid)
            return Results.BadRequest(article.Notifications);

        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();
        
        // TODO: Cadastrar tags caso não existam. Caso existam, apenas atribuir ao artigo.

        var category = await context.Categories.FindAsync(request.CategoryId);
        var categoryName = category != null ? category.Name : "";

        var response = new ArticleResponse(article.Id, article.Title, article.Content, categoryName);
        return Results.Created("/article", response);
    }
}
