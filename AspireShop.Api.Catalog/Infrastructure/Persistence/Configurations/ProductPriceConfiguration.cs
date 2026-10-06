using AspireShop.Api.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspireShop.Api.Catalog.Infrastructure.Persistence.Configurations;

public sealed class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
{
    public void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        builder.HasKey(price => price.Id);

        builder.Property(price => price.Amount)
            .HasPrecision(18, 2);

        builder.Property(price => price.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        builder.HasOne(price => price.Product)
            .WithMany(product => product.Prices)
            .HasForeignKey(price => price.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(price => new { price.ProductId, price.ValidFrom });
    }
}
