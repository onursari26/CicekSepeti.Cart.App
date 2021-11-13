using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CicekSepeti.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [ProducesResponseType(typeof(ResponseInfo<string>), StatusCodes.Status201Created)]
        [HttpPost("login")]
        public async Task<IActionResult> CreateTokenAsync([FromBody] LoginModel login)
        {
            return ResponseResult(await _authenticationService.CreateTokenAsync(login));
        }
    }
}
