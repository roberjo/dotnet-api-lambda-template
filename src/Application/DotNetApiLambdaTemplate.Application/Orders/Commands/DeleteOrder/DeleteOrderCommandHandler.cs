using MediatR;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;

namespace DotNetApiLambdaTemplate.Application.Orders.Commands.DeleteOrder;

/// <summary>
/// Handler for DeleteOrderCommand
/// </summary>
public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (order == null)
        {
            return false;
        }

        await _orderRepository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
