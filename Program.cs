using CheckIN.Configuration;
using CheckIN.Data.Model;
using CheckIN.Middleware;
using CheckIN.Services;
using CheckIN.Services.Customer;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using System.Security.Claims;

namespace CheckIN
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            var app = builder.Build();

            Configure(app, builder.Environment);

            var logger = app.Services.GetRequiredService<ILogger<TiToService>>();
            logger.LogInformation("TitoService starting...");

            await app.RunAsync();

            logger.LogError("TitoService stopped!");

        }
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Add services to the container.
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddSimpleConsole(options =>
                {
                    options.SingleLine = false;
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                });
            }
            builder.Services.Configure<TiToConfiguration>(builder.Configuration.GetSection("Tito"));

            var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection") ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddDefaultIdentity<User>(options =>
            //{
            //    options.SignIn.RequireConfirmedAccount = false;
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequiredLength = 0;
            //    options.ClaimsIdentity.RoleClaimType = "role";
            //})
            //    .AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();


            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 0;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
                options.Lockout.MaxFailedAccessAttempts = 0;
                options.Lockout.AllowedForNewUsers = false;
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
            });


            builder.Services.AddControllersWithViews();
            //    options =>
            //{
            //    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
            //});

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<Common>();
            builder.Services.AddSingleton<ITiToService, TiToService>();
            builder.Services.AddScoped<ICustomerProvider, CustomerProvider>();

        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHsts();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //custom middleware
            //app.UseCustomerMiddleware();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Entry}/{id?}");
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        //pattern: "{controller=Home}/{action=Index}/{id?}",
            //        pattern: "{controller}/{action}/{id}",
            //        defaults: new
            //        {
            //            controller = "Account",
            //            action = "Entry",
            //            id = UrlParameter.Optional,
            //        });
            //});
        }


        //future
        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}