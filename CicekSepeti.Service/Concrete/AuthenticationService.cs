using CicekSepeti.Core.Extensions;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Abstract;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Concrete
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly TokenOption _option;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBasketService _basketService;

        public AuthenticationService(
               IOptions<TokenOption> option,
               UserManager<User> userManager,
               IConfiguration configuration,
               IBasketService basketService,
               IHttpContextAccessor httpContext)
              : base(httpContext)
        {
            _option = option.Value;
            _userManager = userManager;
            _configuration = configuration;
            _basketService = basketService;
        }

        /// <summary>
        /// Login olup token üretir.
        /// Eğer cookide sepet varsa onu redise yazma işleminin tetikler
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<string>> CreateTokenAsync(LoginModel login)
        {
            User user = await _userManager.FindByNameAsync(login.Username);

            if (user == null)
                return ResponseInfo<string>.Error("(UserName/Email) not found", System.Net.HttpStatusCode.NotFound);

            if (!await _userManager.CheckPasswordAsync(user, login.Password))
                return ResponseInfo<string>.Error("(UserName/Email) or Password is wrong", System.Net.HttpStatusCode.BadRequest);

            var token = CreateToken(user);

            var basketModelResponse = await _basketService.GetBasketFromCookie();
            if (basketModelResponse != null)
                await _basketService.AddToBasketFromLogin(user.Id, basketModelResponse);

            return ResponseInfo<string>.Success(token, System.Net.HttpStatusCode.Created);
        }

        /// <summary>
        /// Token üretir
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CreateToken(User user)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_option.Expiration);
            var securityKey = _configuration["JWTSecurityKey"].GetSymmetricSecurityKey();

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _option.Issuer,
                expires: accessTokenExpiration,
                 notBefore: DateTime.Now,
                 claims: GetClaims(user, _option.Audience),
                 signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(jwtSecurityToken);

            return token;
        }

        /// <summary>
        /// Claimleri oluşturur
        /// </summary>
        /// <param name="userApp"></param>
        /// <param name="audiences"></param>
        /// <returns></returns>
        private IEnumerable<Claim> GetClaims(User userApp, List<string> audiences)
        {
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id.ToString()),
                new Claim(ClaimTypes.Name,userApp.UserName),
            };

            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            return userList;
        }



    }
}
