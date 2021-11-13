namespace CicekSepeti.Domain.Concrete
{
    public class BasketItem
    {
        public int ProductId { get; private set; }
        public decimal CurrentQuantity { get; private set; }
        public decimal CurrentPrice { get; private set; }

        public BasketItem(int productId, decimal currentQuantity, decimal currentPrice)
        {
            ProductId = productId;
            CurrentQuantity = currentQuantity;
            CurrentPrice = currentPrice;
        }

        public void Update(decimal currentQuantity, decimal currentPrice)
        {
            CurrentQuantity = currentQuantity;
            CurrentPrice = currentPrice;
        }

        public void UpdatePrice(decimal currentPrice)
        {
            CurrentPrice = currentPrice;
        }

        private BasketItem()
        {

        }
    }
}
