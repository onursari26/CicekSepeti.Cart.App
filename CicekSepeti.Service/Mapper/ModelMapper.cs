using AutoMapper;
using CicekSepeti.Domain.Concrete;
using CicekSepeti.Model;

namespace CicekSepeti.Service.Mapper
{
    public class ModelMapper : Profile
    {
        public ModelMapper()
        {
            CreateMap<ProductModel, Product>().ReverseMap();

            CreateMap<DiscountModel, Discount>().ReverseMap();


            CreateMap<BasketModel, Basket>().ReverseMap();

            CreateMap<BasketItemModel, BasketItem>().ReverseMap();
        }
    }
}
