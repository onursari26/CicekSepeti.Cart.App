using CicekSepeti.Model;
using CicekSepeti.Service.Response;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ResponseInfo<string>> CreateTokenAsync(LoginModel login);
    }
}
