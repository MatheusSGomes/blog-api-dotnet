using Blog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base (options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Article>()
            .Property(p => p.Title)
            .IsRequired();

        modelBuilder.Entity<Article>()
            .Property(p => p.Content)
            .IsRequired();

        modelBuilder.Entity<Category>()
            .Property(p => p.Name)
            .IsRequired();
    }
}
