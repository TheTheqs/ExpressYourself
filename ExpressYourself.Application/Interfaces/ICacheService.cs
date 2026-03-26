using ExpressYourself.Application.UseCases.GetIpInformation;

namespace ExpressYourself.Application.Interfaces;

/// <summary>
/// Provides cache operations for IP information responses.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves cached IP information by the given IP address.
    /// </summary>
    /// <param name="ip">The IP address used as cache key reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The cached <see cref="GetIpInformationResponse"/> if found; otherwise, <c>null</c>.
    /// </returns>
    Task<GetIpInformationResponse?> GetIpInformationAsync(
        string ip,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores IP information in cache for the given IP address.
    /// </summary>
    /// <param name="ip">The IP address used as cache key reference.</param>
    /// <param name="response">The response object to cache.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetIpInformationAsync(
        string ip,
        GetIpInformationResponse response,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes cached IP information for the given IP address.
    /// </summary>
    /// <param name="ip">The IP address used as cache key reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RemoveIpInformationAsync(
        string ip,
        CancellationToken cancellationToken = default);
}