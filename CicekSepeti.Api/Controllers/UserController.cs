using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CicekSepeti.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseInfo<UserModel>), StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserModel user)
        {
            return ResponseResult(await _userService.CreateUserAsync(user));
        }

        [ProducesResponseType(typeof(ResponseInfo<UserModel>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetUserByNameAsync()
        {
            return ResponseResult(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name));
        }
    }
}
