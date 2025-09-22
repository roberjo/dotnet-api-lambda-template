using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.UpdateOrder;

/// <summary>
/// Command to update an existing order
/// </summary>
public class UpdateOrderCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// New order status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// New shipping address
    /// </summary>
    public string? ShippingAddress { get; set; }

    /// <summary>
    /// New billing address
    /// </summary>
    public string? BillingAddress { get; set; }

    /// <summary>
    /// New payment method
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// New payment transaction ID
    /// </summary>
    public string? PaymentTransactionId { get; set; }

    /// <summary>
    /// New order notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// New expected delivery date
    /// </summary>
    public DateTime? ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// New shipping tracking number
    /// </summary>
    public string? TrackingNumber { get; set; }

    /// <summary>
    /// New shipping carrier
    /// </summary>
    public string? ShippingCarrier { get; set; }
}
