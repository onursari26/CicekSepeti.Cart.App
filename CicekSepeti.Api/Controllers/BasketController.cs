using CicekSepeti.Api.Models;
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
    public class BasketController : BaseController
    {
        private readonly IBasketService _basketService;
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [ProducesResponseType(typeof(ResponseInfo<BasketModel>), StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<IActionResult> AddBasketAsync(BasketItemViewModel basket)
        {
            return ResponseResult(await _basketService.AddToBasket(new BasketItemModel
            {
                ProductId = basket.ProductId,
                CurrentQuantity = basket.CurrentQuantity
            }));
        }

        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpDelete]
        public async Task<IActionResult> RemoveBasketAsync()
        {
            return ResponseResult(await _basketService.EmptyBasketAsync());
        }

        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveProductFromCartAsync(int productId)
        {
            return ResponseResult(await _basketService.RemoveProductFromCartAsync(productId));
        }

        [ProducesResponseType(typeof(ResponseInfo<BasketModel>), StatusCodes.Status201Created)]
        [HttpGet]
        public async Task<IActionResult> GetBasketAsync()
        {
            return ResponseResult(await _basketService.GetBasketAsync());
        }

        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpPost("applydiscount")]
        public async Task<IActionResult> ApplyDiscount(DiscountViewModel discount)
        {
            return ResponseResult(await _basketService.ApplyDiscount(discount.Code));
        }

        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpPost("canceldiscount")]
        public async Task<IActionResult> CancelDiscount()
        {
            return ResponseResult(await _basketService.CancelDiscount());
        }
    }
}
