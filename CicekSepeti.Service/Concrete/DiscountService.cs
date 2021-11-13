using CicekSepeti.Core.Interfaces;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Abstract;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CicekSepeti.Service.Concrete
{
    public class DiscountService : BaseService, IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DiscountService(IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContext) : base(httpContext)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Girilen koda göre indirim bilgisini getirir
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<DiscountModel> GetDiscountByCode(string code)
        {
            var discount = await _unitOfWork.Repository<Discount>().GetQuery(p =>
            {
                p = p.Where(w => w.Code == code);
                return p;
            }).FirstOrDefaultAsync();

            return ObjectMapper.Mapper.Map<DiscountModel>(discount);
        }

        /// <summary>
        /// Tüm indirimleri listeler
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<DiscountModel>> GetDiscounts()
        {
            var discounts = (await _unitOfWork.Repository<Discount>().GetAllAsync()).ToList().AsReadOnly();

            return ObjectMapper.Mapper.Map<IReadOnlyList<DiscountModel>>(discounts);
        }

        /// <summary>
        /// Girilen koda göre indirim bilgisini siler
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task RemoveDiscountByCode(string code)
        {
            var discount = await _unitOfWork.Repository<Discount>().GetQuery(p =>
            {
                p = p.Where(w => w.Code == code);
                return p;
            }).FirstOrDefaultAsync();

            discount.RemoveDiscount();
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Hangfire tarafından süreleri dolmuş indirim bilgilerini pasife çeker
        /// </summary>
        /// <returns></returns>
        public async Task PassiveDiscount()
        {
            var discounts = await _unitOfWork.Repository<Discount>().GetQuery(p =>
            {
                p = p.Where(w => w.ExpiryDate < System.DateTime.Now);
                return p;
            }).ToListAsync();

            if (discounts.Count > 0)
            {
                discounts.ForEach(x =>
                {
                    x.RemoveDiscount();
                });

                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
