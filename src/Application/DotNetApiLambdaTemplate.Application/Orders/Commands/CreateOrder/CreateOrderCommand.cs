using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Command to create a new order
/// </summary>
public class CreateOrderCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Customer user ID
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Customer email at the time of order
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Customer name at the time of order
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Currency for the order
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Shipping address
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// Billing address
    /// </summary>
    public string? BillingAddress { get; set; }

    /// <summary>
    /// Order source (e.g., "Web", "Mobile", "API")
    /// </summary>
    public string OrderSource { get; set; } = "Web";

    /// <summary>
    /// Order notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Promo code used
    /// </summary>
    public string? PromoCode { get; set; }
}
