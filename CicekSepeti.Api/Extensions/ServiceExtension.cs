using CicekSepeti.Service.Concrete;
using CicekSepeti.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CicekSepeti.Api.Extensions
{
    public static class ServiceExtension
    {
        public static void BusinessServiceEx(this IServiceCollection services)
        {
            services.AddScoped<IDatabaseInitializer, DatabaseInitializerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IDiscountService, DiscountService>();
        }
    }
}
