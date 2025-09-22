using Microsoft.EntityFrameworkCore;
using DotNetApiLambdaTemplate.Domain.Entities;

namespace DotNetApiLambdaTemplate.Infrastructure.Data;

/// <summary>
/// Application database context for Entity Framework Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            // Configure owned types
            entity.OwnsOne(e => e.Name, name =>
            {
                name.Property(n => n.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(100);
                name.Property(n => n.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(100);
            });

            entity.OwnsOne(e => e.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("Email").IsRequired().HasMaxLength(254);
            });

            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsDeleted).IsRequired();

            // Indexes
            entity.HasIndex(e => e.Email.Value).IsUnique();
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDeleted);
        });

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.StockQuantity).IsRequired();
            entity.Property(e => e.MinStockLevel).IsRequired();
            entity.Property(e => e.MaxStockLevel).IsRequired();
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.ReviewCount).IsRequired();
            entity.Property(e => e.IsFeatured).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsDeleted).IsRequired();

            // Configure owned types
            entity.OwnsOne(e => e.Price, price =>
            {
                price.Property(p => p.Amount).HasColumnName("PriceAmount").HasColumnType("decimal(18,2)");
                price.Property(p => p.Currency).HasColumnName("PriceCurrency").HasMaxLength(3);
            });

            entity.OwnsOne(e => e.Cost, cost =>
            {
                cost.Property(c => c.Amount).HasColumnName("CostAmount").HasColumnType("decimal(18,2)");
                cost.Property(c => c.Currency).HasColumnName("CostCurrency").HasMaxLength(3);
            });

            // Indexes
            entity.HasIndex(e => e.Sku).IsUnique();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsDeleted);
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(254);
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.OrderSource).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsDeleted).IsRequired();

            // Configure owned types
            entity.OwnsOne(e => e.Subtotal, subtotal =>
            {
                subtotal.Property(s => s.Amount).HasColumnName("SubtotalAmount").HasColumnType("decimal(18,2)");
                subtotal.Property(s => s.Currency).HasColumnName("SubtotalCurrency").HasMaxLength(3);
            });

            entity.OwnsOne(e => e.TaxAmount, tax =>
            {
                tax.Property(t => t.Amount).HasColumnName("TaxAmount").HasColumnType("decimal(18,2)");
                tax.Property(t => t.Currency).HasColumnName("TaxCurrency").HasMaxLength(3);
            });

            entity.OwnsOne(e => e.ShippingCost, shipping =>
            {
                shipping.Property(s => s.Amount).HasColumnName("ShippingCostAmount").HasColumnType("decimal(18,2)");
                shipping.Property(s => s.Currency).HasColumnName("ShippingCostCurrency").HasMaxLength(3);
            });

            entity.OwnsOne(e => e.DiscountAmount, discount =>
            {
                discount.Property(d => d.Amount).HasColumnName("DiscountAmount").HasColumnType("decimal(18,2)");
                discount.Property(d => d.Currency).HasColumnName("DiscountCurrency").HasMaxLength(3);
            });

            entity.OwnsOne(e => e.TotalAmount, total =>
            {
                total.Property(t => t.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
                total.Property(t => t.Currency).HasColumnName("TotalCurrency").HasMaxLength(3);
            });

            // Configure OrderItems as owned entities
            entity.OwnsMany(e => e.OrderItems, orderItem =>
            {
                orderItem.WithOwner().HasForeignKey("OrderId");
                orderItem.Property<Guid>("OrderId");
                orderItem.Property<Guid>("Id").ValueGeneratedOnAdd();

                orderItem.Property(oi => oi.ProductId).IsRequired();
                orderItem.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);
                orderItem.Property(oi => oi.ProductSku).IsRequired().HasMaxLength(100);
                orderItem.Property(oi => oi.Quantity).IsRequired();

                orderItem.OwnsOne(oi => oi.UnitPrice, price =>
                {
                    price.Property(p => p.Amount).HasColumnName("UnitPriceAmount").HasColumnType("decimal(18,2)");
                    price.Property(p => p.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3);
                });

                orderItem.HasKey("Id");
            });

            // Indexes
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);
        });
    }
}