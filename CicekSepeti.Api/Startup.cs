using CicekSepeti.Api.Extensions;
using CicekSepeti.Core.Concrete;
using CicekSepeti.Core.Context;
using CicekSepeti.Core.Interfaces;
using CicekSepeti.Logger;
using CicekSepeti.Logger.Extension;
using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System.Globalization;

namespace CicekSepeti.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            Log.Logger = new LoggerConfiguration()
                     .ReadFrom.Configuration(Configuration)
                     .CreateLogger();

            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");

            services.AddHangfireEx(Configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddIdentityEx();

            services.Configure<TokenOption>(Configuration.GetSection("TokenOption"));

            services.AddDbContext<AplicationContext>(ops => ops.UseInMemoryDatabase("AplicationDb"));

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork<AplicationContext>));

            services.AddAuthenticationEx(Configuration);

            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            }).AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()))
            .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
            .AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.AddFluentValidationEx();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddRedisEx(Configuration);

            services.BusinessServiceEx();

            services.AddHttpClient();

            services.AddSwaggerEx();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IDiscountService discountService)
        {
            app.UseHangfireDashboard();

            loggerFactory.AddSerilog();

            app.UseCors("CorsPolicy");

            app.UseCookiePolicy();

            app.UseStaticFiles();

            app.UseRouting();

            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });

            app.UseGlobalLogger();
            app.UseGlobalException();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            RecurringJob.AddOrUpdate(() => discountService.PassiveDiscount(), Configuration["HangFire:PassiveDiscountCronExpression"]);
        }
    }
}
