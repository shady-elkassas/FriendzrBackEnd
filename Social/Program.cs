using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Social.Entity.DBContext;
using Social.Sercices;
using Social.Services.Services;
using System;
using System.IO;

namespace Social
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
.AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
.AddEnvironmentVariables()
.Build();
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(Configuration)
             .CreateLogger();
            try
            {
                Log.Information("Application Starting.##############3");
                Log.Warning("Application Starting warning");

                var host = CreateWebHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        var context = services.GetRequiredService<AuthDBContext>();
                        context.Database.Migrate(); // apply all migrations

                        var applicationUserService = services.GetRequiredService<IUserService>();
                        applicationUserService.InitializeSuperAdminAccount().Wait();
                        //SeedData.Initialize(services); // Insert default data
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }
                //CreateWebHostBuilder(args).Build().Run();
                host.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.######333");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            //CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args).UseSerilog()
                    .UseStartup<Startup>().ConfigureServices(services =>
                   services.AddHostedService<DerivedBackgroundPrinter>());
    }
}
