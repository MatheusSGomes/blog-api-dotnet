using Blog.Domain;
using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public ApplicationDbContext() { }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base (options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<Notification>();
        
        modelBuilder.Entity<Article>()
            .Property(p => p.Title)
            .IsRequired();

        modelBuilder.Entity<Article>()
            .Property(p => p.Content)
            .IsRequired();

        modelBuilder.Entity<Category>()
            .Property(p => p.Name)
            .IsRequired();

        modelBuilder.Entity<Article>()
            .HasMany(article => article.Tags)
            .WithMany(tag => tag.Articles)
            .UsingEntity(x => x.ToTable("ArticleTag"));

        modelBuilder.Entity<Article>()
            .Property(a => a.CountViews)
            .HasDefaultValue(0);
    }
}
