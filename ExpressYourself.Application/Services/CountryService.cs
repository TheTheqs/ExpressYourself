using ExpressYourself.Application.Interfaces;
using ExpressYourself.Domain.Entities;

namespace ExpressYourself.Application.Services;

/// <summary>
/// Provides country resolution and creation operations for application workflows.
/// </summary>
public sealed class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CountryService"/> class.
    /// </summary>
    /// <param name="countryRepository">The country repository.</param>
    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }

    /// <summary>
    /// Retrieves an existing country by its two-letter code, or creates and persists it if it does not exist.
    /// </summary>
    /// <param name="countryName">The country name.</param>
    /// <param name="twoLetterCode">The two-letter country code.</param>
    /// <param name="threeLetterCode">The three-letter country code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The resolved <see cref="Country"/> entity.</returns>
    public async Task<Country> GetOrCreateAsync(
        string countryName,
        string twoLetterCode,
        string threeLetterCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedTwoLetterCode = twoLetterCode.Trim().ToUpperInvariant();

        var existingCountry = await _countryRepository.GetByTwoLetterCodeAsync(
            normalizedTwoLetterCode,
            cancellationToken);

        if (existingCountry is not null)
        {
            return existingCountry;
        }

        var newCountry = new Country(
            countryName,
            normalizedTwoLetterCode,
            threeLetterCode);

        await _countryRepository.AddAsync(newCountry, cancellationToken);
        await _countryRepository.SaveChangesAsync(cancellationToken);

        return newCountry;
    }
}