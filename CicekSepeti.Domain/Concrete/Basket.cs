using CicekSepeti.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CicekSepeti.Domain.Concrete
{
    public class Basket : BaseEntity<string>
    {
        public int? UserId { get; private set; }
        public bool IsUpdatePrice { get; private set; }
        public bool IsApplyDiscount { get; private set; }
        public bool IsPassiveProduct { get; private set; }
        public string DiscountCode { get; private set; }
        public decimal DiscountPrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        public decimal TotalCount { get; private set; }

        private readonly List<int> _removeProducts = new();
        public IReadOnlyCollection<int> RemoveProducts => _removeProducts;

        private readonly List<BasketItem> _items = new();
        public IReadOnlyCollection<BasketItem> Items => _items;


        public Basket(int? userId)
        {
            UserId = userId;

            SetId(Guid.NewGuid().ToString());

            //Items = new List<BasketItem>();

            SetCreator(userId);
            SetModifier(userId);
        }

        public void AddBasketItem(BasketItem item, int? userId)
        {
            _items.Add(item);

            TotalPrice = Items.Sum(x => x.CurrentPrice * x.CurrentQuantity);
            TotalCount = Items.Count;

            SetModifier(userId);
        }

        public void UpdateBasketItem(int productId, BasketItem item, int? userId)
        {
            var basketItem = Items.FirstOrDefault(x => x.ProductId == productId);

            if (basketItem != null)
                _items.Remove(basketItem);

            _items.Add(item);

            TotalPrice = Items.Sum(x => x.CurrentPrice * x.CurrentQuantity);
            TotalCount = Items.Count;

            SetModifier(userId);

        }

        public void RemoveBasketItem(BasketItem item, int? userId)
        {
            _items.Remove(item);

            TotalPrice = Items.Sum(x => x.CurrentPrice * x.CurrentQuantity);
            TotalCount = Items.Count;

            SetModifier(userId);
        }

        public void RemoveBasketItem(List<int> passiveProducts, int? userId)
        {
            passiveProducts.ForEach(item =>
            {
                var removeItem = _items.FirstOrDefault(x => x.ProductId == item);
                if (removeItem != null)
                    _items.Remove(removeItem);
            });

            TotalPrice = Items.Sum(x => x.CurrentPrice * x.CurrentQuantity);
            TotalCount = Items.Count;

            IsPassiveProduct = true;
            _removeProducts.AddRange(passiveProducts);

            SetModifier(userId);
        }


        public void UpdatePrice(int? userId)
        {
            IsUpdatePrice = true;

            TotalPrice = Items.Sum(x => x.CurrentPrice * x.CurrentQuantity);
            TotalCount = Items.Count;

            SetModifier(userId);
        }

        public void ApplyDiscount(string discountCode, decimal discountPrice, int? userId)
        {
            IsApplyDiscount = true;
            DiscountCode = discountCode;
            DiscountPrice = discountPrice;

            SetModifier(userId);
        }

        public void CancelDiscount(int? userId)
        {
            IsApplyDiscount = false;
            DiscountCode = null;
            DiscountPrice = 0;

            SetModifier(userId);
        }
    }
}
