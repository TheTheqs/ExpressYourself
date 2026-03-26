namespace ExpressYourself.Application.UseCases.GetAddressReport;

/// <summary>
/// Represents a single row of the address report grouped by country.
/// </summary>
public sealed class GetAddressReportItemResponse
{
    /// <summary>
    /// Gets the country name.
    /// </summary>
    public string CountryName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the number of stored IP addresses associated with the country.
    /// </summary>
    public int AddressesCount { get; init; }

    /// <summary>
    /// Gets the most recent update timestamp among the stored IP addresses of the country.
    /// </summary>
    public DateTime LastAddressUpdated { get; init; }
}