using Blog.Domain;
using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Categories;

public class CategoryPost
{
    public static string Template => "/category";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromBody] CategoryRequest categoryRequest, ApplicationDbContext context)
    {
        var category = new Category(name: categoryRequest.Name);

        if (!category.IsValid)
            return Results.BadRequest(category.Notifications);

        context.Categories.Add(category);
        context.SaveChanges();
        return Results.Created("/category", category.Id);
    }
}
