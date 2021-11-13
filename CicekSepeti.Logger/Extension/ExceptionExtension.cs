using CicekSepeti.Logger.Middleware;
using Microsoft.AspNetCore.Builder;

namespace CicekSepeti.Logger.Extension
{
    public static class ExceptionExtension
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalException>();
        }
    }
}
