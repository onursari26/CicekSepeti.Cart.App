using CicekSepeti.Model;
using CicekSepeti.Service.Response;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Interfaces
{
    public interface IUserService
    {
        Task<ResponseInfo<UserModel>> CreateUserAsync(UserModel user);
        Task<ResponseInfo<UserModel>> GetUserByNameAsync(string username);
    }
}
