using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.UseCases.GetIpInformation;
using ExpressYourself.Domain.Entities;
using ExpressYourself.Domain.ValueObjects;
using ExpressYourself.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExpressYourself.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides Entity Framework Core persistence operations for <see cref="IpAddress"/> entities.
/// </summary>
public sealed class IpAddressRepository : IIpAddressRepository
{
    private readonly ExpressYourselfDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpAddressRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public IpAddressRepository(ExpressYourselfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves an IP address entity by its address value.
    /// </summary>
    /// <param name="address">The IP address value object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The matching <see cref="IpAddress"/> entity if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<IpAddress?> GetByAddressAsync(
        IpAddressValue address,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.IpAddresses
            .FirstOrDefaultAsync(
                ipAddress => ipAddress.Address == address,
                cancellationToken);
    }

    /// <summary>
    /// Retrieves the complete information for a given IP address directly from the database.
    /// </summary>
    /// <param name="address">The IP address value object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A response projection containing the IP and country information if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<GetIpInformationResponse?> GetInformationByAddressAsync(
        IpAddressValue address,
        CancellationToken cancellationToken = default)
    {
        return await (
            from ipAddress in _dbContext.IpAddresses.AsNoTracking()
            join country in _dbContext.Countries.AsNoTracking()
                on ipAddress.CountryId equals country.Id
            where ipAddress.Address == address
            select new GetIpInformationResponse
            {
                Ip = ipAddress.Address.Value,
                CountryName = country.Name,
                TwoLetterCode = country.TwoLetterCode,
                ThreeLetterCode = country.ThreeLetterCode
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a batch of IP address entities using pagination parameters.
    /// </summary>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection of IP address entities.</returns>
    public async Task<IReadOnlyCollection<IpAddress>> GetBatchAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.IpAddresses
            .AsNoTracking()
            .OrderBy(ipAddress => ipAddress.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new IP address entity to the persistence store.
    /// </summary>
    /// <param name="ipAddress">The IP address entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task AddAsync(
        IpAddress ipAddress,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.IpAddresses.AddAsync(ipAddress, cancellationToken);
    }

    /// <summary>
    /// Marks an existing IP address entity as updated.
    /// </summary>
    /// <param name="ipAddress">The IP address entity to update.</param>
    public void Update(IpAddress ipAddress)
    {
        _dbContext.IpAddresses.Update(ipAddress);
    }

    /// <summary>
    /// Persists pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}