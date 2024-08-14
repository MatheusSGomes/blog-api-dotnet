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

    public static async Task<IResult> Action([FromRoute] Guid id, [FromBody] CategoryRequest request, ApplicationDbContext context)
    {
        var category = await context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
        category.Name = request.Name;
        await context.SaveChangesAsync();
        return Results.Ok(category);
    }
}
