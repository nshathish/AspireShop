using AspireShop.Api.Orders.Domain.Entities;
using AspireShop.Api.Orders.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AspireShop.Api.Orders.Infrastructure.Persistence;

public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}
