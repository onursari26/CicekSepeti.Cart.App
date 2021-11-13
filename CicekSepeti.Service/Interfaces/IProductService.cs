using CicekSepeti.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Interfaces
{
    public interface IProductService
    {
        Task<ProductModel> GetProductById(int id);
        Task<IReadOnlyList<ProductModel>> GetProducts();
        Task UpdatePrice(int id, decimal price);
        Task PassiveProduct(int id);
        Task ActiveProduct(int id);
        Task<IReadOnlyList<ProductModel>> GetProductByIds(List<int> ids);
    }
}
