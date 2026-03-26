using ExpressYourself.Application.Interfaces;

namespace ExpressYourself.Application.UseCases.GetAddressReport;

/// <summary>
/// Handles the retrieval of the address report grouped by country.
/// </summary>
public sealed class GetAddressReportUseCase
{
    private readonly IAddressReportRepository _addressReportRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAddressReportUseCase"/> class.
    /// </summary>
    /// <param name="addressReportRepository">The repository responsible for retrieving report data.</param>
    public GetAddressReportUseCase(IAddressReportRepository addressReportRepository)
    {
        _addressReportRepository = addressReportRepository;
    }

    /// <summary>
    /// Retrieves the address report grouped by country.
    /// When country codes are provided, only matching countries are returned.
    /// When the filter is null or empty, all countries are returned.
    /// </summary>
    /// <param name="twoLetterCodes">Optional list of two-letter country codes used to filter the report.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection containing the report rows.</returns>
    public async Task<IReadOnlyCollection<GetAddressReportItemResponse>> ExecuteAsync(
        IReadOnlyCollection<string>? twoLetterCodes,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<string>? normalizedCodes = null;

        if (twoLetterCodes is not null && twoLetterCodes.Count > 0)
        {
            normalizedCodes = twoLetterCodes
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code.Trim().ToUpperInvariant())
                .Distinct()
                .ToArray();
        }

        var result = await _addressReportRepository.GetReportAsync(
            normalizedCodes,
            cancellationToken);

        return result;
    }
}