using MediatR;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Queries.GetOrderById;

/// <summary>
/// Query to get an order by ID
/// </summary>
public class GetOrderByIdQuery : IRequest<OrderDto?>
{
    /// <summary>
    /// Order ID
    /// </summary>
    public Guid Id { get; set; }
}
