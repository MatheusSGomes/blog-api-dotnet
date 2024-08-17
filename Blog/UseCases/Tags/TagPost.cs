using Blog.Domain;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagPost
{
    public static string Template => "/tag";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action([FromBody] TagRequest request, ApplicationDbContext context)
    {
        var tag = new Tag(request.Name);

        if (!tag.IsValid)
            return Results.BadRequest(tag.Notifications);

        context.Tags.Add(tag);
        context.SaveChanges();

        return Results.Ok(new TagResponse(tag.Id, tag.Name));
    }
}
