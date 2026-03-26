using ExpressYourself.Application.Interfaces;
using ExpressYourself.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpressYourself.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides Entity Framework Core persistence operations for <see cref="Country"/> entities.
/// </summary>
public sealed class CountryRepository : ICountryRepository
{
    private readonly ExpressYourselfDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CountryRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public CountryRepository(ExpressYourselfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a country by its two-letter code.
    /// </summary>
    /// <param name="twoLetterCode">The two-letter country code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Country"/> entity if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<Country?> GetByTwoLetterCodeAsync(
        string twoLetterCode,
        CancellationToken cancellationToken = default)
    {
        var normalizedTwoLetterCode = twoLetterCode.Trim().ToUpperInvariant();

        return await _dbContext.Countries
            .FirstOrDefaultAsync(
                country => country.TwoLetterCode == normalizedTwoLetterCode,
                cancellationToken);
    }

    /// <summary>
    /// Adds a new country entity to the persistence store.
    /// </summary>
    /// <param name="country">The country entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task AddAsync(
        Country country,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Countries.AddAsync(country, cancellationToken);
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