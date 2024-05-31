using CheckIN.Services.Customer;

namespace CheckIN.Middleware
{
    public class CustomerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICustomerProvider customerProvider)
        {
            var host = context.Request.Host.Host;
            var tenant = host.Split('.')[0];
            var tenantId = GetCustomerIdFromSubdomain(tenant);

            customerProvider.SetCustomerId(tenantId);

            await _next(context);
        }

        private int GetCustomerIdFromSubdomain(string subdomain)
        {
            // Implement your logic to get tenant id from subdomain
            return 1; // for demo purposes
        }
    }

    public static class CustomerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomerMiddleware>();
        }
    }
}
