using Blog.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Categories;

public class CategoryGetAll
{
    public static string Template => "/category";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context)
    {
        var categories = await context.Categories.ToListAsync();
        var response = categories.Select(c => new CategoryResponse(Id: c.Id, Name: c.Name));
        return Results.Ok(response);
    }
}
