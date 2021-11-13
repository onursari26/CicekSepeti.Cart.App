using CicekSepeti.Core.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CicekSepeti.Api.Extensions
{
    public static class RedisExtension
    {
        public static void AddRedisEx(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRedisService>(r => new RedisService(configuration));
        }
    }
}
