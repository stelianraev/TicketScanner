using CheckIN.Configuration;
using CheckIN.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using System.Reflection;

namespace CheckIN
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            var app = builder.Build();

            Configure(app);

            var logger = app.Services.GetRequiredService<ILogger<TiToService>>();
            logger.LogInformation("MediaEncoderService starting...");

            await app.RunAsync();

            logger.LogError("MediaEncoderService stopped!");

        }
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            //IConfiguration configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .Build();

            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddSimpleConsole(options =>
                {
                    options.SingleLine = false;
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                });
            }

            builder.Services.Configure<TiToConfiguration>(builder.Configuration.GetSection("Tito"));

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<ITiToService, TiToService>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}