using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Tags;

public class TagGetById
{
    public static string Template => "/tag/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    private static IResult Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var tag = context.Tags.Find(id);
        var response = new TagResponse(tag.Id, tag.Name);
        return Results.Ok(response);
    }
}
