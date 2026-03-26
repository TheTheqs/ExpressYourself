using ExpressYourself.Application.Interfaces;
using ExpressYourself.Application.UseCases.GetAddressReport;
using FluentAssertions;

namespace ExpressYourself.Tests.UseCases;

/// <summary>
/// Unit tests for <see cref="GetAddressReportUseCase"/>.
/// </summary>
public sealed class GetAddressReportUseCaseTests
{
    /// <summary>
    /// Should call the repository with null when no filter is provided.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_CallRepositoryWithNull_When_FilterIsNull()
    {
        // Arrange
        var expectedResponse = new List<GetAddressReportItemResponse>
        {
            new()
            {
                CountryName = "Brazil",
                AddressesCount = 10,
                LastAddressUpdated = DateTime.UtcNow
            }
        };

        var repository = new FakeAddressReportRepository
        {
            ResponseToReturn = expectedResponse
        };

        var useCase = new GetAddressReportUseCase(repository);

        // Act
        var result = await useCase.ExecuteAsync(null);

        // Assert
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(expectedResponse);

        repository.GetReportCallCount.Should().Be(1);
        repository.LastReceivedCodes.Should().BeNull();
    }

    /// <summary>
    /// Should normalize, uppercase and remove duplicates from the provided filter codes.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_NormalizeCodes_When_FilterIsProvided()
    {
        // Arrange
        var repository = new FakeAddressReportRepository
        {
            ResponseToReturn = new List<GetAddressReportItemResponse>()
        };

        var useCase = new GetAddressReportUseCase(repository);

        var inputCodes = new[] { " br ", "us", "BR", " Us " };

        // Act
        await useCase.ExecuteAsync(inputCodes);

        // Assert
        repository.GetReportCallCount.Should().Be(1);
        repository.LastReceivedCodes.Should().NotBeNull();
        repository.LastReceivedCodes.Should().BeEquivalentTo(["BR", "US"]);
    }

    /// <summary>
    /// Should ignore blank or whitespace values from the provided filter codes.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_Should_IgnoreBlankCodes_When_FilterContainsInvalidValues()
    {
        // Arrange
        var repository = new FakeAddressReportRepository
        {
            ResponseToReturn = new List<GetAddressReportItemResponse>()
        };

        var useCase = new GetAddressReportUseCase(repository);

        var inputCodes = new[] { " ", "", " br ", "   " };

        // Act
        await useCase.ExecuteAsync(inputCodes);

        // Assert
        repository.GetReportCallCount.Should().Be(1);
        repository.LastReceivedCodes.Should().NotBeNull();
        repository.LastReceivedCodes.Should().BeEquivalentTo(["BR"]);
    }

    /// <summary>
    /// Fake repository used to control report query behavior during tests.
    /// </summary>
    private sealed class FakeAddressReportRepository : IAddressReportRepository
    {
        public IReadOnlyCollection<GetAddressReportItemResponse> ResponseToReturn { get; set; }
            = Array.Empty<GetAddressReportItemResponse>();

        public int GetReportCallCount { get; private set; }

        public IReadOnlyCollection<string>? LastReceivedCodes { get; private set; }

        public Task<IReadOnlyCollection<GetAddressReportItemResponse>> GetReportAsync(
            IReadOnlyCollection<string>? twoLetterCodes,
            CancellationToken cancellationToken = default)
        {
            GetReportCallCount++;
            LastReceivedCodes = twoLetterCodes;

            return Task.FromResult(ResponseToReturn);
        }
    }
}