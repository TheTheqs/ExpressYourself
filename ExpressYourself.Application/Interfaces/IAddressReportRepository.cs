using ExpressYourself.Application.UseCases.GetAddressReport;

namespace ExpressYourself.Application.Interfaces;

/// <summary>
/// Provides raw-query access for address report data.
/// </summary>
public interface IAddressReportRepository
{
    /// <summary>
    /// Retrieves the address report grouped by country.
    /// When country codes are provided, only matching countries are included.
    /// When the filter is null or empty, all countries are included.
    /// </summary>
    /// <param name="twoLetterCodes">Optional list of two-letter country codes used to filter the report.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection containing the report rows.</returns>
    Task<IReadOnlyCollection<GetAddressReportItemResponse>> GetReportAsync(
        IReadOnlyCollection<string>? twoLetterCodes,
        CancellationToken cancellationToken = default);
}