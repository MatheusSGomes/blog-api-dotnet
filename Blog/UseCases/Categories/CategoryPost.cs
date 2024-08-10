namespace Blog.UseCases.Categories;

public class CategoryPost
{
    public static string Template => "/category";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action()
    {
        var response = "Chegou aqui!";
        return Results.Created("/category", response);
    }
}
