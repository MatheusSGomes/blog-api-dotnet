using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Categories;

public class CategoryDelete
{
    public static string Template => "/category/{id}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var category = await context.Categories.FindAsync(id);

        if (category == null)
            return Results.NotFound(ResourceErrorMessages.CATEGORY_NOT_FOUND);
        
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}
