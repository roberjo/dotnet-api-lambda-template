using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.Enums;
using DotNetApiLambdaTemplate.Domain.ValueObjects;

namespace DotNetApiLambdaTemplate.Domain.Services;

/// <summary>
/// Domain service interface for order-related business logic
/// </summary>
public interface IOrderDomainService
{
    /// <summary>
    /// Validates if an order can be created for a user
    /// </summary>
    /// <param name="customerId">Customer user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if order can be created</returns>
    Task<bool> CanCreateOrderAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if an order can be updated
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if order can be updated</returns>
    Task<bool> CanUpdateOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if an order can be cancelled
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if order can be cancelled</returns>
    Task<bool> CanCancelOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if an order can be shipped
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if order can be shipped</returns>
    Task<bool> CanShipOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if an order can be delivered
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if order can be delivered</returns>
    Task<bool> CanDeliverOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates shipping cost for an order
    /// </summary>
    /// <param name="orderItems">Order items</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Shipping cost</returns>
    Task<Money> CalculateShippingCostAsync(IEnumerable<OrderItem> orderItems, string shippingAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates tax amount for an order
    /// </summary>
    /// <param name="subtotal">Order subtotal</param>
    /// <param name="shippingAddress">Shipping address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tax amount</returns>
    Task<Money> CalculateTaxAmountAsync(Money subtotal, string shippingAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if products are available for order
    /// </summary>
    /// <param name="orderItems">Order items to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if all products are available</returns>
    Task<bool> ValidateProductAvailabilityAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reserves inventory for order items
    /// </summary>
    /// <param name="orderItems">Order items to reserve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if inventory was successfully reserved</returns>
    Task<bool> ReserveInventoryAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases reserved inventory for order items
    /// </summary>
    /// <param name="orderItems">Order items to release</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the operation</returns>
    Task ReleaseInventoryAsync(IEnumerable<OrderItem> orderItems, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a unique order number
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unique order number</returns>
    Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates order business rules
    /// </summary>
    /// <param name="order">Order to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with any errors</returns>
    Task<OrderValidationResult> ValidateOrderAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next valid status for an order
    /// </summary>
    /// <param name="currentStatus">Current order status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of valid next statuses</returns>
    Task<IEnumerable<OrderStatus>> GetValidNextStatusesAsync(OrderStatus currentStatus, CancellationToken cancellationToken = default);
}

/// <summary>
/// Order validation result
/// </summary>
public class OrderValidationResult
{
    /// <summary>
    /// Whether the order is valid
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
    public static OrderValidationResult Valid() => new() { IsValid = true };

    /// <summary>
    /// Creates an invalid result with errors
    /// </summary>
    public static OrderValidationResult Invalid(params string[] errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };

    /// <summary>
    /// Creates a result with warnings
    /// </summary>
    public static OrderValidationResult WithWarnings(params string[] warnings) => new()
    {
        IsValid = true,
        Warnings = warnings.ToList()
    };
}
