using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Blog.Infrastructure;
using Blog.UseCases.Articles;
using Blog.UseCases.Auth;
using Blog.UseCases.Categories;
using Blog.UseCases.Tags;
using Blog.UseCases.Writers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Blog API", Version = "v1"});

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, adicione o bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var configuration = builder.Configuration;
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// object cycle
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 3;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(authenticationOptions =>
{
    authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtBearerOptions =>
{
    var symetricSecurityKey = Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]);

    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(symetricSecurityKey),
    };
});

builder.Services.AddAuthorization(authorizationOptions =>
{
    // Todas as rotas precisam ser autenticadas
    authorizationOptions.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
    
    authorizationOptions.AddPolicy("NameClaimPolicy", policy =>
    {
        policy
            .RequireAuthenticatedUser()
            /*.Requirements.Add(new NameClaimRequirement("Matheus 4"))*/;
    });
});

builder.Services.AddRateLimiter(_ => 
    _.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(20);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    }).OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            await context.HttpContext.Response.WriteAsync(
                $"Too many requests. Please, try again after {retryAfter.TotalMinutes} minute(s). ",
                cancellationToken: token);
        }
        else
        {
            await context.HttpContext.Response.WriteAsync(
                "Too many requests. Please, try again. ", cancellationToken: token);
        }
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

app.MapMethods(CreateWriter.Template, CreateWriter.Methods, CreateWriter.Handle);

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryGet.Template, CategoryGet.Methods, CategoryGet.Handle);
app.MapMethods(CategoryUpdate.Template, CategoryUpdate.Methods, CategoryUpdate.Handle);
app.MapMethods(CategoryDelete.Template, CategoryDelete.Methods, CategoryDelete.Handle);

app.MapMethods(ArticlePost.Template, ArticlePost.Methods, ArticlePost.Handle);
app.MapMethods(ArticleGetAll.Template, ArticleGetAll.Methods, ArticleGetAll.Handle);
app.MapMethods(ArticleGetById.Template, ArticleGetById.Methods, ArticleGetById.Handle);
app.MapMethods(ArticleUpdate.Template, ArticleUpdate.Methods, ArticleUpdate.Handle);
app.MapMethods(ArticleDelete.Template, ArticleDelete.Methods, ArticleDelete.Handle);

app.MapMethods(TagPost.Template, TagPost.Methods, TagPost.Handle);
app.MapMethods(TagGetAll.Template, TagGetAll.Methods, TagGetAll.Handle);
app.MapMethods(TagGetById.Template, TagGetById.Methods, TagGetById.Handle);
app.MapMethods(TagUpdate.Template, TagUpdate.Methods, TagUpdate.Handle);
app.MapMethods(TagDelete.Template, TagDelete.Methods, TagDelete.Handle);

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext httpContext) =>
{
    var error = httpContext.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error.InnerException is NpgsqlException)
            return Results.Problem(title: "Database out", statusCode: 500);
    }

    return Results.Problem(title: "Ocorreu um erro", statusCode: 500);
});

app.UseRateLimiter();

app.MapGet("/ratelimited", [AllowAnonymous] () =>
    {
        var response = "Resposta gerada em: " + DateTime.UtcNow.ToString();
        return response;
    })
    .WithName("RateLimited")
    .RequireRateLimiting("fixed")
    .WithOpenApi();

app.Run();


public class NameClaimRequirement : IAuthorizationRequirement
{
    public string Name { get; }

    public NameClaimRequirement(string name)
    {
        Name = name;
    }
}

public class NameClaimRequirementHandler : AuthorizationHandler<NameClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        NameClaimRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "Name"))
        {
            var user = context.User.FindFirst(c => c.Type == "Name")?.Value;

            if (user == "Matheus 4")
            {
                context.Succeed(requirement);
            }
        }
        return Task.CompletedTask;
    }
}
