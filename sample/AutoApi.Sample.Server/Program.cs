using AutoApi.Sample.Server.Database;
using Microsoft.EntityFrameworkCore;

namespace AutoApi.Sample.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AutoApiEfDbContext>();
        await context.Database.MigrateAsync();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
