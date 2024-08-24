using Blog.Domain;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Categories;

public class CategoryPost
{
    public static string Template => "/category";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action([FromBody] CategoryRequest categoryRequest, ApplicationDbContext context)
    {
        var category = new Category(name: categoryRequest.Name);

        if (!category.IsValid)
            return Results.BadRequest(category.Notifications);

        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();
        return Results.Created("/category", category.Id);
    }
}
