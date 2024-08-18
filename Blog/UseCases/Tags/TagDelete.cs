using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagDelete
{
    public static string Template => "/tag/{id}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var tag = await context.Tags.FindAsync(id);

        if (tag == null)
            return Results.BadRequest(ResourceErrorMessages.TAG_NOT_FOUND);

        context.Remove(tag);
        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}
