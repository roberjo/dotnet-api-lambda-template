using AutoMapper;
using MediatR;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Products.Commands.AdjustProductInventory;

/// <summary>
/// Handler for AdjustProductInventoryCommand
/// </summary>
public class AdjustProductInventoryCommandHandler : IRequestHandler<AdjustProductInventoryCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public AdjustProductInventoryCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProductDto> Handle(AdjustProductInventoryCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {request.Id} not found.");
        }

        product.AdjustStock(request.QuantityChange, "System"); // TODO: Get from context
        await _productRepository.UpdateAsync(product, cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
