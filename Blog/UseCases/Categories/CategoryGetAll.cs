using Blog.Infrastructure;

namespace Blog.UseCases.Categories;

public class CategoryGetAll
{
    public static string Template => "/category";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action(ApplicationDbContext context)
    {
        var categories = context.Categories.ToList();
        var response = categories.Select(c => new CategoryResponse(Id: c.Id, Name: c.Name));
        return Results.Ok(response);
    }
}
