using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.RemoveOrderItem;

/// <summary>
/// Command to remove an item from an order
/// </summary>
public class RemoveOrderItemCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Product ID to remove from the order
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// User or system performing the removal
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;
}
