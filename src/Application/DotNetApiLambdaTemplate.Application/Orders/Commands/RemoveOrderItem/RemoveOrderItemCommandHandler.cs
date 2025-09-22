using AutoMapper;
using MediatR;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.RemoveOrderItem;

/// <summary>
/// Handler for RemoveOrderItemCommand
/// </summary>
public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public RemoveOrderItemCommandHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<OrderDto> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");
        }

        order.RemoveOrderItem(request.ProductId, request.UpdatedBy);
        await _orderRepository.UpdateAsync(order, cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }
}
