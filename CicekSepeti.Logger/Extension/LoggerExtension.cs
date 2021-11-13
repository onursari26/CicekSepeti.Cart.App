using CicekSepeti.Logger.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CicekSepeti.Logger.Extension
{
    public static class LoggerExtension
    {
        public static IApplicationBuilder UseGlobalLogger(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalLogger>();
        }
    }
}
