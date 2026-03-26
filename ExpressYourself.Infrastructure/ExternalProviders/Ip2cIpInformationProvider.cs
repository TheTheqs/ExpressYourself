using System.Net;
using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.Models;

namespace ExpressYourself.Infrastructure.ExternalProviders;

/// <summary>
/// Provides external IP information lookup using the IP2C service.
/// </summary>
public sealed class Ip2cIpInformationProvider : IIpInformationProvider
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="Ip2cIpInformationProvider"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to call the external provider.</param>
    public Ip2cIpInformationProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves IP information from the external IP2C provider.
    /// </summary>
    /// <param name="ip">The IP address to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// An <see cref="IpInformationProviderResponse"/> containing the resolved information,
    /// or <c>null</c> when the provider cannot resolve the IP.
    /// </returns>
    public async Task<IpInformationProviderResponse?> GetInformationAsync(
        string ip,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            throw new ArgumentException("IP address cannot be null or empty.", nameof(ip));
        }

        if (!IPAddress.TryParse(ip, out var parsedIp))
        {
            throw new ArgumentException("Invalid IP address format.", nameof(ip));
        }

        if (parsedIp.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
        {
            return null;
        }

        using var response = await _httpClient.GetAsync(ip, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("The external IP provider returned an empty response.");
        }

        var parts = content.Trim().Split(';', StringSplitOptions.TrimEntries);

        if (parts.Length < 1)
        {
            throw new InvalidOperationException("The external IP provider returned an invalid response format.");
        }

        var status = parts[0];

        return status switch
        {
            "0" => throw new InvalidOperationException("The external IP provider rejected the request due to invalid input."),
            "1" => ParseSuccessResponse(ip, parts),
            "2" => null,
            _ => throw new InvalidOperationException("The external IP provider returned an unknown response status.")
        };
    }

    /// <summary>
    /// Parses a successful IP2C response into a provider response model.
    /// </summary>
    /// <param name="ip">The queried IP address.</param>
    /// <param name="parts">The semicolon-delimited response parts.</param>
    /// <returns>A populated <see cref="IpInformationProviderResponse"/> instance.</returns>
    private static IpInformationProviderResponse ParseSuccessResponse(string ip, string[] parts)
    {
        if (parts.Length < 4)
        {
            throw new InvalidOperationException("The external IP provider returned an incomplete success response.");
        }

        return new IpInformationProviderResponse
        {
            Ip = ip,
            TwoLetterCode = parts[1],
            ThreeLetterCode = parts[2],
            CountryName = parts[3]
        };
    }
}