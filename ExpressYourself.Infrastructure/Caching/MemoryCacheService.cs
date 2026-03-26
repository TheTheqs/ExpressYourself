using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.UseCases.GetIpInformation;
using Microsoft.Extensions.Caching.Memory;

namespace ExpressYourself.Infrastructure.Caching;

/// <summary>
/// Provides in-memory cache operations for IP information responses.
/// </summary>
public sealed class MemoryCacheService : ICacheService
{
    private const string CacheKeyPrefix = "ip-information:";
    private static readonly TimeSpan SlidingExpiration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan AbsoluteExpiration = TimeSpan.FromHours(6);

    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheService"/> class.
    /// </summary>
    /// <param name="memoryCache">The in-memory cache instance.</param>
    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Retrieves cached IP information by the given IP address.
    /// </summary>
    /// <param name="ip">The IP address used as cache key reference.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The cached <see cref="GetIpInformationResponse"/> if found; otherwise, <c>null</c>.
    /// </returns>
    public Task<GetIpInformationResponse?> GetIpInformationAsync(
        string ip,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(ip);

        _memoryCache.TryGetValue(cacheKey, out GetIpInformationResponse? cachedResponse);

        return Task.FromResult(cachedResponse);
    }

    /// <summary>
    /// Stores IP information in cache for the given IP address.
    /// </summary>
    /// <param name="ip">The IP address used as cache key reference.</param>
    /// <param name="response">The response object to cache.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task SetIpInformationAsync(
        string ip,
        GetIpInformationResponse response,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(ip);

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = SlidingExpiration,
            AbsoluteExpirationRelativeToNow = AbsoluteExpiration
        };

        _memoryCache.Set(cacheKey, response, cacheEntryOptions);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Builds the cache key for a given IP address.
    /// </summary>
    /// <param name="ip">The IP address.</param>
    /// <returns>A formatted cache key.</returns>
    private static string BuildCacheKey(string ip)
    {
        return $"{CacheKeyPrefix}{ip}";
    }
}