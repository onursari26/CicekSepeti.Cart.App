using CicekSepeti.Core.Interfaces;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Abstract;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Concrete
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _services;
        public ProductService(
            IUnitOfWork unitOfWork,
            IServiceProvider services,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _unitOfWork = unitOfWork;
            _services = services;
        }

        /// <summary>
        /// Girilen idye göre ürün bilgisini getirir
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductModel> GetProductById(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            return ObjectMapper.Mapper.Map<ProductModel>(product);

        }

        /// <summary>
        /// Tüm ürünleri listeler
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<ProductModel>> GetProducts()
        {
            var products = (await _unitOfWork.Repository<Product>().GetAllAsync()).ToList().AsReadOnly();

            return ObjectMapper.Mapper.Map<IReadOnlyList<ProductModel>>(products);
        }

        /// <summary>
        /// Fiyat güncellemesi yapar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task UpdatePrice(int id, decimal price)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            product.UpdatePrice(price, UserId);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Ürünü pasife alır
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task PassiveProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            product.PassiveStock(UserId);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Ürünü aktife alır
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ActiveProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            product.ActiveStock(UserId);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// girilen id listesine göre ürünlerin id ve fiyat bilgilerini getirir
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<ProductModel>> GetProductByIds(List<int> ids)
        {
            var products = await _unitOfWork.Repository<Product>().GetQuery(p =>
            {
                p = p.Where(x => ids.Contains(x.Id));
                return p;
            }).Select(x => new ProductModel { Id = x.Id, Price = x.Price, IsActive = x.IsActive }).ToListAsync();

            return products.AsReadOnly();
        }
    }
}
