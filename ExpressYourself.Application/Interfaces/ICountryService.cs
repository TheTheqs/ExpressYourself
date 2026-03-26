using ExpressYourself.Domain.Entities;

namespace ExpressYourself.Application.Interfaces;

/// <summary>
/// Provides application operations for resolving and ensuring country existence.
/// </summary>
public interface ICountryService
{
    /// <summary>
    /// Retrieves an existing country by its two-letter code, or creates and persists it if it does not exist.
    /// </summary>
    /// <param name="countryName">The country name.</param>
    /// <param name="twoLetterCode">The two-letter country code.</param>
    /// <param name="threeLetterCode">The three-letter country code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resolved <see cref="Country"/> entity.</returns>
    Task<Country> GetOrCreateAsync(
        string countryName,
        string twoLetterCode,
        string threeLetterCode,
        CancellationToken cancellationToken = default);
}