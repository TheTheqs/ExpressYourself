using ExpressYourself.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpressYourself.Api.Controllers;

/// <summary>
/// Provides temporary endpoints to validate database connectivity and mappings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class DatabaseTestController : ControllerBase
{
    private readonly ExpressYourselfDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseTestController"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public DatabaseTestController(ExpressYourselfDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns a small list of countries from the database to validate connectivity.
    /// </summary>
    /// <returns>A list of countries loaded from the database.</returns>
    [HttpGet("countries")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
    {
        var countries = await _dbContext.Countries
            .AsNoTracking()
            .OrderBy(country => country.Id)
            .Select(country => new
            {
                country.Id,
                country.Name,
                country.TwoLetterCode,
                country.ThreeLetterCode,
                country.CreatedAt
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        return Ok(countries);
    }
}