using AutoApi.Sample.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoApi.Sample.Server.Database;

public class AutoApiEfDbContext : DbContext
{
    public AutoApiEfDbContext(DbContextOptions<AutoApiEfDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder builder) { }

    public DbSet<Blog> Blogs { get; set; } = default!;
    public DbSet<Post> Posts { get; set; } = default!;
    public DbSet<Author> Authors { get; set; } = default!;
}
