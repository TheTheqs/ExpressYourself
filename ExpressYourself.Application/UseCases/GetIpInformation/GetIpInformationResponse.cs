namespace ExpressYourself.Application.UseCases.GetIpInformation;

/// <summary>
/// Represents the response returned when querying information for an IP address.
/// </summary>
public sealed class GetIpInformationResponse
{
    /// <summary>
    /// Gets the queried IP address.
    /// </summary>
    public string Ip { get; init; } = string.Empty;

    /// <summary>
    /// Gets the country name associated with the IP address.
    /// </summary>
    public string CountryName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the two-letter country code associated with the IP address.
    /// </summary>
    public string TwoLetterCode { get; init; } = string.Empty;

    /// <summary>
    /// Gets the three-letter country code associated with the IP address.
    /// </summary>
    public string ThreeLetterCode { get; init; } = string.Empty;
}