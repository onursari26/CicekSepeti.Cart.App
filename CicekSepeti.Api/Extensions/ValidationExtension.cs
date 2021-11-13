using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CicekSepeti.Api.Extensions
{
    public static class ValidationExtension
    {
        public static void AddFluentValidationEx(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                    return new BadRequestObjectResult(ResponseInfo<object>.Error(errors));
                };
            });
        }
    }
}
