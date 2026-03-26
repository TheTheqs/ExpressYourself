using ExpressYourself.Application.Interfaces;
using ExpressYourself.Domain.ValueObjects;

namespace ExpressYourself.Application.UseCases.GetIpInformation;

/// <summary>
/// Handles the retrieval of information for a given IP address directly from the database.
/// </summary>
public sealed class GetIpInformationUseCase
{
    private readonly IIpAddressRepository _ipAddressRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetIpInformationUseCase"/> class.
    /// </summary>
    /// <param name="ipAddressRepository">The IP address repository.</param>
    public GetIpInformationUseCase(IIpAddressRepository ipAddressRepository)
    {
        _ipAddressRepository = ipAddressRepository;
    }

    /// <summary>
    /// Retrieves information for a given IP address directly from the database.
    /// </summary>
    /// <param name="ip">The IP address to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// A response containing the IP and country information if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<GetIpInformationResponse?> ExecuteAsync(
        string ip,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            throw new ArgumentException("IP address cannot be null or empty.", nameof(ip));
        }

        var ipAddressValue = IpAddressValue.Create(ip);

        var result = await _ipAddressRepository.GetInformationByAddressAsync(
            ipAddressValue,
            cancellationToken);

        return result;
    }
}