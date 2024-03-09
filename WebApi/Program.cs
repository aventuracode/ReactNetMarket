using BusinessLogic.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApi;
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using (var scope = host.Services.CreateScope())
        {
            var service = scope.ServiceProvider;
            var loggerFactory = service.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = service.GetRequiredService<MarketDbContext>();
                await context.Database.MigrateAsync();
                await MarketDbContextData.CargarDataAsync(context,loggerFactory);
            }
            catch (Exception e)
            {

                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(e, "errores en el proceso de migracion");
            }
        }
        host.Run();

    }
    private static IHostBuilder CreateHostBuilder(string[] args)
     => Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}