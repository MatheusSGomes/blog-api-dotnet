using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagUpdate
{
    public static string Template => "/tag/{id}";
    public static string[] Methods => new string[] { HttpMethod.Put.ToString() };
    public static Delegate Handle => Action;

    private static async Task<IResult> Action([FromRoute] Guid id, [FromBody] TagRequest request, ApplicationDbContext context)
    {
        var tag = await context.Tags.FindAsync(id);

        if (tag == null)
            return Results.NotFound(ResourceErrorMessages.TAG_NOT_FOUND);
        
        if (!tag.IsValid)
            return Results.BadRequest(tag.Notifications);

        tag.Name = request.Name;
        await context.SaveChangesAsync();

        return Results.Ok(tag.Id);
    }
}
