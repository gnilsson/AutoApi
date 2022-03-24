using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AutoApi.Sample.Server.Database;

public class AutoApiEfDbContextFactory : IDesignTimeDbContextFactory<AutoApiEfDbContext>
{
    public AutoApiEfDbContext CreateDbContext(string[] args)
    {
        //todo add guardclause
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment is null) throw new InvalidOperationException("No environment specified");

        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AutoApi.Sample.Server"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AutoApiEfDbContext>();

        optionsBuilder.UseSqlServer(config.GetConnectionString("sqlConn"))
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging();

        return new AutoApiEfDbContext(optionsBuilder.Options);
    }
}
