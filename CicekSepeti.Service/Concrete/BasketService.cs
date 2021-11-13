using CicekSepeti.Core.Cache;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Abstract;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Mapper;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Concrete
{
    public class BasketService : BaseService, IBasketService
    {
        private readonly IProductService _productService;
        private readonly IDiscountService _discountService;
        private readonly IRedisService _redisService;
        public BasketService(IProductService productService, IRedisService redisService, IDiscountService discountService, IHttpContextAccessor httpContext) : base(httpContext)
        {
            _productService = productService;
            _discountService = discountService;
            _redisService = redisService;
        }

        /// <summary>
        /// Sepete Ekler
        /// </summary>
        /// <param name="basketItem"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<Basket>> AddToBasket(BasketItemModel basketItem)
        {
            var productResponse = await GetProduct(basketItem);
            if (!productResponse.IsSuccessfull)
                return ResponseInfo<Basket>.Error(productResponse.ErrorMessage);

            var stockResponse = await StockControl(basketItem.CurrentQuantity.Value, productResponse.Data);
            if (!stockResponse.IsSuccessfull)
                return ResponseInfo<Basket>.Error(stockResponse.ErrorMessage);

            return ResponseInfo<Basket>.Success(!UserId.HasValue ? await AddCookie(basketItem) : await AddRedis(basketItem));
        }

        /// <summary>
        /// Stok bilgisini alır
        /// </summary>
        /// <param name="basketItem"></param>
        /// <returns></returns>
        private async Task<ResponseInfo<ProductModel>> GetProduct(BasketItemModel basketItem)
        {
            if (basketItem.ProductId <= 0)
                return ResponseInfo<ProductModel>.Error("ürün kodu hatalı", System.Net.HttpStatusCode.NotFound);

            if (!basketItem.CurrentQuantity.HasValue)
                basketItem.CurrentQuantity = 1;

            var product = await _productService.GetProductById(basketItem.ProductId);

            if (product == null)
                return ResponseInfo<ProductModel>.Error("Ürün bulunamadı.", System.Net.HttpStatusCode.NotFound);

            if (!product.IsActive)
                return ResponseInfo<ProductModel>.Error("Ürün bulunamadı.", System.Net.HttpStatusCode.NotFound);

            basketItem.CurrentPrice = product.Price;

            return ResponseInfo<ProductModel>.Success(product);
        }

        /// <summary>
        /// Stokla ilgili kontrolleri yapar
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        private Task<ResponseInfo<NoContentModel>> StockControl(decimal quantity, ProductModel product)
        {
            if (quantity > product.StockQuantity)
                return Task.FromResult(ResponseInfo<NoContentModel>.Error("Yeterli stok bulunamadı."));

            if (quantity > product.MaxSaleableQuantity)
                return Task.FromResult(ResponseInfo<NoContentModel>.Error("Max alınabilen miktardan fazla girildi."));

            return Task.FromResult(ResponseInfo<NoContentModel>.Success());
        }

        /// <summary>
        /// Sepeti cookiede tutar (User Login değilse)
        /// </summary>
        /// <param name="basketItem"></param>
        /// <returns></returns>
        private async Task<Basket> AddCookie(BasketItemModel basketItem)
        {
            var basketModelResponse = await GetBasketFromCookie();

            if (basketModelResponse != null)
            {
                var basket = ObjectMapper.Mapper.Map<Basket>(basketModelResponse);

                await UpdatePriceFromBasket(basket);

                var updateBasketItem = basket.Items.FirstOrDefault(x => x.ProductId == basketItem.ProductId);

                if (updateBasketItem != null)
                {
                    updateBasketItem.Update(basketItem.CurrentQuantity.Value, basketItem.CurrentPrice.Value);
                    basket.UpdateBasketItem(updateBasketItem.ProductId, updateBasketItem, UserId);
                }
                else
                    basket.AddBasketItem(new BasketItem(basketItem.ProductId, basketItem.CurrentQuantity.Value, basketItem.CurrentPrice.Value), UserId);

                UpdateCookie(JsonConvert.SerializeObject(basket));

                return basket;
            }
            else
            {
                var basket = new Basket(UserId);

                basket.AddBasketItem(new BasketItem(basketItem.ProductId, basketItem.CurrentQuantity.Value, basketItem.CurrentPrice.Value), UserId);

                AddCookie(JsonConvert.SerializeObject(basket));

                return basket;
            }
        }

        /// <summary>
        /// Cookide bulunan sepeti getirir (User Login değilse)
        /// </summary>
        /// <returns></returns>
        public async Task<BasketModel> GetBasketFromCookie()
        {
            var json = GetCookie();

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var basket = JsonConvert.DeserializeObject<BasketModel>(json);
            return basket;
        }

        /// <summary>
        /// Sepeti redise ekler (User Login ise)
        /// </summary>
        /// <param name="basketItem"></param>
        /// <returns></returns>
        private async Task<Basket> AddRedis(BasketItemModel basketItem)
        {
            var basketResponse = await GetBasketFromRedis();

            if (basketResponse != null)
            {
                var basket = ObjectMapper.Mapper.Map<Basket>(basketResponse);

                await UpdatePriceFromBasket(basket);

                var updateBasketItem = basket.Items.FirstOrDefault(x => x.ProductId == basketItem.ProductId);

                if (updateBasketItem != null)
                {
                    updateBasketItem.Update(basketItem.CurrentQuantity.Value, basketItem.CurrentPrice.Value);
                    basket.UpdateBasketItem(updateBasketItem.ProductId, updateBasketItem, UserId);
                }
                else
                {
                    basket.AddBasketItem(new BasketItem(basketItem.ProductId, basketItem.CurrentQuantity.Value, basketItem.CurrentPrice.Value), UserId);
                }

                await _redisService.SetAsync(UserId, basket);

                return basket;
            }
            else
            {
                var basket = new Basket(UserId);

                basket.AddBasketItem(new BasketItem(basketItem.ProductId, basketItem.CurrentQuantity.Value, basketItem.CurrentPrice.Value), UserId);

                await _redisService.SetAsync(UserId, basket);

                return basket;
            }

        }

        /// <summary>
        /// Redisten sepeti getirir (User Login ise)
        /// </summary>
        /// <returns></returns>
        private async Task<BasketModel> GetBasketFromRedis()
        {
            var basket = await _redisService.GetAsync<BasketModel>(UserId);
            return basket;
        }

        /// <summary>
        /// Yeni login olan kullanıcının token olusana kadar idsine baglı rediste sepet varsa onu getirir 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<BasketModel> GetBasketFromRedisByUserId(int userId)
        {
            var basket = await _redisService.GetAsync<BasketModel>(userId);
            return basket;
        }


        /// <summary>
        /// Sepetin tümünü siler
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseInfo<NoContentModel>> EmptyBasketAsync()
        {
            if (!UserId.HasValue)
                RemoveCookie();
            else
                await _redisService.RemoveAsync(UserId);

            return ResponseInfo<NoContentModel>.Success();
        }

        /// <summary>
        /// Ürüne göre sepetten siler
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<NoContentModel>> RemoveProductFromCartAsync(int productId)
        {
            BasketModel basketResponse = !UserId.HasValue ? await GetBasketFromCookie() : await GetBasketFromRedis();

            if (basketResponse == null)
                return ResponseInfo<NoContentModel>.Error("Sepet bulunamadı.");

            var basket = ObjectMapper.Mapper.Map<Basket>(basketResponse);

            var basketItem = basket.Items.FirstOrDefault(x => x.ProductId == productId);
            if (basketItem == null)
                return ResponseInfo<NoContentModel>.Error("Ürün bulunamadı.");

            basket.RemoveBasketItem(basketItem, null);

            if (!UserId.HasValue)
                UpdateCookie(JsonConvert.SerializeObject(basket));
            else
                await _redisService.SetAsync(UserId, basket);

            return ResponseInfo<NoContentModel>.Success();
        }

        /// <summary>
        /// Sepeti getirir
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseInfo<BasketModel>> GetBasketAsync()
        {
            var basketModel = !UserId.HasValue ? await GetBasketFromCookie() : await GetBasketFromRedis();

            var basket = ObjectMapper.Mapper.Map<Basket>(basketModel);

            if (basket != null)
                await UpdatePriceFromBasket(basket);

            return ResponseInfo<BasketModel>.Success(basketModel);
        }

        /// <summary>
        /// Sepette daha önceden eklene stok varsa onların fiyatlarını kontrol eder.
        /// </summary>
        /// <param name="basket"></param>
        /// <returns></returns>
        private async Task UpdatePriceFromBasket(Basket basket)
        {
            var ids = basket.Items.Select(x => x.ProductId).ToList();

            var dbProducts = await _productService.GetProductByIds(ids);

            List<int> removeProducts = new List<int>();

            foreach (var item in basket.Items)
            {
                var product = dbProducts.FirstOrDefault(x => x.Id == item.ProductId);
                if (product.Price != item.CurrentPrice)
                {
                    item.UpdatePrice(product.Price.Value);
                    basket.UpdatePrice(UserId);
                }
                if (!product.IsActive)
                {
                    removeProducts.Add(item.ProductId);
                }
            }

            if (removeProducts.Count > 0)
                basket.RemoveBasketItem(removeProducts, UserId);


            if (!UserId.HasValue)
                UpdateCookie(JsonConvert.SerializeObject(basket));
            else
                await _redisService.SetAsync(UserId, basket);
        }

        /// <summary>
        /// Yeni login olan kullanıcının token olusana kadar cookiede sepeti varsa onu redise ekler
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="basketModel"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<NoContentModel>> AddToBasketFromLogin(int userId, BasketModel basketModel)
        {
            var basketResponse = await GetBasketFromRedisByUserId(userId);

            if (basketResponse != null)
            {
                var basket = ObjectMapper.Mapper.Map<Basket>(basketResponse);

                await UpdatePriceFromBasket(basket);

                var updateBasketItems = basket.Items.Where(x => basketModel.Items.Any(y => y.ProductId == x.ProductId)).ToList();
                if (updateBasketItems != null && updateBasketItems.Count > 0)
                {
                    foreach (var updateBasketItem in updateBasketItems)
                    {
                        var item = basketModel.Items.FirstOrDefault(x => x.ProductId == updateBasketItem.ProductId);
                        updateBasketItem.Update(item.CurrentQuantity.Value, item.CurrentPrice.Value);
                        basket.UpdateBasketItem(updateBasketItem.ProductId, updateBasketItem, UserId);
                    }
                }

                var addBasketItems = basketModel.Items.Where(x => !basket.Items.Any(y => y.ProductId == x.ProductId)).ToList();
                if (addBasketItems != null && addBasketItems.Count > 0)
                {
                    foreach (var addBasketItem in addBasketItems)
                    {
                        var item = basketModel.Items.FirstOrDefault(x => x.ProductId == addBasketItem.ProductId);
                        basket.AddBasketItem(new BasketItem(item.ProductId, item.CurrentQuantity.Value, item.CurrentPrice.Value), userId);
                    }
                }

                await _redisService.SetAsync(userId, basket);
            }
            else
            {
                var newBasket = new Basket(userId);

                foreach (var item in basketModel.Items)
                    newBasket.AddBasketItem(new BasketItem(item.ProductId, item.CurrentQuantity.Value, item.CurrentPrice.Value), userId);

                if (basketModel.IsApplyDiscount)
                    newBasket.ApplyDiscount(basketModel.DiscountCode, basketModel.DiscountPrice, userId);

                await _redisService.SetAsync(userId, newBasket);
            }

            RemoveCookie();

            return ResponseInfo<NoContentModel>.Success();
        }

        /// <summary>
        /// İndirim uygular
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResponseInfo<NoContentModel>> ApplyDiscount(string code)
        {
            var discountResponse = await DiscountControl(code);
            if (!discountResponse.IsSuccessfull)
                return ResponseInfo<NoContentModel>.Error(discountResponse.ErrorMessage);

            BasketModel basketModel = !UserId.HasValue ? await GetBasketFromCookie() : await GetBasketFromRedis();

            var basket = ObjectMapper.Mapper.Map<Basket>(basketModel);

            if (basketModel == null)
                return ResponseInfo<NoContentModel>.Error("Sepet boşken indirim uygulanamaz.");

            if (basketModel.TotalPrice < discountResponse.Data.MinAmount)
                return ResponseInfo<NoContentModel>.Error("Min. uygulanan tutar eksik.");

            if (basketModel.IsApplyDiscount)
                return ResponseInfo<NoContentModel>.Error("Bir kere indirim uygulanabilir.");

            basket.CancelDiscount(UserId);
            basket.ApplyDiscount(code, discountResponse.Data.Price, UserId);

            if (!UserId.HasValue)
                UpdateCookie(JsonConvert.SerializeObject(basket));
            else
                await _redisService.SetAsync(UserId, basket);

            //order olusturulurken iptal edilmesi daha doğru olacaktır.
            //await _discountService.RemoveDiscountByCode(code);

            return ResponseInfo<NoContentModel>.Success();
        }

        /// <summary>
        /// İndirimi iptal eder
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseInfo<NoContentModel>> CancelDiscount()
        {
            BasketModel basketModel = !UserId.HasValue ? await GetBasketFromCookie() : await GetBasketFromRedis();

            var basket = ObjectMapper.Mapper.Map<Basket>(basketModel);

            if (basketModel == null)
                return ResponseInfo<NoContentModel>.Error("Sepet boş.");

            if (!basketModel.IsApplyDiscount)
                return ResponseInfo<NoContentModel>.Error("İndirim uygulanmamış.");

            basket.CancelDiscount(UserId);

            if (!UserId.HasValue)
                UpdateCookie(JsonConvert.SerializeObject(basket));
            else
                await _redisService.SetAsync(UserId, basket);

            return ResponseInfo<NoContentModel>.Success();
        }

        /// <summary>
        /// İndirim bilgisini kontrol eder
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private async Task<ResponseInfo<DiscountModel>> DiscountControl(string code)
        {
            var discount = await _discountService.GetDiscountByCode(code);

            if (discount == null)
                return ResponseInfo<DiscountModel>.Error("İndirim kodu hatalı.");

            if (!discount.IsActive)
                return ResponseInfo<DiscountModel>.Error("İndirim kodu aktif değil.");

            if (discount.ExpiryDate < DateTime.Today)
                return ResponseInfo<DiscountModel>.Error("İndirim kodu süresi dolu.");

            return ResponseInfo<DiscountModel>.Success(discount);
        }
    }
}
