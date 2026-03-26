using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.UseCases.GetIpInformation;
using ExpressYourself.Domain.ValueObjects;
using FluentAssertions;

namespace ExpressYourself.Tests.UseCases;

/// <summary>
/// Unit tests for <see cref="GetIpInformationUseCase"/>.
/// </summary>
public sealed class GetIpInformationUseCaseTests
{
    /// <summary>
    /// Should return cached response and avoid database access when the IP is already in cache.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_ReturnCachedResponse_When_IpExistsInCache()
    {
        // Arrange
        var ip = "44.255.255.254";

        var cachedResponse = new GetIpInformationResponse
        {
            Ip = ip,
            CountryName = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC"
        };

        var cacheService = new FakeCacheService
        {
            CachedResponse = cachedResponse
        };

        var repository = new FakeIpAddressRepository();

        var useCase = new GetIpInformationUseCase(repository, cacheService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(cachedResponse);

        repository.GetInformationByAddressCallCount.Should().Be(0);
        cacheService.SetCallCount.Should().Be(0);
    }

    /// <summary>
    /// Should query the database, cache the result and return it when the IP is not found in cache.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_QueryDatabase_AndCacheResult_When_IpIsNotInCache()
    {
        // Arrange
        var ip = "44.255.255.254";

        var databaseResponse = new GetIpInformationResponse
        {
            Ip = ip,
            CountryName = "Greece",
            TwoLetterCode = "GR",
            ThreeLetterCode = "GRC"
        };

        var cacheService = new FakeCacheService();

        var repository = new FakeIpAddressRepository
        {
            ResponseToReturn = databaseResponse
        };

        var useCase = new GetIpInformationUseCase(repository, cacheService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().NotBeNull();
        result!.Ip.Should().Be(ip);
        result.CountryName.Should().NotBeNullOrWhiteSpace();
        result.TwoLetterCode.Should().HaveLength(2);
        result.ThreeLetterCode.Should().HaveLength(3);

        repository.GetInformationByAddressCallCount.Should().Be(1);
        cacheService.SetCallCount.Should().Be(1);
        cacheService.LastStoredIp.Should().Be(ip);
        cacheService.LastStoredResponse.Should().NotBeNull();
    }

    /// <summary>
    /// Should return null and avoid cache storage when the IP is not found in cache or database.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_ReturnNull_When_IpIsNotInCacheOrDatabase()
    {
        // Arrange
        var ip = "201.202.203.204";

        var cacheService = new FakeCacheService();

        var repository = new FakeIpAddressRepository
        {
            ResponseToReturn = null
        };

        var useCase = new GetIpInformationUseCase(repository, cacheService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().BeNull();

        repository.GetInformationByAddressCallCount.Should().Be(1);
        cacheService.SetCallCount.Should().Be(0);
    }

    /// <summary>
    /// Fake cache service used to control cache behavior during tests.
    /// </summary>
    private sealed class FakeCacheService : ICacheService
    {
        public GetIpInformationResponse? CachedResponse { get; set; }
        public int SetCallCount { get; private set; }
        public string? LastStoredIp { get; private set; }
        public GetIpInformationResponse? LastStoredResponse { get; private set; }

        public Task<GetIpInformationResponse?> GetIpInformationAsync(
            string ip,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CachedResponse);
        }

        public Task SetIpInformationAsync(
            string ip,
            GetIpInformationResponse response,
            CancellationToken cancellationToken = default)
        {
            SetCallCount++;
            LastStoredIp = ip;
            LastStoredResponse = response;

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Fake repository used to control database behavior during tests.
    /// </summary>
    private sealed class FakeIpAddressRepository : IIpAddressRepository
    {
        public GetIpInformationResponse? ResponseToReturn { get; set; }
        public int GetInformationByAddressCallCount { get; private set; }

        public Task<Domain.Entities.IpAddress?> GetByAddressAsync(
            IpAddressValue address,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<GetIpInformationResponse?> GetInformationByAddressAsync(
            IpAddressValue address,
            CancellationToken cancellationToken = default)
        {
            GetInformationByAddressCallCount++;

            return Task.FromResult(ResponseToReturn);
        }

        public Task<IReadOnlyCollection<Domain.Entities.IpAddress>> GetBatchAsync(
            int skip,
            int take,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(
            Domain.Entities.IpAddress ipAddress,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Update(Domain.Entities.IpAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}