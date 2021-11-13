using System;

namespace CicekSepeti.Model
{
    public class DiscountModel
    {
        public string Code { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public decimal MinAmount { get; set; }
    }
}
