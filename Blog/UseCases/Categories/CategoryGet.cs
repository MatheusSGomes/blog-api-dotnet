using Blog.Exception;
using Blog.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.UseCases.Categories;

public class CategoryGet
{
    public static string Template => "/category/{id}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    public static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var category = await context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();

        if (category == null)
            return Results.NotFound(ResourceErrorMessages.CATEGORY_NOT_FOUND);

        var response = new CategoryResponse(Id: category.Id, Name: category.Name);
        return Results.Ok(response);
    }
}
