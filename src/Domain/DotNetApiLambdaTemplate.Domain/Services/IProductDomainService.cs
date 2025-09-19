using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Services;

/// <summary>
/// Domain service interface for product-related business logic
/// </summary>
public interface IProductDomainService
{
    /// <summary>
    /// Validates if a product can be created
    /// </summary>
    /// <param name="sku">Product SKU</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product can be created</returns>
    Task<bool> CanCreateProductAsync(string sku, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a product can be updated
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product can be updated</returns>
    Task<bool> CanUpdateProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a product can be deactivated
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product can be deactivated</returns>
    Task<bool> CanDeactivateProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a product can be deleted
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product can be deleted</returns>
    Task<bool> CanDeleteProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if inventory can be adjusted for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="quantity">Quantity to adjust</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if inventory can be adjusted</returns>
    Task<bool> CanAdjustInventoryAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a product can be featured
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if product can be featured</returns>
    Task<bool> CanFeatureProductAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the optimal reorder point for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Optimal reorder point</returns>
    Task<int> CalculateOptimalReorderPointAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the optimal reorder quantity for a product
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Optimal reorder quantity</returns>
    Task<int> CalculateOptimalReorderQuantityAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates product pricing
    /// </summary>
    /// <param name="price">Product price</param>
    /// <param name="cost">Product cost</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pricing validation result</returns>
    Task<ProductPricingValidationResult> ValidatePricingAsync(Money price, Money? cost, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique SKU for a product
    /// </summary>
    /// <param name="category">Product category</param>
    /// <param name="brand">Product brand</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unique SKU</returns>
    Task<string> GenerateUniqueSkuAsync(ProductCategory category, string? brand, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products that need restocking
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of products that need restocking</returns>
    Task<IEnumerable<Product>> GetProductsNeedingRestockAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets low stock products
    /// </summary>
    /// <param name="threshold">Stock threshold</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of low stock products</returns>
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates product business rules
    /// </summary>
    /// <param name="product">Product to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result</returns>
    Task<ProductValidationResult> ValidateProductAsync(Product product, CancellationToken cancellationToken = default);
}

/// <summary>
/// Product pricing validation result
/// </summary>
public class ProductPricingValidationResult
{
    /// <summary>
    /// Whether the pricing is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Calculated profit margin
    /// </summary>
    public decimal? ProfitMargin { get; set; }

    /// <summary>
    /// Creates a valid result
    /// </summary>
    public static ProductPricingValidationResult Valid() => new() { IsValid = true };

    /// <summary>
    /// Creates an invalid result with errors
    /// </summary>
    public static ProductPricingValidationResult Invalid(params string[] errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };

    /// <summary>
    /// Creates a result with warnings
    /// </summary>
    public static ProductPricingValidationResult WithWarnings(params string[] warnings) => new()
    {
        IsValid = true,
        Warnings = warnings.ToList()
    };
}

/// <summary>
/// Product validation result
/// </summary>
public class ProductValidationResult
{
    /// <summary>
    /// Whether the product is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of validation errors
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// List of validation warnings
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Creates a valid result
    /// </summary>
    public static ProductValidationResult Valid() => new() { IsValid = true };

    /// <summary>
    /// Creates an invalid result with errors
    /// </summary>
    public static ProductValidationResult Invalid(params string[] errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };

    /// <summary>
    /// Creates a result with warnings
    /// </summary>
    public static ProductValidationResult WithWarnings(params string[] warnings) => new()
    {
        IsValid = true,
        Warnings = warnings.ToList()
    };
}
