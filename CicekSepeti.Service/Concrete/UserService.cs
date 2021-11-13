using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Concrete
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// YEni kullanıcı oluşturur
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<UserModel>> CreateUserAsync(UserModel user)
        {
            var newUser = new User
            {
                Email = user.Email,
                UserName = user.Username,
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
                return ResponseInfo<UserModel>.Error(result.Errors.Select(x => x.Description).ToList());

            return ResponseInfo<UserModel>.Success(user, System.Net.HttpStatusCode.Created);
        }

        /// <summary>
        /// Auth. olan kullanıcı bilgisini getirir
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<UserModel>> GetUserByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return ResponseInfo<UserModel>.Error("User not found");

            return ResponseInfo<UserModel>.Success(new UserModel
            {
                Email = user.Email,
                Username = user.UserName
            });
        }
    }
}
