using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.UpdateOrderItemQuantity;

/// <summary>
/// Command to update the quantity of an order item
/// </summary>
public class UpdateOrderItemQuantityCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID containing the item
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Product ID of the item to update
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// New quantity
    /// </summary>
    public int NewQuantity { get; set; }
}
