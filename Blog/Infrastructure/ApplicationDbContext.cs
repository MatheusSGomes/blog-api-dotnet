using Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(@"Host=localhost;Port=5000;Username=docker;Password=docker;Database=blog;");
    }
}
