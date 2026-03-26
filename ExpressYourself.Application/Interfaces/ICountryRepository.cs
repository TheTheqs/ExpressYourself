using ExpressYourself.Domain.Entities;

namespace ExpressYourself.Application.Interfaces;

/// <summary>
/// Provides persistence operations for <see cref="Country"/> entities.
/// </summary>
public interface ICountryRepository
{
    /// <summary>
    /// Retrieves a country by its two-letter code.
    /// </summary>
    /// <param name="twoLetterCode">The two-letter country code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Country"/> entity if found; otherwise, <c>null</c>.
    /// </returns>
    Task<Country?> GetByTwoLetterCodeAsync(
        string twoLetterCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new country entity to the persistence store.
    /// </summary>
    /// <param name="country">The country entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddAsync(
        Country country,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}