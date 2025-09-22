using DotNetApiLambdaTemplate.Domain.Enums;

namespace DotNetApiLambdaTemplate.Application.Common.DTOs;

/// <summary>
/// Data Transfer Object for Product entity
/// </summary>
public class ProductDto
{
    /// <summary>
    /// Product unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Stock Keeping Unit (unique identifier for product variant)
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Product category
    /// </summary>
    public ProductCategory Category { get; set; }

    /// <summary>
    /// Selling price amount
    /// </summary>
    public decimal PriceAmount { get; set; }

    /// <summary>
    /// Price currency
    /// </summary>
    public string PriceCurrency { get; set; } = "USD";

    /// <summary>
    /// Cost amount (optional, for internal use)
    /// </summary>
    public decimal? CostAmount { get; set; }

    /// <summary>
    /// Cost currency
    /// </summary>
    public string? CostCurrency { get; set; }

    /// <summary>
    /// Current stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Minimum stock level before reorder is triggered
    /// </summary>
    public int MinStockLevel { get; set; }

    /// <summary>
    /// Maximum stock level
    /// </summary>
    public int MaxStockLevel { get; set; }

    /// <summary>
    /// Product weight in kilograms (optional)
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Product dimensions (e.g., "10x5x2 cm") (optional)
    /// </summary>
    public string? Dimensions { get; set; }

    /// <summary>
    /// Product brand (optional)
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// Product model (optional)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Product color (optional)
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Product size (e.g., "S", "M", "L", "One Size") (optional)
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// Comma-separated tags for the product (optional)
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Comma-separated URLs of product images (optional)
    /// </summary>
    public string ImageUrls { get; set; } = string.Empty;

    /// <summary>
    /// JSON string of product specifications (optional)
    /// </summary>
    public string Specifications { get; set; } = string.Empty;

    /// <summary>
    /// Average rating (0-5)
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Number of reviews
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Warranty period in months (optional)
    /// </summary>
    public int? WarrantyMonths { get; set; }

    /// <summary>
    /// Indicates if the product is featured (e.g., on homepage)
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// User who last updated this record
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}
