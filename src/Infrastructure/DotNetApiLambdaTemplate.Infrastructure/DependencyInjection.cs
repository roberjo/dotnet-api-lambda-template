using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DotNetApiLambdaTemplate.Application.Common.Interfaces;
using DotNetApiLambdaTemplate.Infrastructure.Data;
using DotNetApiLambdaTemplate.Infrastructure.Repositories;

namespace DotNetApiLambdaTemplate.Infrastructure;

/// <summary>
/// Extension methods for configuring dependency injection for the Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework Core
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Add repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add caching
        services.AddMemoryCache();

        // Add HTTP client
        services.AddHttpClient();

        return services;
    }
}
