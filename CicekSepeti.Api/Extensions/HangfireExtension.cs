using Hangfire;
using Hangfire.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CicekSepeti.Api.Extensions
{
    public static class HangfireExtension
    {
        public static void AddHangfireEx(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config.UseSQLiteStorage($"{configuration["HangFire:Connection"]}"));

            services.AddHangfireServer(config =>
            {
                config.WorkerCount = int.Parse(configuration["Hangfire:WorkerCount"]);
            });
        }
    }
}
