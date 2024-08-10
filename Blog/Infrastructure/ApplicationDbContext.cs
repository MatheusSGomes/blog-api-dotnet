using Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base (options) { }
}
