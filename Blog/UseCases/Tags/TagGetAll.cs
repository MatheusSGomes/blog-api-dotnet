using Blog.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Tags;

public class TagGetAll
{
    public static string Template => "/tag";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    /// <param name="context"></param>
    /// <param name="page"></param>
    /// <param name="rows"></param>

    [AllowAnonymous]
    private static async Task<IResult> Action(ApplicationDbContext context, [FromQuery] int page = 1, [FromQuery] int rows = 10)
    {
        var tags = await context.Tags
            .Skip((page - 1) * rows)
            .Take(rows)
            .ToListAsync();

        var response = tags.Select(tag => new TagResponse(tag.Id, tag.Name));
        return Results.Ok(response);
    }
}
