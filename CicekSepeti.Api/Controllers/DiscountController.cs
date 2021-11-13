using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CicekSepeti.Api.Controllers
{
    public class DiscountController : BaseController
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }


        [ProducesResponseType(typeof(ResponseInfo<IReadOnlyList<DiscountModel>>), StatusCodes.Status200OK)]
        [HttpGet("all")]
        public async Task<IActionResult> GetDiscountsAsync()
        {
            return ResponseResult(ResponseInfo<IReadOnlyList<DiscountModel>>.Success((await _discountService.GetDiscounts())));
        }
    }
}
