using AutoMapper;
using DotNetApiLambdaTemplate.Domain.Entities;
using DotNetApiLambdaTemplate.Domain.ValueObjects;
using DotNetApiLambdaTemplate.Application.Common.DTOs;

namespace DotNetApiLambdaTemplate.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateUserMappings();
        CreateProductMappings();
        CreateOrderMappings();
    }

    private void CreateUserMappings()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.Name.FirstName} {src.Name.LastName}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<UserDto, User>()
            .ConstructUsing(src => new User(
                src.Id,
                FullName.Create(src.FirstName, src.LastName),
                Email.Create(src.Email),
                src.Role,
                src.CreatedBy,
                src.PhoneNumber,
                src.Department,
                src.JobTitle,
                src.ExternalId,
                src.TimeZone ?? "UTC",
                src.Language ?? "en-US"));
    }

    private void CreateProductMappings()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.PriceAmount, opt => opt.MapFrom(src => src.Price.Amount))
            .ForMember(dest => dest.PriceCurrency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(dest => dest.CostAmount, opt => opt.MapFrom(src => src.Cost != null ? src.Cost.Amount : (decimal?)null))
            .ForMember(dest => dest.CostCurrency, opt => opt.MapFrom(src => src.Cost != null ? src.Cost.Currency : null));

        CreateMap<ProductDto, Product>()
            .ConstructUsing(src => new Product(
                src.Id,
                src.Name,
                src.Description,
                src.Sku,
                src.Category,
                Money.Create(src.PriceAmount, src.PriceCurrency),
                src.CreatedBy,
                src.CostAmount.HasValue ? Money.Create(src.CostAmount.Value, src.CostCurrency ?? src.PriceCurrency) : null,
                src.StockQuantity,
                src.MinStockLevel,
                src.MaxStockLevel,
                src.Weight,
                src.Dimensions,
                src.Brand,
                src.Model,
                src.Color,
                src.Size,
                src.Tags,
                src.ImageUrls,
                src.Specifications,
                src.WarrantyMonths));
    }

    private void CreateOrderMappings()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.SubtotalAmount, opt => opt.MapFrom(src => src.Subtotal.Amount))
            .ForMember(dest => dest.SubtotalCurrency, opt => opt.MapFrom(src => src.Subtotal.Currency))
            .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount.Amount))
            .ForMember(dest => dest.TaxCurrency, opt => opt.MapFrom(src => src.TaxAmount.Currency))
            .ForMember(dest => dest.ShippingCostAmount, opt => opt.MapFrom(src => src.ShippingCost.Amount))
            .ForMember(dest => dest.ShippingCostCurrency, opt => opt.MapFrom(src => src.ShippingCost.Currency))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount.Amount))
            .ForMember(dest => dest.DiscountCurrency, opt => opt.MapFrom(src => src.DiscountAmount.Currency))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
            .ForMember(dest => dest.TotalCurrency, opt => opt.MapFrom(src => src.TotalAmount.Currency));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.UnitPriceAmount, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.UnitPriceCurrency, opt => opt.MapFrom(src => src.UnitPrice.Currency))
            .ForMember(dest => dest.TotalPriceAmount, opt => opt.MapFrom(src => src.TotalPrice.Amount))
            .ForMember(dest => dest.TotalPriceCurrency, opt => opt.MapFrom(src => src.TotalPrice.Currency));

        CreateMap<OrderItemDto, OrderItem>()
            .ConstructUsing(src => OrderItem.Create(
                src.ProductId,
                src.ProductName,
                src.ProductSku,
                src.Quantity,
                Money.Create(src.UnitPriceAmount, src.UnitPriceCurrency),
                src.Weight,
                src.Dimensions,
                src.Brand,
                src.Model,
                src.Color,
                src.Size));
    }
}
