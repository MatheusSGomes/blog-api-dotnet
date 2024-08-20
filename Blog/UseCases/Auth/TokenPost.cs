namespace Blog.UseCases.Auth;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static IResult Action()
    {

        return Results.Ok();
    }
}
