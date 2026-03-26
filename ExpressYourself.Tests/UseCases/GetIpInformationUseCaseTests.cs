using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.Models;
using ExpressYourself.Application.UseCases.GetIpInformation;
using ExpressYourself.Domain.Entities;
using ExpressYourself.Domain.ValueObjects;
using FluentAssertions;

namespace ExpressYourself.Tests.UseCases;

/// <summary>
/// Unit tests for <see cref="GetIpInformationUseCase"/>.
/// </summary>
public sealed class GetIpInformationUseCaseTests
{
    /// <summary>
    /// Should return cached response and avoid database and external provider access when the IP is already in cache.
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
        var provider = new FakeIpInformationProvider();
        var countryService = new FakeCountryService();

        var useCase = new GetIpInformationUseCase(
            repository,
            cacheService,
            provider,
            countryService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(cachedResponse);

        repository.GetInformationByAddressCallCount.Should().Be(0);
        repository.AddCallCount.Should().Be(0);
        repository.SaveChangesCallCount.Should().Be(0);

        provider.GetInformationCallCount.Should().Be(0);
        countryService.GetOrCreateCallCount.Should().Be(0);

        cacheService.SetCallCount.Should().Be(0);
    }

    /// <summary>
    /// Should query the database, cache the result and avoid the external provider when the IP is not found in cache.
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

        var provider = new FakeIpInformationProvider();
        var countryService = new FakeCountryService();

        var useCase = new GetIpInformationUseCase(
            repository,
            cacheService,
            provider,
            countryService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().NotBeNull();
        result!.Ip.Should().Be(ip);
        result.CountryName.Should().NotBeNullOrWhiteSpace();
        result.TwoLetterCode.Should().HaveLength(2);
        result.ThreeLetterCode.Should().HaveLength(3);

        repository.GetInformationByAddressCallCount.Should().Be(1);
        repository.AddCallCount.Should().Be(0);
        repository.SaveChangesCallCount.Should().Be(0);

        provider.GetInformationCallCount.Should().Be(0);
        countryService.GetOrCreateCallCount.Should().Be(0);

        cacheService.SetCallCount.Should().Be(1);
        cacheService.LastStoredIp.Should().Be(ip);
        cacheService.LastStoredResponse.Should().NotBeNull();
    }

    /// <summary>
    /// Should use the external provider, resolve country, persist the IP address and cache the result
    /// when the IP is not found in cache or database.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_UseProvider_ResolveCountry_PersistIp_AndCache_When_IpIsNotInCacheOrDatabase()
    {
        // Arrange
        var ip = "191.202.239.155";

        var providerResponse = new IpInformationProviderResponse
        {
            Ip = ip,
            CountryName = "Brazil",
            TwoLetterCode = "BR",
            ThreeLetterCode = "BRA"
        };

        var cacheService = new FakeCacheService();

        var repository = new FakeIpAddressRepository
        {
            ResponseToReturn = null
        };

        var provider = new FakeIpInformationProvider
        {
            ResponseToReturn = providerResponse
        };

        var countryService = new FakeCountryService
        {
            CountryToReturn = new Country("Brazil", "BR", "BRA")
        };

        var useCase = new GetIpInformationUseCase(
            repository,
            cacheService,
            provider,
            countryService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().NotBeNull();
        result!.Ip.Should().Be(ip);
        result.CountryName.Should().Be("Brazil");
        result.TwoLetterCode.Should().Be("BR");
        result.ThreeLetterCode.Should().Be("BRA");

        repository.GetInformationByAddressCallCount.Should().Be(1);
        repository.AddCallCount.Should().Be(1);
        repository.SaveChangesCallCount.Should().Be(1);
        repository.LastAddedIpAddress.Should().NotBeNull();
        repository.LastAddedIpAddress!.Address.Value.Should().Be(ip);

        provider.GetInformationCallCount.Should().Be(1);

        countryService.GetOrCreateCallCount.Should().Be(1);
        countryService.LastCountryName.Should().Be("Brazil");
        countryService.LastTwoLetterCode.Should().Be("BR");
        countryService.LastThreeLetterCode.Should().Be("BRA");

        cacheService.SetCallCount.Should().Be(1);
        cacheService.LastStoredIp.Should().Be(ip);
        cacheService.LastStoredResponse.Should().NotBeNull();
    }

    /// <summary>
    /// Should return null and avoid persistence and caching when the IP is not found in cache,
    /// database or external provider.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_ReturnNull_When_IpIsNotFoundInCacheDatabaseOrProvider()
    {
        // Arrange
        var ip = "201.202.203.204";

        var cacheService = new FakeCacheService();

        var repository = new FakeIpAddressRepository
        {
            ResponseToReturn = null
        };

        var provider = new FakeIpInformationProvider
        {
            ResponseToReturn = null
        };

        var countryService = new FakeCountryService();

        var useCase = new GetIpInformationUseCase(
            repository,
            cacheService,
            provider,
            countryService);

        // Act
        var result = await useCase.ExecuteAsync(ip);

        // Assert
        result.Should().BeNull();

        repository.GetInformationByAddressCallCount.Should().Be(1);
        repository.AddCallCount.Should().Be(0);
        repository.SaveChangesCallCount.Should().Be(0);

        provider.GetInformationCallCount.Should().Be(1);

        countryService.GetOrCreateCallCount.Should().Be(0);

        cacheService.SetCallCount.Should().Be(0);
    }

    /// <summary>
    /// Should throw an exception when the IP format is invalid.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_ThrowArgumentException_When_IpIsInvalid()
    {
        // Arrange
        var invalidIp = "abc.invalid.ip";

        var cacheService = new FakeCacheService();
        var repository = new FakeIpAddressRepository();
        var provider = new FakeIpInformationProvider();
        var countryService = new FakeCountryService();

        var useCase = new GetIpInformationUseCase(
            repository,
            cacheService,
            provider,
            countryService);

        // Act
        Func<Task> act = async () => await useCase.ExecuteAsync(invalidIp);

        // Assert
        await act.Should().ThrowAsync<Domain.Exceptions.DomainException>();

        repository.GetInformationByAddressCallCount.Should().Be(0);
        provider.GetInformationCallCount.Should().Be(0);
        countryService.GetOrCreateCallCount.Should().Be(0);
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

        public Task RemoveIpInformationAsync(
        string ip,
        CancellationToken cancellationToken = default)
        {
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
        public int AddCallCount { get; private set; }
        public int SaveChangesCallCount { get; private set; }
        public IpAddress? LastAddedIpAddress { get; private set; }

        public Task<IpAddress?> GetByAddressAsync(
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

        public Task<IReadOnlyCollection<IpAddress>> GetBatchAsync(
            int skip,
            int take,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(
            IpAddress ipAddress,
            CancellationToken cancellationToken = default)
        {
            AddCallCount++;
            LastAddedIpAddress = ipAddress;
            return Task.CompletedTask;
        }

        public void Update(IpAddress ipAddress)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Fake external provider used to control external lookup behavior during tests.
    /// </summary>
    private sealed class FakeIpInformationProvider : IIpInformationProvider
    {
        public IpInformationProviderResponse? ResponseToReturn { get; set; }
        public int GetInformationCallCount { get; private set; }

        public Task<IpInformationProviderResponse?> GetInformationAsync(
            string ip,
            CancellationToken cancellationToken = default)
        {
            GetInformationCallCount++;
            return Task.FromResult(ResponseToReturn);
        }
    }

    /// <summary>
    /// Fake country service used to control country resolution behavior during tests.
    /// </summary>
    private sealed class FakeCountryService : ICountryService
    {
        public int GetOrCreateCallCount { get; private set; }
        public string? LastCountryName { get; private set; }
        public string? LastTwoLetterCode { get; private set; }
        public string? LastThreeLetterCode { get; private set; }
        public Country CountryToReturn { get; set; } = new Country("Brazil", "BR", "BRA");

        public Task<Country> GetOrCreateAsync(
            string countryName,
            string twoLetterCode,
            string threeLetterCode,
            CancellationToken cancellationToken = default)
        {
            GetOrCreateCallCount++;
            LastCountryName = countryName;
            LastTwoLetterCode = twoLetterCode;
            LastThreeLetterCode = threeLetterCode;

            return Task.FromResult(CountryToReturn);
        }
    }
}