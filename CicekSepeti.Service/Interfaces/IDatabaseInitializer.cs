using System.Threading.Tasks;

namespace CicekSepeti.Service.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }
}
