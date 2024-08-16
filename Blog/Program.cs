using Blog.Infrastructure;
using Blog.UseCases.Articles;
using Blog.UseCases.Categories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryGet.Template, CategoryGet.Methods, CategoryGet.Handle);
app.MapMethods(CategoryUpdate.Template, CategoryUpdate.Methods, CategoryUpdate.Handle);
app.MapMethods(CategoryDelete.Template, CategoryDelete.Methods, CategoryDelete.Handle);

app.MapMethods(ArticlePost.Template, ArticlePost.Methods, ArticlePost.Handle);
app.MapMethods(ArticleGetAll.Template, ArticleGetAll.Methods, ArticleGetAll.Handle);
app.MapMethods(ArticleGetById.Template, ArticleGetById.Methods, ArticleGetById.Handle);

app.Run();
