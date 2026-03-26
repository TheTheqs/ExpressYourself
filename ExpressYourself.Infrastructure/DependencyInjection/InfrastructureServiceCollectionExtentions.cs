using ExpressYourself.Application.Interfaces;
using ExpressYourself.Infrastructure.Caching;
using ExpressYourself.Infrastructure.Persistence;
using ExpressYourself.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressYourself.Infrastructure.DependencyInjection;

/// <summary>
/// Provides dependency injection registration methods for infrastructure services.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers infrastructure services and database access dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the default database connection string is not configured.
    /// </exception>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The connection string 'DefaultConnection' was not found.");
        }

        // DbContext
        services.AddDbContext<ExpressYourselfDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        // Caching
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // Repositories
        services.AddScoped<IIpAddressRepository, IpAddressRepository>();

        return services;
    }
}