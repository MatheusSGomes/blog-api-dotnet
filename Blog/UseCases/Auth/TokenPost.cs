using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Exception;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Blog.UseCases.Auth;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action(LoginRequest request, UserManager<IdentityUser> userManager, IConfiguration configuration)
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

        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);

        var symetricSecurityKey = new SymmetricSecurityKey(key);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Audience = "Blog",
            Issuer = "Issuer",
            Expires = DateTime.UtcNow.AddDays(2),
            SigningCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHanlder = new JwtSecurityTokenHandler();
        var token = tokenHanlder.CreateToken(tokenDescriptor);

        return Results.Ok(user);
    }
}
