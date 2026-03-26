using ExpressYourself.Application.Interfaces;
using ExpressYourself.Domain.ValueObjects;

namespace ExpressYourself.Application.UseCases.GetIpInformation;

/// <summary>
/// Handles the retrieval of information for a given IP address using cache-first strategy.
/// </summary>
public sealed class GetIpInformationUseCase
{
    private readonly IIpAddressRepository _ipAddressRepository;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetIpInformationUseCase"/> class.
    /// </summary>
    /// <param name="ipAddressRepository">The IP address repository.</param>
    /// <param name="cacheService">The cache service used to store and retrieve IP information.</param>
    public GetIpInformationUseCase(
        IIpAddressRepository ipAddressRepository,
        ICacheService cacheService)
    {
        _ipAddressRepository = ipAddressRepository;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Retrieves information for a given IP address using cache-first lookup.
    /// If not found in cache, the database is queried and the result is cached for future requests.
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

        var cachedResult = await _cacheService.GetIpInformationAsync(
            ipAddressValue.Value,
            cancellationToken);

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var databaseResult = await _ipAddressRepository.GetInformationByAddressAsync(
            ipAddressValue,
            cancellationToken);

        if (databaseResult is null)
        {
            return null;
        }

        await _cacheService.SetIpInformationAsync(
            ipAddressValue.Value,
            databaseResult,
            cancellationToken);

        return databaseResult;
    }
}