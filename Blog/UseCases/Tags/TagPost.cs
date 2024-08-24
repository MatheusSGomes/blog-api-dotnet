using Blog.Domain;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagPost
{
    public static string Template => "/tag";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [AllowAnonymous]
    public static async Task<IResult> Action([FromBody] TagRequest request, ApplicationDbContext context)
    {
        var tag = new Tag(request.Name);

        if (!tag.IsValid)
            return Results.BadRequest(tag.Notifications);

        await context.Tags.AddAsync(tag);
        await context.SaveChangesAsync();

        return Results.Ok(new TagResponse(tag.Id, tag.Name));
    }
}
