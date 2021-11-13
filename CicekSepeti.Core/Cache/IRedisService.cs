using System.Threading.Tasks;

namespace CicekSepeti.Core.Cache
{
    public interface IRedisService
    {
        Task<bool> SetAsync(object key, object value, double keyExpire = -1);
        Task<T> GetAsync<T>(object key);
        Task<bool> RemoveAsync(object key);
    }
}
