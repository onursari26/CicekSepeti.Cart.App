using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Response;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Interfaces
{
    public interface IBasketService
    {
        Task<ResponseInfo<Basket>> AddToBasket(BasketItemModel basket);

        Task<ResponseInfo<NoContentModel>> EmptyBasketAsync();

        Task<ResponseInfo<NoContentModel>> RemoveProductFromCartAsync(int productId);

        Task<ResponseInfo<BasketModel>> GetBasketAsync();

        Task<ResponseInfo<NoContentModel>> AddToBasketFromLogin(int userId, BasketModel basket);

        Task<ResponseInfo<NoContentModel>> ApplyDiscount(string code);

        Task<ResponseInfo<NoContentModel>> CancelDiscount();

        Task<BasketModel> GetBasketFromCookie();
    }
}
