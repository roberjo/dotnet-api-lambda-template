using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Services;

/// <summary>
/// Domain service for product-related business logic
/// </summary>
public class ProductDomainService : IProductDomainService
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public ProductDomainService(
        IProductRepository productRepository,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    /// <summary>
    /// Validates if a product can be created
    /// </summary>
    public async Task<bool> CanCreateProductAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(sku));

        var existingProduct = await _productRepository.GetBySkuAsync(sku.Trim().ToUpperInvariant(), cancellationToken);
        return existingProduct == null;
    }

    /// <summary>
    /// Validates if a product can be updated
    /// </summary>
    public async Task<bool> CanUpdateProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        return product != null;
    }

    /// <summary>
    /// Validates if a product can be deactivated
    /// </summary>
    public async Task<bool> CanDeactivateProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null || !product.IsActive)
            return false;

        // Check if product has pending orders
        var pendingOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Pending, cancellationToken);
        var hasPendingOrders = pendingOrders.Any(order =>
            order.OrderItems.Any(item => item.ProductId == productId));

        return !hasPendingOrders;
    }

    /// <summary>
    /// Validates if a product can be deleted
    /// </summary>
    public async Task<bool> CanDeleteProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return false;

        // Check if product has any orders (completed or pending)
        var allOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Pending, cancellationToken);
        var hasOrders = allOrders.Any(order =>
            order.OrderItems.Any(item => item.ProductId == productId));

        return !hasOrders;
    }

    /// <summary>
    /// Validates if inventory can be adjusted for a product
    /// </summary>
    public async Task<bool> CanAdjustInventoryAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null || !product.IsActive)
            return false;

        // Check if adjustment would result in negative stock
        var newStock = product.StockQuantity + quantity;
        return newStock >= 0;
    }

    /// <summary>
    /// Validates if a product can be featured
    /// </summary>
    public async Task<bool> CanFeatureProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null || !product.IsActive)
            return false;

        // Business rules for featuring a product
        if (product.StockQuantity <= 0)
            return false;

        if (product.Rating < 3.0m)
            return false;

        return true;
    }

    /// <summary>
    /// Calculates the optimal reorder point for a product
    /// </summary>
    public async Task<int> CalculateOptimalReorderPointAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return 0;

        // Simple reorder point calculation based on historical data
        // In a real implementation, this would use actual sales data
        var averageDailySales = await CalculateAverageDailySalesAsync(productId, cancellationToken);
        var leadTimeDays = 7; // Assume 7-day lead time
        var safetyStock = (int)(averageDailySales * 2); // 2 days safety stock

        return (int)(averageDailySales * leadTimeDays) + safetyStock;
    }

    /// <summary>
    /// Calculates the optimal reorder quantity for a product
    /// </summary>
    public async Task<int> CalculateOptimalReorderQuantityAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty", nameof(productId));

        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return 0;

        // Simple EOQ (Economic Order Quantity) calculation
        var averageDailySales = await CalculateAverageDailySalesAsync(productId, cancellationToken);
        var annualDemand = averageDailySales * 365;
        var orderingCost = 50m; // Fixed ordering cost
        var holdingCost = product.Price.Amount * 0.2m; // 20% of product cost

        if (holdingCost <= 0)
            return (int)(averageDailySales * 30); // 30 days supply

        var eoq = Math.Sqrt((double)((2 * annualDemand * orderingCost) / holdingCost));
        return Math.Max(1, (int)Math.Ceiling(eoq));
    }

    /// <summary>
    /// Validates product pricing
    /// </summary>
    public async Task<ProductPricingValidationResult> ValidatePricingAsync(Money price, Money? cost, CancellationToken cancellationToken = default)
    {
        if (price == null)
            throw new ArgumentNullException(nameof(price));

        var result = new ProductPricingValidationResult { IsValid = true };

        // Validate price is positive
        if (price.Amount <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Price must be greater than zero");
        }

        // Validate cost if provided
        if (cost != null)
        {
            if (cost.Amount < 0)
            {
                result.IsValid = false;
                result.Errors.Add("Cost cannot be negative");
            }

            if (cost.Currency != price.Currency)
            {
                result.IsValid = false;
                result.Errors.Add("Cost currency must match price currency");
            }

            // Calculate profit margin
            if (cost.Amount > 0 && price.Amount > cost.Amount)
            {
                result.ProfitMargin = ((price.Amount - cost.Amount) / price.Amount) * 100;

                // Add warnings for low margins
                if (result.ProfitMargin < 10)
                {
                    result.Warnings.Add("Low profit margin - consider reviewing pricing");
                }
            }
            else if (cost.Amount > price.Amount)
            {
                result.IsValid = false;
                result.Errors.Add("Price must be greater than cost");
            }
        }

        // Validate price is not too high (business rule)
        if (price.Amount > 100000)
        {
            result.Warnings.Add("Very high price - additional approval may be required");
        }

        return result;
    }

    /// <summary>
    /// Generates a unique SKU for a product
    /// </summary>
    public async Task<string> GenerateUniqueSkuAsync(ProductCategory category, string? brand, CancellationToken cancellationToken = default)
    {
        string sku;
        bool isUnique = false;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            sku = GenerateSku(category, brand);
            var existingProduct = await _productRepository.GetBySkuAsync(sku, cancellationToken);
            isUnique = existingProduct == null;
            attempts++;
        }
        while (!isUnique && attempts < maxAttempts);

        if (!isUnique)
            throw new InvalidOperationException("Unable to generate unique SKU after multiple attempts");

        return sku;
    }

    /// <summary>
    /// Gets products that need restocking
    /// </summary>
    public async Task<IEnumerable<Product>> GetProductsNeedingRestockAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetActiveProductsAsync(cancellationToken);
        return products.Where(p => p.NeedsRestocking());
    }

    /// <summary>
    /// Gets low stock products
    /// </summary>
    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetActiveProductsAsync(cancellationToken);
        return products.Where(p => p.StockQuantity <= threshold);
    }

    /// <summary>
    /// Validates product business rules
    /// </summary>
    public async Task<ProductValidationResult> ValidateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        var result = new ProductValidationResult { IsValid = true };

        // Validate SKU uniqueness
        if (!await CanCreateProductAsync(product.Sku, cancellationToken))
        {
            result.IsValid = false;
            result.Errors.Add("SKU already exists");
        }

        // Validate pricing
        var pricingResult = await ValidatePricingAsync(product.Price, product.Cost, cancellationToken);
        if (!pricingResult.IsValid)
        {
            result.IsValid = false;
            result.Errors.AddRange(pricingResult.Errors);
        }
        result.Warnings.AddRange(pricingResult.Warnings);

        // Validate stock levels
        if (product.StockQuantity < 0)
        {
            result.IsValid = false;
            result.Errors.Add("Stock quantity cannot be negative");
        }

        if (product.MinStockLevel > product.MaxStockLevel)
        {
            result.IsValid = false;
            result.Errors.Add("Minimum stock level cannot be greater than maximum stock level");
        }

        // Validate weight for physical products
        if (product.Weight.HasValue && product.Weight.Value <= 0)
        {
            result.IsValid = false;
            result.Errors.Add("Weight must be positive for physical products");
        }

        // Add warnings
        if (product.StockQuantity > product.MaxStockLevel)
        {
            result.Warnings.Add("Stock quantity exceeds maximum stock level");
        }

        if (product.Rating > 0 && product.ReviewCount == 0)
        {
            result.Warnings.Add("Product has rating but no reviews");
        }

        return result;
    }

    #region Private Methods

    /// <summary>
    /// Calculates average daily sales for a product
    /// </summary>
    private async Task<decimal> CalculateAverageDailySalesAsync(Guid productId, CancellationToken cancellationToken)
    {
        // This is a simplified calculation
        // In a real implementation, this would analyze historical order data
        var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return 0;

        // Simple heuristic based on product category and rating
        var baseSales = product.Category switch
        {
            ProductCategory.Electronics => 2.5m,
            ProductCategory.Clothing => 5.0m,
            ProductCategory.Books => 3.0m,
            ProductCategory.HomeGarden => 1.5m,
            ProductCategory.Sports => 2.0m,
            ProductCategory.HealthBeauty => 4.0m,
            ProductCategory.Automotive => 0.5m,
            ProductCategory.FoodBeverage => 8.0m,
            ProductCategory.ToysGames => 3.5m,
            ProductCategory.OfficeSupplies => 2.0m,
            _ => 1.0m
        };

        // Adjust based on rating
        var ratingMultiplier = product.Rating switch
        {
            >= 4.5m => 1.5m,
            >= 4.0m => 1.2m,
            >= 3.5m => 1.0m,
            >= 3.0m => 0.8m,
            _ => 0.5m
        };

        return baseSales * ratingMultiplier;
    }

    /// <summary>
    /// Generates a SKU based on category and brand
    /// </summary>
    private static string GenerateSku(ProductCategory category, string? brand)
    {
        var categoryCode = category switch
        {
            ProductCategory.Electronics => "ELC",
            ProductCategory.Clothing => "CLO",
            ProductCategory.Books => "BOK",
            ProductCategory.HomeGarden => "HOM",
            ProductCategory.Sports => "SPT",
            ProductCategory.HealthBeauty => "HLT",
            ProductCategory.Automotive => "AUT",
            ProductCategory.FoodBeverage => "FOD",
            ProductCategory.ToysGames => "TOY",
            ProductCategory.OfficeSupplies => "OFF",
            _ => "OTH"
        };

        var brandCode = string.IsNullOrWhiteSpace(brand)
            ? "UNK"
            : brand.Substring(0, Math.Min(3, brand.Length)).ToUpperInvariant();

        var timestamp = DateTimeOffset.UtcNow.ToString("MMdd");
        var random = new Random().Next(100, 999);

        return $"{categoryCode}-{brandCode}-{timestamp}-{random}";
    }

    #endregion
}
