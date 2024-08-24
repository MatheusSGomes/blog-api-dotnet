using Blog.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Categories;

public class CategoryGetAll
{
    public static string Template => "/category";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action(ApplicationDbContext context, [FromQuery] int page = 1, [FromQuery] int rows = 5)
    {
        var categories = await context.Categories
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToListAsync();

        var response = categories.Select(c => new CategoryResponse(Id: c.Id, Name: c.Name));
        return Results.Ok(response);
    }
}
