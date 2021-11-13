using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;

namespace CicekSepeti.Service.Abstract
{
    public abstract class BaseService
    {
        private readonly IHttpContextAccessor _httpContext;

        protected readonly int? UserId;

        public BaseService(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
                return;

            _httpContext = httpContextAccessor;

            if ((_httpContext?.HttpContext?.User?.Claims)!.Any())
            {
                UserId = int.Parse(_httpContext.HttpContext.User.Claims?.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value ?? null);
            }
        }

        public void AddCookie(string basket)
        {
            if (_httpContext.HttpContext.Request.Cookies["basket"] == null)
                _httpContext.HttpContext.Response.Cookies.Append("basket", basket
                    //, new CookieOptions { Expires = DateTime.Now.AddDays(1) }
                    );
        }

        public string GetCookie()
        {
            _httpContext.HttpContext.Request.Cookies.TryGetValue("basket", out string basketCookies);
            return basketCookies;
        }

        public void UpdateCookie(string basket)
        {
            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
                _httpContext.HttpContext.Response.Cookies.Delete("basket");

            _httpContext.HttpContext.Response.Cookies.Append("basket", basket
              //, new CookieOptions { Expires = DateTime.Now.AddDays(1) }
              );
        }

        public void RemoveCookie()
        {
            if (_httpContext.HttpContext.Request.Cookies["basket"] != null)
                _httpContext.HttpContext.Response.Cookies.Delete("basket");
        }
    }
}
