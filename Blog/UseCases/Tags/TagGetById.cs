using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagGetById
{
    public static string Template => "/tag/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    private static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var tag = await context.Tags.FindAsync(id);

        if (tag == null)
            return Results.NotFound(ResourceErrorMessages.TAG_NOT_FOUND);
        
        var response = new TagResponse(tag.Id, tag.Name);
        return Results.Ok(response);
    }
}
