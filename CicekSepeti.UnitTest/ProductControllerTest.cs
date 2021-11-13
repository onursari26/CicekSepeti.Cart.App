using CicekSepeti.Api.Controllers;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;
using CicekSepeti.Service.Interfaces;
using CicekSepeti.Service.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CicekSepeti.UnitTest
{
    public class ProductControllerTest
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _productController;

        private IReadOnlyList<ProductModel> products;
        public ProductControllerTest()
        {
            _mockService = new Mock<IProductService>();
            _productController = new ProductController(_mockService.Object);

            products = new List<ProductModel>
            {
               new ProductModel
               {
                   Id=1,
                   Code=  "1000",
                   Name= "Test-1000",
                   Description= null,
                   StockQuantity= 1000,
                   MaxSaleableQuantity= 10,
                   Price=150
               },
               new ProductModel
               {
                   Id=2,
                   Code=  "2000",
                   Name= "Test-2000",
                   Description= null,
                   StockQuantity= 1250,
                   MaxSaleableQuantity= 5,
                   Price=500
               },
               new ProductModel
               {
                   Id=3,
                   Code=  "3000",
                   Name= "Test-3000",
                   Description= null,
                   StockQuantity= 150,
                   MaxSaleableQuantity= 5,
                   Price=1000
               },
               new ProductModel
               {
                   Id=4,
                   Code=  "4000",
                   Name= "Test-4000",
                   Description= null,
                   StockQuantity= 150,
                   MaxSaleableQuantity= 5,
                   Price=25000
               },
            };
        }

        [Fact]
        public async void GetProducts_ActionExecutes_ReturnOkResultWithProducts()
        {
            _mockService.Setup(x => x.GetProducts()).ReturnsAsync(products);

            var result = await _productController.GetProductsAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnProduct = Assert.IsAssignableFrom<ResponseInfo<IReadOnlyList<ProductModel>>>(okResult.Value);

            Assert.Equal(4, returnProduct.Data.Count);
        }

        [Theory]
        [InlineData(1)]
        public void UpdateProduct_IdIsNotEqualProduct_ReturnBadRequestResult(int productId)
        {
            var product = products.SingleOrDefault(x => x.Id == productId);

            var result = _productController.UpdatePrice(2, 100);

            var badRequestResult = Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int productId)
        {
            var product = products.SingleOrDefault(x => x.Id == productId);

            _mockService.Setup(x => x.UpdatePrice(product.Id, 100));

            var result = _productController.UpdatePrice(productId, 100);

            _mockService.Verify(x => x.UpdatePrice(product.Id, 100), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
