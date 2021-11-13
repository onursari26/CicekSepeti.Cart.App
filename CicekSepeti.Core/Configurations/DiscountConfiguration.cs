using CicekSepeti.Domain.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CicekSepeti.Core.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.ToTable("Discounts");
            builder.Property(x => x.Code).HasMaxLength(32);
            builder.Property(x => x.Price).HasColumnType("decimal(18,8)");
            builder.Property(x => x.MinAmount).HasColumnType("decimal(18,8)");
            builder.Property(x => x.ExpiryDate).HasColumnType("datetime");
        }
    }
}
