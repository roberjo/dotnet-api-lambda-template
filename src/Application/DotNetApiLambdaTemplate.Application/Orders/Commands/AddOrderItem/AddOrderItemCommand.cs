using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.AddOrderItem;

/// <summary>
/// Command to add an item to an existing order
/// </summary>
public class AddOrderItemCommand : IRequest<OrderDto>
{
    /// <summary>
    /// Order ID to add item to
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Product ID to add
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Quantity to add
    /// </summary>
    public int Quantity { get; set; }
}
