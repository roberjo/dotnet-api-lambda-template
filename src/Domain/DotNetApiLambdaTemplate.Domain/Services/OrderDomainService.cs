using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Services;

/// <summary>
/// Domain service for order-related business logic
/// </summary>
public class OrderDomainService : IOrderDomainService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;

    public OrderDomainService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Validates if an order can be created for a user
    /// </summary>
    public async Task<bool> CanCreateOrderAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId cannot be empty", nameof(customerId));

        var customer = await _userRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer == null || !customer.IsActive)
            return false;

        // Check if customer has any pending orders that might conflict
        var pendingOrders = await _orderRepository.GetPendingOrdersByCustomerAsync(customerId, cancellationToken);
        if (pendingOrders.Count() > 5) // Business rule: max 5 pending orders
            return false;

        return true;
    }

    /// <summary>
    /// Validates if an order can be updated
    /// </summary>
    public async Task<bool> CanUpdateOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        return order?.CanBeModified() ?? false;
    }

    /// <summary>
    /// Validates if an order can be cancelled
    /// </summary>
    public async Task<bool> CanCancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        return order?.CanBeCancelled() ?? false;
    }

    /// <summary>
    /// Validates if an order can be shipped
    /// </summary>
    public async Task<bool> CanShipOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return false;

        // Can ship if order is confirmed or preparing
        return order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Preparing;
    }

    /// <summary>
    /// Validates if an order can be delivered
    /// </summary>
    public async Task<bool> CanDeliverOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        return order?.Status == OrderStatus.Shipped;
    }

    /// <summary>
    /// Calculates shipping cost for an order
    /// </summary>
    public async Task<Money> CalculateShippingCostAsync(IEnumerable<OrderItem> orderItems, string shippingAddress, CancellationToken cancellationToken = default)
    {
        if (orderItems == null)
            throw new ArgumentNullException(nameof(orderItems));
        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("ShippingAddress cannot be null or empty", nameof(shippingAddress));

        var items = orderItems.ToList();
        if (!items.Any())
            return Money.Create(0, "USD");

        // Get total weight
        var totalWeight = items.Sum(item => item.GetTotalWeight() ?? 0);
        var currency = items.First().UnitPrice.Currency;

        // Simple shipping calculation based on weight and address
        var baseCost = CalculateBaseShippingCost(totalWeight, shippingAddress);
        return Money.Create(baseCost, currency);
    }

    /// <summary>
    /// Calculates tax amount for an order
    /// </summary>
    public async Task<Money> CalculateTaxAmountAsync(Money subtotal, string shippingAddress, CancellationToken cancellationToken = default)
    {
        if (subtotal == null)
            throw new ArgumentNullException(nameof(subtotal));
        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("ShippingAddress cannot be null or empty", nameof(shippingAddress));

        // Simple tax calculation based on address
        var taxRate = GetTaxRateForAddress(shippingAddress);
        var taxAmount = subtotal.Amount * taxRate;

        return Money.Create(taxAmount, subtotal.Currency);
    }

    /// <summary>
    /// Validates if products are available for order
    /// </summary>
    public async Task<bool> ValidateProductAvailabilityAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        if (orderItems == null)
            throw new ArgumentNullException(nameof(orderItems));

        var items = orderItems.ToList();
        if (!items.Any())
            return false;

        foreach (var item in items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product == null || !product.IsActive || !product.IsInStock())
                return false;

            if (product.StockQuantity < item.Quantity)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Reserves inventory for order items
    /// </summary>
    public async Task<bool> ReserveInventoryAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        if (orderItems == null)
            throw new ArgumentNullException(nameof(orderItems));

        var items = orderItems.ToList();
        if (!items.Any())
            return true;

        // Validate availability first
        if (!await ValidateProductAvailabilityAsync(items, cancellationToken))
            return false;

        // Reserve inventory for each item
        foreach (var item in items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product == null)
                return false;

            if (!product.ReserveStock(item.Quantity, "System"))
                return false;

            await _productRepository.UpdateAsync(product, cancellationToken);
        }

        return true;
    }

    /// <summary>
    /// Releases reserved inventory for order items
    /// </summary>
    public async Task ReleaseInventoryAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default)
    {
        if (orderItems == null)
            throw new ArgumentNullException(nameof(orderItems));

        var items = orderItems.ToList();
        if (!items.Any())
            return;

        foreach (var item in items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product != null)
            {
                product.ReleaseStock(item.Quantity, "System");
                await _productRepository.UpdateAsync(product, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Generates a unique order number
    /// </summary>
    public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        string orderNumber;
        bool isUnique = false;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            orderNumber = GenerateOrderNumber();
            var existingOrder = await _orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);
            isUnique = existingOrder == null;
            attempts++;
        }
        while (!isUnique && attempts < maxAttempts);

        if (!isUnique)
            throw new InvalidOperationException("Unable to generate unique order number after multiple attempts");

        return orderNumber;
    }

    /// <summary>
    /// Validates order business rules
    /// </summary>
    public async Task<OrderValidationResult> ValidateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        var result = new OrderValidationResult { IsValid = true };

        // Validate customer
        var customer = await _userRepository.GetByIdAsync(order.CustomerId, cancellationToken);
        if (customer == null || !customer.IsActive)
        {
            result.IsValid = false;
            result.Errors.Add("Customer is not valid or inactive");
        }

        // Validate order items
        if (!order.OrderItems.Any())
        {
            result.IsValid = false;
            result.Errors.Add("Order must have at least one item");
        }

        // Validate product availability
        if (!await ValidateProductAvailabilityAsync(order.OrderItems, cancellationToken))
        {
            result.IsValid = false;
            result.Errors.Add("One or more products are not available");
        }

        // Validate totals
        if (order.TotalAmount.Amount < 0)
        {
            result.IsValid = false;
            result.Errors.Add("Order total cannot be negative");
        }

        // Validate shipping address for physical products
        var hasPhysicalProducts = order.OrderItems.Any(item => item.Weight.HasValue);
        if (hasPhysicalProducts && string.IsNullOrWhiteSpace(order.ShippingAddress))
        {
            result.IsValid = false;
            result.Errors.Add("Shipping address is required for physical products");
        }

        // Add warnings
        if (order.TotalAmount.Amount > 10000) // High value order
        {
            result.Warnings.Add("High value order - additional verification may be required");
        }

        return result;
    }

    /// <summary>
    /// Gets the next valid status for an order
    /// </summary>
    public async Task<IEnumerable<OrderStatus>> GetValidNextStatusesAsync(OrderStatus currentStatus, CancellationToken cancellationToken = default)
    {
        return currentStatus switch
        {
            OrderStatus.Pending => new[] { OrderStatus.Processing, OrderStatus.Cancelled },
            OrderStatus.Processing => new[] { OrderStatus.Confirmed, OrderStatus.Cancelled, OrderStatus.OnHold },
            OrderStatus.Confirmed => new[] { OrderStatus.Preparing, OrderStatus.Cancelled },
            OrderStatus.Preparing => new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            OrderStatus.Shipped => new[] { OrderStatus.Delivered, OrderStatus.Returned },
            OrderStatus.Delivered => new[] { OrderStatus.Returned },
            OrderStatus.Cancelled => Array.Empty<OrderStatus>(),
            OrderStatus.Refunded => Array.Empty<OrderStatus>(),
            OrderStatus.OnHold => new[] { OrderStatus.Processing, OrderStatus.Cancelled },
            OrderStatus.Returned => new[] { OrderStatus.Refunded },
            _ => Array.Empty<OrderStatus>()
        };
    }

    #region Private Methods

    /// <summary>
    /// Calculates base shipping cost based on weight and address
    /// </summary>
    private static decimal CalculateBaseShippingCost(decimal totalWeight, string shippingAddress)
    {
        // Simple shipping calculation
        var baseCost = 5.00m; // Base shipping cost

        // Add weight-based cost
        if (totalWeight > 1000) // Over 1kg
            baseCost += (totalWeight - 1000) / 1000 * 2.00m; // $2 per additional kg

        // Add address-based cost (simplified)
        if (shippingAddress.Contains("International", StringComparison.OrdinalIgnoreCase))
            baseCost += 15.00m; // International shipping surcharge

        return Math.Round(baseCost, 2);
    }

    /// <summary>
    /// Gets tax rate for a shipping address
    /// </summary>
    private static decimal GetTaxRateForAddress(string shippingAddress)
    {
        // Simple tax calculation based on address
        if (shippingAddress.Contains("CA", StringComparison.OrdinalIgnoreCase) ||
            shippingAddress.Contains("California", StringComparison.OrdinalIgnoreCase))
            return 0.0875m; // 8.75% CA tax

        if (shippingAddress.Contains("NY", StringComparison.OrdinalIgnoreCase) ||
            shippingAddress.Contains("New York", StringComparison.OrdinalIgnoreCase))
            return 0.08m; // 8% NY tax

        if (shippingAddress.Contains("TX", StringComparison.OrdinalIgnoreCase) ||
            shippingAddress.Contains("Texas", StringComparison.OrdinalIgnoreCase))
            return 0.0625m; // 6.25% TX tax

        return 0.05m; // Default 5% tax
    }

    /// <summary>
    /// Generates a random order number
    /// </summary>
    private static string GenerateOrderNumber()
    {
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(100000, 999999);
        return $"ORD-{timestamp}-{random}";
    }

    #endregion
}

/// <summary>
/// Repository interface for order data access
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetPendingOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for product data access
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
