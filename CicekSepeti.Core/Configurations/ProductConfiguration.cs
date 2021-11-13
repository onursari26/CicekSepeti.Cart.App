using CicekSepeti.Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CicekSepeti.Core.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.Code).HasMaxLength(32);
            builder.Property(x => x.Description).HasMaxLength(400);
            builder.Property(x => x.StockQuantity).HasColumnType("decimal(18,8)");
            builder.Property(x => x.MaxSaleableQuantity).HasColumnType("decimal(18,8)");
            builder.Property(x => x.Price).HasColumnType("decimal(18,8)");
        }
    }
}
