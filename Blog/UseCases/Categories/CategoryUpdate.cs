using Blog.Domain;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Categories;

public class CategoryUpdate
{
    public static string Template => "/category/{id}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid id, [FromBody] CategoryRequest request, ApplicationDbContext context)
    {
        var category = context.Categories.Where(c => c.Id == id).FirstOrDefault();
        category.Name = request.Name;
        context.SaveChanges();
        return Results.Ok(category);
    }
}
