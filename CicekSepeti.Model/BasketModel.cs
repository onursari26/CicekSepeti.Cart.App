using System.Collections.Generic;

namespace CicekSepeti.Model
{
    public class BasketModel : BaseModel<string>
    {
        public int? UserId { get; set; }
        public List<BasketItemModel> Items { get; set; }
        public bool IsUpdatePrice { get; set; }
        public bool IsApplyDiscount { get; set; }
        public string DiscountCode { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalCount { get; set; }
    }
}
