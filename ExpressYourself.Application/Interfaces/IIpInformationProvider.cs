using ExpressYourself.Application.Models;

namespace ExpressYourself.Application.Interfaces;

/// <summary>
/// Provides external IP information lookup operations.
/// </summary>
public interface IIpInformationProvider
{
    /// <summary>
    /// Retrieves IP information from an external provider.
    /// </summary>
    /// <param name="ip">The IP address to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An <see cref="IpInformationProviderResponse"/> containing the resolved information,
    /// or <c>null</c> when the provider cannot resolve the IP.
    /// </returns>
    Task<IpInformationProviderResponse?> GetInformationAsync(
        string ip,
        CancellationToken cancellationToken = default);
}