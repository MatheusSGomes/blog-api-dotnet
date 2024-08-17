using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagDelete
{
    public static string Template => "/tag/{id}";
    public static string[] Methods => new string[] { HttpMethod.Delete.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var tag = context.Tags.Find(id);

        if (tag == null)
            return Results.BadRequest(ResourceErrorMessages.TAG_NOT_FOUND);

        context.Remove(tag);
        context.SaveChanges();

        return Results.NoContent();
    }
}
