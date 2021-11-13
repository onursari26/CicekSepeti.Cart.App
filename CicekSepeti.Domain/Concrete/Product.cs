using CicekSepeti.Domain.Abstract;
using System;

namespace CicekSepeti.Domain.Concrete
{
    public class Product : BaseEntity<int>
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal StockQuantity { get; private set; }
        public decimal? MaxSaleableQuantity { get; private set; }
        public decimal? Price { get; private set; }
        public bool IsActive { get; private set; }


        public Product(string code, string name, string description, decimal quantity, decimal? maxSaleableQuantity, decimal? price, bool isActive, int? userId)
        {
            Code = code;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = quantity;
            MaxSaleableQuantity = maxSaleableQuantity;
            IsActive = isActive;

            SetCreator(userId);
            SetModifier(userId);
        }


        public void Update(string code, string name, string description, decimal quantity, decimal? maxSaleableQuantity, decimal? price, bool isActive, int? userId)
        {
            Code = code;
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = quantity;
            MaxSaleableQuantity = maxSaleableQuantity;
            IsActive = isActive;

            SetModifier(userId);
        }

        public void ActiveStock(int? userId)
        {
            IsActive = true;
            SetModifier(userId);
        }

        public void PassiveStock(int? userId)
        {
            IsActive = false;
            SetModifier(userId);
        }

        public void DropStock(decimal quantity, int? userId)
        {
            StockQuantity = StockQuantity - quantity;
            SetModifier(userId);
        }

        public void UpdatePrice(decimal price, int? userId)
        {
            Price = price;
            SetModifier(userId);
        }
        private Product()
        {

        }
    }
}
