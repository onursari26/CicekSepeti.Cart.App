using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CicekSepeti.Logger.Middleware
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;

        public GlobalException(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            if (context.Request.Method.ToUpper() == "POST" || context.Request.Method.ToUpper() == "PUT" || context.Request.Method.ToUpper() == "DELETE")
            {
                statusCode = HttpStatusCode.BadRequest;
            }

            context.Response.StatusCode = (int)statusCode;

            var result = JsonConvert.SerializeObject(ResponseInfo<object>.Fail(ex, statusCode));

            Log.Error(exception: ex, messageTemplate: result);

            await context.Response.WriteAsync(result);
        }
    }
}
