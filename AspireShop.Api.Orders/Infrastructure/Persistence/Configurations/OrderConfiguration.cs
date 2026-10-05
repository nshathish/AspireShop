using AspireShop.Api.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspireShop.Api.Orders.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(order => order.CustomerEmail)
            .IsRequired()
            .HasMaxLength(320);

        builder.Property(order => order.Total)
            .HasPrecision(18, 2);

        builder.Property(order => order.Status)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(order => order.CreatedAt)
            .IsRequired();
    }
}
