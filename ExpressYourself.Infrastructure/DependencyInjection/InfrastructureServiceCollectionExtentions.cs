using ExpressYourself.Application.Interfaces;
using ExpressYourself.Infrastructure.Caching;
using ExpressYourself.Infrastructure.ExternalProviders;
using ExpressYourself.Infrastructure.Persistence;
using ExpressYourself.Infrastructure.Persistence.Repositories;
using ExpressYourself.Infrastructure.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressYourself.Infrastructure.DependencyInjection;

/// <summary>
/// Provides dependency injection registration methods for infrastructure services.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
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

        //Reporting (RawSQL Repository)
        services.AddScoped<IAddressReportRepository>(_ =>
            new AddressReportRepository(connectionString));

        // Caching
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // Providers
        services.AddHttpClient<IIpInformationProvider, Ip2cIpInformationProvider>(client =>
        {
            client.BaseAddress = new Uri("https://ip2c.org/");
        });

        // Repositories
        services.AddScoped<IIpAddressRepository, IpAddressRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();

        return services;
    }
}