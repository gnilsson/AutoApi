using AutoApi.Sample.Shared.Entities;
using GN.Toolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AutoApi.Sample.Server.Database;

public class AutoApiEfDbContext : DbContext
{
    public AutoApiEfDbContext(DbContextOptions<AutoApiEfDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder builder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var identifierConverter = new ValueConverter<Identifier, Guid>(
            s => s,
            t => new Identifier(t));

        modelBuilder.Entity<Blog>(x =>
        {
            x.Property(x => x.Id).HasConversion(identifierConverter);
            x.Property(x => x.BlogCategory).HasConversion<string>();
        });

        modelBuilder.Entity<Post>(x =>
        {
            x.Property(c => c.Id).HasConversion(identifierConverter);
            x.Property(c => c.BlogId).HasConversion(identifierConverter);
        });

        modelBuilder.Entity<Author>(x =>
        {
            x.Property(x => x.Id).HasConversion(identifierConverter);
            x.Property(x => x.Profession).HasConversion<string>();
        });
    }

    public DbSet<Blog> Blogs { get; init; } = default!;
    public DbSet<Post> Posts { get; init; } = default!;
    public DbSet<Author> Authors { get; init; } = default!;
}
