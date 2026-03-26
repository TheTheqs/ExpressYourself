using ExpressYourself.Application.UseCases.GetIpInformation;
using ExpressYourself.Domain.Entities;
using ExpressYourself.Domain.ValueObjects;

namespace ExpressYourself.Application.Interfaces;

/// <summary>
/// Provides persistence operations for <see cref="IpAddress"/> entities.
/// </summary>
public interface IIpAddressRepository
{
    /// <summary>
    /// Retrieves an IP address entity by its address value.
    /// </summary>
    /// <param name="address">The IP address value object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The matching <see cref="IpAddress"/> entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<IpAddress?> GetByAddressAsync(
        IpAddressValue address,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the complete information for a given IP address directly from the database.
    /// </summary>
    /// <param name="address">The IP address value object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A response projection containing the IP and country information if found; otherwise, <c>null</c>.
    /// </returns>
    Task<GetIpInformationResponse?> GetInformationByAddressAsync(
        IpAddressValue address,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a batch of IP address entities using pagination parameters.
    /// </summary>
    /// <param name="skip">The number of records to skip.</param>
    /// <param name="take">The number of records to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection of IP address entities.</returns>
    Task<IReadOnlyCollection<IpAddress>> GetBatchAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new IP address entity to the persistence store.
    /// </summary>
    /// <param name="ipAddress">The IP address entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(
        IpAddress ipAddress,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing IP address entity as updated.
    /// </summary>
    /// <param name="ipAddress">The IP address entity to update.</param>
    void Update(IpAddress ipAddress);

    /// <summary>
    /// Persists pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}