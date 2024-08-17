using Blog.Infrastructure;
using Blog.UseCases.Articles;
using Blog.UseCases.Categories;
using Blog.UseCases.Tags;
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
app.MapMethods(ArticleUpdate.Template, ArticleUpdate.Methods, ArticleUpdate.Handle);
app.MapMethods(ArticleDelete.Template, ArticleDelete.Methods, ArticleDelete.Handle);

app.MapMethods(TagPost.Template, TagPost.Methods, TagPost.Handle);
app.MapMethods(TagGetAll.Template, TagGetAll.Methods, TagGetAll.Handle);
app.MapMethods(TagGetById.Template, TagGetById.Methods, TagGetById.Handle);
app.MapMethods(TagUpdate.Template, TagUpdate.Methods, TagUpdate.Handle);
app.MapMethods(TagDelete.Template, TagDelete.Methods, TagDelete.Handle);

app.Run();
