using CicekSepeti.Core.Cache;
using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CicekSepeti.Api.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseInfo<IReadOnlyList<ProductModel>>), StatusCodes.Status200OK)]
        [HttpGet("all")]
        public async Task<IActionResult> GetProductsAsync()
        {
            return ResponseResult(ResponseInfo<IReadOnlyList<ProductModel>>.Success((await _productService.GetProducts())));
        }

        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrice(int id, decimal price)
        {
            await _productService.UpdatePrice(id, price);

            return ResponseResult(ResponseInfo<NoContentModel>.Success());
        }


        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpPut("{id}/false")]
        public async Task<IActionResult> PassiveProduct(int id)
        {
            await _productService.PassiveProduct(id);

            return ResponseResult(ResponseInfo<NoContentModel>.Success());
        }

        [ProducesResponseType(typeof(ResponseInfo<NoContentModel>), StatusCodes.Status204NoContent)]
        [HttpPut("{id}/true")]
        public async Task<IActionResult> ActiveProduct(int id)
        {
            await _productService.ActiveProduct(id);

            return ResponseResult(ResponseInfo<NoContentModel>.Success());
        }

    }
}
