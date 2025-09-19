using DotNetApiLambdaTemplate.Domain.Common;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Entities;

/// <summary>
/// Product entity representing a sellable item
/// </summary>
public class Product : BaseEntity<Guid>
{
    /// <summary>
    /// Product name
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Product description
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Product SKU (Stock Keeping Unit)
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>
    /// Product category
    /// </summary>
    public ProductCategory Category { get; private set; }

    /// <summary>
    /// Product price
    /// </summary>
    public Money Price { get; private set; } = null!;

    /// <summary>
    /// Product cost (for profit calculation)
    /// </summary>
    public Money? Cost { get; private set; }

    /// <summary>
    /// Current stock quantity
    /// </summary>
    public int StockQuantity { get; private set; }

    /// <summary>
    /// Minimum stock level for reorder alerts
    /// </summary>
    public int MinStockLevel { get; private set; }

    /// <summary>
    /// Maximum stock level
    /// </summary>
    public int MaxStockLevel { get; private set; }

    /// <summary>
    /// Product weight in grams
    /// </summary>
    public decimal? Weight { get; private set; }

    /// <summary>
    /// Product dimensions (length x width x height in cm)
    /// </summary>
    public string? Dimensions { get; private set; }

    /// <summary>
    /// Product brand
    /// </summary>
    public string? Brand { get; private set; }

    /// <summary>
    /// Product model
    /// </summary>
    public string? Model { get; private set; }

    /// <summary>
    /// Product color
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Product size
    /// </summary>
    public string? Size { get; private set; }

    /// <summary>
    /// Product tags for search and categorization
    /// </summary>
    public string Tags { get; private set; } = string.Empty;

    /// <summary>
    /// Whether the product is active and available for sale
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Whether the product is featured
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Product images URLs (comma-separated)
    /// </summary>
    public string ImageUrls { get; private set; } = string.Empty;

    /// <summary>
    /// Product specifications (JSON format)
    /// </summary>
    public string Specifications { get; private set; } = string.Empty;

    /// <summary>
    /// Product warranty period in months
    /// </summary>
    public int? WarrantyMonths { get; private set; }

    /// <summary>
    /// Product rating (0-5)
    /// </summary>
    public decimal Rating { get; private set; }

    /// <summary>
    /// Number of reviews
    /// </summary>
    public int ReviewCount { get; private set; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Product() { }

    /// <summary>
    /// Creates a new Product instance
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="name">Product name</param>
    /// <param name="description">Product description</param>
    /// <param name="sku">Product SKU</param>
    /// <param name="category">Product category</param>
    /// <param name="price">Product price</param>
    /// <param name="createdBy">User or system that created this product</param>
    /// <param name="cost">Product cost (optional)</param>
    /// <param name="stockQuantity">Initial stock quantity</param>
    /// <param name="minStockLevel">Minimum stock level</param>
    /// <param name="maxStockLevel">Maximum stock level</param>
    /// <param name="weight">Product weight in grams</param>
    /// <param name="dimensions">Product dimensions</param>
    /// <param name="brand">Product brand</param>
    /// <param name="model">Product model</param>
    /// <param name="color">Product color</param>
    /// <param name="size">Product size</param>
    /// <param name="tags">Product tags</param>
    /// <param name="imageUrls">Product image URLs</param>
    /// <param name="specifications">Product specifications</param>
    /// <param name="warrantyMonths">Warranty period in months</param>
    public Product(
        Guid id,
        string name,
        string description,
        string sku,
        ProductCategory category,
        Money price,
        string createdBy,
        Money? cost = null,
        int stockQuantity = 0,
        int minStockLevel = 0,
        int maxStockLevel = 1000,
        decimal? weight = null,
        string? dimensions = null,
        string? brand = null,
        string? model = null,
        string? color = null,
        string? size = null,
        string tags = "",
        string imageUrls = "",
        string specifications = "",
        int? warrantyMonths = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(sku));
        if (price == null)
            throw new ArgumentNullException(nameof(price));
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("CreatedBy cannot be null or empty", nameof(createdBy));

        Name = name.Trim();
        Description = description.Trim();
        Sku = sku.Trim().ToUpperInvariant();
        Category = category;
        Price = price;
        Cost = cost;
        StockQuantity = Math.Max(0, stockQuantity);
        MinStockLevel = Math.Max(0, minStockLevel);
        MaxStockLevel = Math.Max(maxStockLevel, minStockLevel);
        Weight = weight;
        Dimensions = dimensions?.Trim();
        Brand = brand?.Trim();
        Model = model?.Trim();
        Color = color?.Trim();
        Size = size?.Trim();
        Tags = tags?.Trim() ?? string.Empty;
        ImageUrls = imageUrls?.Trim() ?? string.Empty;
        Specifications = specifications?.Trim() ?? string.Empty;
        WarrantyMonths = warrantyMonths;
        IsActive = true;
        IsFeatured = false;
        Rating = 0;
        ReviewCount = 0;

        SetCreated(createdBy);
    }

    /// <summary>
    /// Updates the product's basic information
    /// </summary>
    /// <param name="name">New name</param>
    /// <param name="description">New description</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateBasicInfo(string name, string description, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty", nameof(description));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Name = name.Trim();
        Description = description.Trim();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product's pricing
    /// </summary>
    /// <param name="price">New price</param>
    /// <param name="cost">New cost (optional)</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdatePricing(Money price, Money? cost, string updatedBy)
    {
        if (price == null)
            throw new ArgumentNullException(nameof(price));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Price = price;
        Cost = cost;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product's inventory
    /// </summary>
    /// <param name="stockQuantity">New stock quantity</param>
    /// <param name="minStockLevel">New minimum stock level</param>
    /// <param name="maxStockLevel">New maximum stock level</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateInventory(int stockQuantity, int minStockLevel, int maxStockLevel, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        StockQuantity = Math.Max(0, stockQuantity);
        MinStockLevel = Math.Max(0, minStockLevel);
        MaxStockLevel = Math.Max(maxStockLevel, minStockLevel);
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product's physical attributes
    /// </summary>
    /// <param name="weight">New weight in grams</param>
    /// <param name="dimensions">New dimensions</param>
    /// <param name="brand">New brand</param>
    /// <param name="model">New model</param>
    /// <param name="color">New color</param>
    /// <param name="size">New size</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdatePhysicalAttributes(
        decimal? weight,
        string? dimensions,
        string? brand,
        string? model,
        string? color,
        string? size,
        string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Weight = weight;
        Dimensions = dimensions?.Trim();
        Brand = brand?.Trim();
        Model = model?.Trim();
        Color = color?.Trim();
        Size = size?.Trim();
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product's category and tags
    /// </summary>
    /// <param name="category">New category</param>
    /// <param name="tags">New tags</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateCategoryAndTags(ProductCategory category, string tags, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Category = category;
        Tags = tags?.Trim() ?? string.Empty;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product's media and specifications
    /// </summary>
    /// <param name="imageUrls">New image URLs</param>
    /// <param name="specifications">New specifications</param>
    /// <param name="warrantyMonths">New warranty period</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateMediaAndSpecifications(string imageUrls, string specifications, int? warrantyMonths, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        ImageUrls = imageUrls?.Trim() ?? string.Empty;
        Specifications = specifications?.Trim() ?? string.Empty;
        WarrantyMonths = warrantyMonths;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Adjusts stock quantity
    /// </summary>
    /// <param name="quantity">Quantity to add (positive) or subtract (negative)</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void AdjustStock(int quantity, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        var newQuantity = StockQuantity + quantity;
        if (newQuantity < 0)
            throw new InvalidOperationException("Stock quantity cannot be negative");

        StockQuantity = newQuantity;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Reserves stock for an order
    /// </summary>
    /// <param name="quantity">Quantity to reserve</param>
    /// <param name="updatedBy">User or system making the update</param>
    /// <returns>True if stock was successfully reserved</returns>
    public bool ReserveStock(int quantity, string updatedBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        if (StockQuantity < quantity)
            return false;

        StockQuantity -= quantity;
        SetUpdated(updatedBy);
        return true;
    }

    /// <summary>
    /// Releases reserved stock
    /// </summary>
    /// <param name="quantity">Quantity to release</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void ReleaseStock(int quantity, string updatedBy)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        StockQuantity += quantity;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Activates the product
    /// </summary>
    /// <param name="activatedBy">User or system activating the product</param>
    public void Activate(string activatedBy)
    {
        if (string.IsNullOrWhiteSpace(activatedBy))
            throw new ArgumentException("ActivatedBy cannot be null or empty", nameof(activatedBy));

        IsActive = true;
        SetUpdated(activatedBy);
    }

    /// <summary>
    /// Deactivates the product
    /// </summary>
    /// <param name="deactivatedBy">User or system deactivating the product</param>
    public void Deactivate(string deactivatedBy)
    {
        if (string.IsNullOrWhiteSpace(deactivatedBy))
            throw new ArgumentException("DeactivatedBy cannot be null or empty", nameof(deactivatedBy));

        IsActive = false;
        SetUpdated(deactivatedBy);
    }

    /// <summary>
    /// Sets the product as featured
    /// </summary>
    /// <param name="featured">Whether the product is featured</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void SetFeatured(bool featured, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        IsFeatured = featured;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Updates the product rating
    /// </summary>
    /// <param name="rating">New rating (0-5)</param>
    /// <param name="reviewCount">New review count</param>
    /// <param name="updatedBy">User or system making the update</param>
    public void UpdateRating(decimal rating, int reviewCount, string updatedBy)
    {
        if (rating < 0 || rating > 5)
            throw new ArgumentException("Rating must be between 0 and 5", nameof(rating));
        if (reviewCount < 0)
            throw new ArgumentException("Review count cannot be negative", nameof(reviewCount));
        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new ArgumentException("UpdatedBy cannot be null or empty", nameof(updatedBy));

        Rating = rating;
        ReviewCount = reviewCount;
        SetUpdated(updatedBy);
    }

    /// <summary>
    /// Checks if the product is in stock
    /// </summary>
    /// <returns>True if in stock</returns>
    public bool IsInStock()
    {
        return StockQuantity > 0;
    }

    /// <summary>
    /// Checks if the product needs restocking
    /// </summary>
    /// <returns>True if stock is below minimum level</returns>
    public bool NeedsRestocking()
    {
        return StockQuantity <= MinStockLevel;
    }

    /// <summary>
    /// Calculates the profit margin
    /// </summary>
    /// <returns>Profit margin percentage, or null if cost is not set</returns>
    public decimal? GetProfitMargin()
    {
        if (Cost == null || Price.Amount == 0)
            return null;

        var profit = Price.Amount - Cost.Amount;
        return (profit / Price.Amount) * 100;
    }

    /// <summary>
    /// Gets the profit amount
    /// </summary>
    /// <returns>Profit amount, or null if cost is not set</returns>
    public Money? GetProfit()
    {
        if (Cost == null)
            return null;

        return Price.Subtract(Cost);
    }
}
