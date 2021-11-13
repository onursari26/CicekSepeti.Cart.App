using CicekSepeti.Domain.Abstract;
using System;

namespace CicekSepeti.Domain.Concrete
{
    public class Discount : BaseEntity<int>
    {
        public string Code { get; private set; }
        public decimal Price { get; private set; }
        public DateTime ExpiryDate { get; private set; }
        public bool IsActive { get; private set; }
        public decimal MinAmount { get; private set; }
        public Discount()
        {

        }
        public Discount(string code, decimal price, DateTime expiryDate, decimal minAmount, int? userId)
        {
            Code = code;
            Price = price;
            ExpiryDate = expiryDate;
            MinAmount = minAmount;
            IsActive = true;

            SetCreator(userId);
            SetModifier(userId);
        }

        public void RemoveDiscount()
        {
            IsActive = false;
        }
    }
}
