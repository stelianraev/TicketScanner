using Microsoft.EntityFrameworkCore;

namespace CheckIN.Common
{
    public static class Extensions
    {
        public static void DbMigration<T>(this IApplicationBuilder app) where T : DbContext
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<T>();
                context.Database.Migrate();
            }
        }
    }
}
