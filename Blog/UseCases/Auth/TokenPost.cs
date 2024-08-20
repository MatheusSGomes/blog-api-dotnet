using System.Security.Claims;
using Blog.Exception;
using Microsoft.AspNetCore.Identity;

namespace Blog.UseCases.Auth;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(LoginRequest request, UserManager<IdentityUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return Results.BadRequest(ResourceErrorMessages.PASSWORD_INVALID);

        var checkPass = await userManager.CheckPasswordAsync(user, request.Password);

        if (checkPass == false)
            return Results.BadRequest(ResourceErrorMessages.PASSWORD_INVALID);

        var userClaims = await userManager.GetClaimsAsync(user);

        var aditionalClaims = new Claim[]
        {
            new Claim(ClaimTypes.Email, request.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var subject = new ClaimsIdentity(aditionalClaims);
        subject.AddClaims(userClaims);

        return Results.Ok(user);
    }
}
