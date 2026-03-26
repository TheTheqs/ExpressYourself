using ExpressYourself.Application.Interfaces;

namespace ExpressYourself.Application.UseCases.RefreshIpInformation;

/// <summary>
/// Refreshes stored IP information in batches using the external IP information provider.
/// </summary>
public sealed class RefreshIpInformationUseCase
{
    private const int BatchSize = 100;

    private readonly IIpAddressRepository _ipAddressRepository;
    private readonly IIpInformationProvider _ipInformationProvider;
    private readonly ICountryService _countryService;
    private readonly ICacheService _cacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshIpInformationUseCase"/> class.
    /// </summary>
    /// <param name="ipAddressRepository">The IP address repository.</param>
    /// <param name="ipInformationProvider">The external IP information provider.</param>
    /// <param name="countryService">The country service used to resolve or create countries.</param>
    /// <param name="cacheService">The cache service used to invalidate outdated IP information.</param>
    public RefreshIpInformationUseCase(
        IIpAddressRepository ipAddressRepository,
        IIpInformationProvider ipInformationProvider,
        ICountryService countryService,
        ICacheService cacheService)
    {
        _ipAddressRepository = ipAddressRepository;
        _ipInformationProvider = ipInformationProvider;
        _countryService = countryService;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Refreshes stored IP information in batches of 100 and updates the database when changes are detected.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A summary of the refresh execution.</returns>
    public async Task<RefreshIpInformationResponse> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var processedCount = 0;
        var updatedCount = 0;
        var unchangedCount = 0;
        var notResolvedCount = 0;

        var skip = 0;

        while (true)
        {
            var ipBatch = await _ipAddressRepository.GetBatchAsync(
                skip,
                BatchSize,
                cancellationToken);

            if (ipBatch.Count == 0)
            {
                break;
            }

            var hasPendingUpdates = false;

            foreach (var ipAddress in ipBatch)
            {
                cancellationToken.ThrowIfCancellationRequested();

                processedCount++;

                var providerResult = await _ipInformationProvider.GetInformationAsync(
                    ipAddress.Address.Value,
                    cancellationToken);

                if (providerResult is null)
                {
                    notResolvedCount++;
                    continue;
                }

                var country = await _countryService.GetOrCreateAsync(
                    providerResult.CountryName,
                    providerResult.TwoLetterCode,
                    providerResult.ThreeLetterCode,
                    cancellationToken);

                if (country.Id == ipAddress.CountryId)
                {
                    unchangedCount++;
                    continue;
                }

                ipAddress.UpdateCountry(country.Id);
                _ipAddressRepository.Update(ipAddress);

                await _cacheService.RemoveIpInformationAsync(
                    ipAddress.Address.Value,
                    cancellationToken);

                updatedCount++;
                hasPendingUpdates = true;
            }

            if (hasPendingUpdates)
            {
                await _ipAddressRepository.SaveChangesAsync(cancellationToken);
            }

            if (ipBatch.Count < BatchSize)
            {
                break;
            }

            skip += BatchSize;
        }

        return new RefreshIpInformationResponse
        {
            ProcessedCount = processedCount,
            UpdatedCount = updatedCount,
            UnchangedCount = unchangedCount,
            NotResolvedCount = notResolvedCount
        };
    }
}