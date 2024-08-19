using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.UseCases.Writers;

public class CreateWriter
{
    public static string Template => "/create-writer";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromBody] WriterRequest request, UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Results.BadRequest(result.Errors);

        var userClaims = new List<Claim>
        {
            new Claim("Name", request.Name),
        };

        var claimResult = await userManager.AddClaimsAsync(user, userClaims);

        if (!claimResult.Succeeded)
            return Results.BadRequest(claimResult.Errors);

        return Results.Created($"/create-writer/{user.Id}", user.Id);
    }
}
