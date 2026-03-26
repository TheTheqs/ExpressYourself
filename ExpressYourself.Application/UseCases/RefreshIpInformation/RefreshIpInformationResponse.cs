namespace ExpressYourself.Application.UseCases.RefreshIpInformation;

/// <summary>
/// Represents the result of a refresh operation for stored IP information.
/// </summary>
public sealed class RefreshIpInformationResponse
{
    /// <summary>
    /// Gets the total number of processed IP addresses.
    /// </summary>
    public int ProcessedCount { get; init; }

    /// <summary>
    /// Gets the number of IP addresses whose country information was updated.
    /// </summary>
    public int UpdatedCount { get; init; }

    /// <summary>
    /// Gets the number of IP addresses whose country information remained unchanged.
    /// </summary>
    public int UnchangedCount { get; init; }

    /// <summary>
    /// Gets the number of IP addresses that could not be resolved by the external provider.
    /// </summary>
    public int NotResolvedCount { get; init; }
}