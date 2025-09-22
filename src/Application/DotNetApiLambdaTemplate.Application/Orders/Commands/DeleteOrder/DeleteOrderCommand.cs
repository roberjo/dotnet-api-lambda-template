using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.DeleteOrder;

/// <summary>
/// Command to delete an order
/// </summary>
public class DeleteOrderCommand : IRequest<bool>
{
    /// <summary>
    /// Order ID to delete
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User or system performing the deletion
    /// </summary>
    public string DeletedBy { get; set; } = string.Empty;
}
