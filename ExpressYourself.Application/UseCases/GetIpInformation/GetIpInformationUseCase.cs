using ExpressYourself.Application.Interfaces;
using ExpressYourself.Domain.Entities;
using ExpressYourself.Domain.ValueObjects;

namespace ExpressYourself.Application.UseCases.GetIpInformation;

/// <summary>
/// Handles the retrieval of information for a given IP address using cache-first strategy.
/// If not found in cache, the database is queried. If still not found, an external provider is used.
/// </summary>
public sealed class GetIpInformationUseCase
{
    private readonly IIpAddressRepository _ipAddressRepository;
    private readonly ICacheService _cacheService;
    private readonly IIpInformationProvider _ipInformationProvider;
    private readonly ICountryService _countryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetIpInformationUseCase"/> class.
    /// </summary>
    /// <param name="ipAddressRepository">The IP address repository.</param>
    /// <param name="cacheService">The cache service used to store and retrieve IP information.</param>
    /// <param name="ipInformationProvider">The external IP information provider.</param>
    /// <param name="countryService">The country service responsible for resolving or creating countries.</param>
    public GetIpInformationUseCase(
        IIpAddressRepository ipAddressRepository,
        ICacheService cacheService,
        IIpInformationProvider ipInformationProvider,
        ICountryService countryService)
    {
        _ipAddressRepository = ipAddressRepository;
        _cacheService = cacheService;
        _ipInformationProvider = ipInformationProvider;
        _countryService = countryService;
    }

    /// <summary>
    /// Retrieves information for a given IP address using cache, database and external provider fallback.
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

        if (databaseResult is not null)
        {
            await _cacheService.SetIpInformationAsync(
                ipAddressValue.Value,
                databaseResult,
                cancellationToken);

            return databaseResult;
        }

        var providerResult = await _ipInformationProvider.GetInformationAsync(
            ipAddressValue.Value,
            cancellationToken);

        if (providerResult is null)
        {
            return null;
        }

        var country = await _countryService.GetOrCreateAsync(
            providerResult.CountryName,
            providerResult.TwoLetterCode,
            providerResult.ThreeLetterCode,
            cancellationToken);

        var newIpAddress = new IpAddress(country.Id, ipAddressValue);

        await _ipAddressRepository.AddAsync(newIpAddress, cancellationToken);
        await _ipAddressRepository.SaveChangesAsync(cancellationToken);

        var response = new GetIpInformationResponse
        {
            Ip = providerResult.Ip,
            CountryName = providerResult.CountryName,
            TwoLetterCode = providerResult.TwoLetterCode,
            ThreeLetterCode = providerResult.ThreeLetterCode
        };

        await _cacheService.SetIpInformationAsync(
            ipAddressValue.Value,
            response,
            cancellationToken);

        return response;
    }
}