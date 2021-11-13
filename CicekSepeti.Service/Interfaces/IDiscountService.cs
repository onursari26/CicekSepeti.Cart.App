using CicekSepeti.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Interfaces
{
    public interface IDiscountService
    {
        Task<DiscountModel> GetDiscountByCode(string code);
        Task<IReadOnlyList<DiscountModel>> GetDiscounts();
        Task RemoveDiscountByCode(string code);
        Task PassiveDiscount();
    }
}
