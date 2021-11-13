using Newtonsoft.Json;

namespace CicekSepeti.Model
{
    public class BasketItemModel
    {
        public int ProductId { get; set; }
        public decimal? CurrentQuantity { get; set; }
        public decimal? CurrentPrice { get; set; }
    }
}
