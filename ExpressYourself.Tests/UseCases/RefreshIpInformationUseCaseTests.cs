using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.Models;
using ExpressYourself.Application.UseCases.GetIpInformation;
using ExpressYourself.Application.UseCases.RefreshIpInformation;
using ExpressYourself.Domain.Entities;
using ExpressYourself.Domain.ValueObjects;
using FluentAssertions;

namespace ExpressYourself.Tests.UseCases;

/// <summary>
/// Unit tests for <see cref="RefreshIpInformationUseCase"/>.
/// </summary>
public sealed class RefreshIpInformationUseCaseTests
{
    /// <summary>
    /// Should count IPs as unchanged when the resolved country matches the current country.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_CountAsUnchanged_When_CountryDoesNotChange()
    {
        // Arrange
        var ipAddress = new IpAddress(1, IpAddressValue.Create("44.255.255.254"));

        var repository = new FakeIpAddressRepository([[ipAddress]]);
        var provider = new FakeIpInformationProvider
        {
            ResponseToReturn = new IpInformationProviderResponse
            {
                Ip = "44.255.255.254",
                CountryName = "Greece",
                TwoLetterCode = "GR",
                ThreeLetterCode = "GRC"
            }
        };

        var countryService = new FakeCountryService
        {
            CountryToReturn = CreateCountryWithId(1, "Greece", "GR", "GRC")
        };

        var cacheService = new FakeCacheService();

        var useCase = new RefreshIpInformationUseCase(
            repository,
            provider,
            countryService,
            cacheService);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.ProcessedCount.Should().Be(1);
        result.UpdatedCount.Should().Be(0);
        result.UnchangedCount.Should().Be(1);
        result.NotResolvedCount.Should().Be(0);

        repository.UpdateCallCount.Should().Be(0);
        repository.SaveChangesCallCount.Should().Be(0);
        cacheService.RemoveCallCount.Should().Be(0);
    }

    /// <summary>
    /// Should update the IP country and invalidate cache when the resolved country changes.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_UpdateCountry_AndInvalidateCache_When_CountryChanges()
    {
        // Arrange
        var ipAddress = new IpAddress(1, IpAddressValue.Create("191.202.239.155"));

        var repository = new FakeIpAddressRepository([[ipAddress]]);
        var provider = new FakeIpInformationProvider
        {
            ResponseToReturn = new IpInformationProviderResponse
            {
                Ip = "191.202.239.155",
                CountryName = "Brazil",
                TwoLetterCode = "BR",
                ThreeLetterCode = "BRA"
            }
        };

        var countryService = new FakeCountryService
        {
            CountryToReturn = CreateCountryWithId(2, "Brazil", "BR", "BRA")
        };

        var cacheService = new FakeCacheService();

        var useCase = new RefreshIpInformationUseCase(
            repository,
            provider,
            countryService,
            cacheService);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.ProcessedCount.Should().Be(1);
        result.UpdatedCount.Should().Be(1);
        result.UnchangedCount.Should().Be(0);
        result.NotResolvedCount.Should().Be(0);

        repository.UpdateCallCount.Should().Be(1);
        repository.SaveChangesCallCount.Should().Be(1);
        cacheService.RemoveCallCount.Should().Be(1);
        cacheService.LastRemovedIp.Should().Be("191.202.239.155");
    }

    /// <summary>
    /// Should count IPs as not resolved when the external provider cannot resolve them.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_CountAsNotResolved_When_ProviderReturnsNull()
    {
        // Arrange
        var ipAddress = new IpAddress(1, IpAddressValue.Create("10.20.30.40"));

        var repository = new FakeIpAddressRepository([[ipAddress]]);
        var provider = new FakeIpInformationProvider
        {
            ResponseToReturn = null
        };

        var countryService = new FakeCountryService();
        var cacheService = new FakeCacheService();

        var useCase = new RefreshIpInformationUseCase(
            repository,
            provider,
            countryService,
            cacheService);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.ProcessedCount.Should().Be(1);
        result.UpdatedCount.Should().Be(0);
        result.UnchangedCount.Should().Be(0);
        result.NotResolvedCount.Should().Be(1);

        repository.UpdateCallCount.Should().Be(0);
        repository.SaveChangesCallCount.Should().Be(0);
        countryService.GetOrCreateCallCount.Should().Be(0);
        cacheService.RemoveCallCount.Should().Be(0);
    }

    private static Country CreateCountryWithId(int id, string name, string twoLetterCode, string threeLetterCode)
    {
        var country = new Country(name, twoLetterCode, threeLetterCode);

        typeof(Country)
            .GetProperty(nameof(Country.Id))!
            .SetValue(country, id);

        return country;
    }

    private sealed class FakeIpAddressRepository : IIpAddressRepository
    {
        private readonly Queue<IReadOnlyCollection<IpAddress>> _batches;

        public int UpdateCallCount { get; private set; }
        public int SaveChangesCallCount { get; private set; }

        public FakeIpAddressRepository(IEnumerable<IReadOnlyCollection<IpAddress>> batches)
        {
            _batches = new Queue<IReadOnlyCollection<IpAddress>>(batches);
        }

        public Task<IpAddress?> GetByAddressAsync(IpAddressValue address, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<GetIpInformationResponse?> GetInformationByAddressAsync(IpAddressValue address, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<IReadOnlyCollection<IpAddress>> GetBatchAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            if (_batches.Count == 0)
            {
                return Task.FromResult<IReadOnlyCollection<IpAddress>>([]);
            }

            return Task.FromResult(_batches.Dequeue());
        }

        public Task AddAsync(IpAddress ipAddress, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public void Update(IpAddress ipAddress)
        {
            UpdateCallCount++;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeIpInformationProvider : IIpInformationProvider
    {
        public IpInformationProviderResponse? ResponseToReturn { get; set; }

        public Task<IpInformationProviderResponse?> GetInformationAsync(string ip, CancellationToken cancellationToken = default)
            => Task.FromResult(ResponseToReturn);
    }

    private sealed class FakeCountryService : ICountryService
    {
        public int GetOrCreateCallCount { get; private set; }
        public Country CountryToReturn { get; set; } = new Country("Brazil", "BR", "BRA");

        public Task<Country> GetOrCreateAsync(string countryName, string twoLetterCode, string threeLetterCode, CancellationToken cancellationToken = default)
        {
            GetOrCreateCallCount++;
            return Task.FromResult(CountryToReturn);
        }
    }

    private sealed class FakeCacheService : ICacheService
    {
        public int RemoveCallCount { get; private set; }
        public string? LastRemovedIp { get; private set; }

        public Task<GetIpInformationResponse?> GetIpInformationAsync(string ip, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task SetIpInformationAsync(string ip, GetIpInformationResponse response, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task RemoveIpInformationAsync(string ip, CancellationToken cancellationToken = default)
        {
            RemoveCallCount++;
            LastRemovedIp = ip;
            return Task.CompletedTask;
        }
    }
}