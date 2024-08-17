using Blog.Infrastructure;

namespace Blog.UseCases.Tags;

public class TagGetAll
{
    public static string Template => "/tag";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    private static IResult Action(ApplicationDbContext context)
    {
        var tags = context.Tags.ToList();
        var response = tags.Select(tag => new TagResponse(tag.Id, tag.Name));
        return Results.Ok(response);
    }
}
