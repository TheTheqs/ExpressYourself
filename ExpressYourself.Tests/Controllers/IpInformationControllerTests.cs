using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpressYourself.Tests.Controllers;

/// <summary>
/// Integration tests for the IP information endpoint.
/// </summary>
public sealed class IpInformationControllerTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpInformationControllerTests"/> class.
    /// </summary>
    /// <param name="factory">The web application factory used to host the API in memory.</param>
    public IpInformationControllerTests(WebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Should return 200 OK and a valid response body when the IP exists in the database.
    /// </summary>
    [Fact]
    public async Task Get_Should_ReturnOk_When_IpExists()
    {
        // Arrange
        var ip = "44.255.255.254";

        // Act
        var response = await _httpClient.GetAsync($"/api/IpInformation?ip={ip}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<IpInformationResponseTestModel>();

        content.Should().NotBeNull();
        content!.Ip.Should().NotBeNullOrWhiteSpace();
        content.CountryName.Should().NotBeNullOrWhiteSpace();
        content.TwoLetterCode.Should().HaveLength(2);
        content.ThreeLetterCode.Should().HaveLength(3);
    }

    /// <summary>
    /// Should return 400 Bad Request when the IP format is invalid.
    /// </summary>
    [Fact]
    public async Task Get_Should_ReturnBadRequest_When_IpFormatIsInvalid()
    {
        // Arrange
        var invalidIp = "abc.invalid.ip";

        // Act
        var response = await _httpClient.GetAsync($"/api/IpInformation?ip={invalidIp}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Should return 404 Not Found when the IP format is valid but the IP does not exist in the database.
    /// </summary>
    [Fact]
    public async Task Get_Should_ReturnNotFound_When_IpDoesNotExist()
    {
        // Arrange
        var nonExistingIp = "201.202.203.204";

        // Act
        var response = await _httpClient.GetAsync($"/api/IpInformation?ip={nonExistingIp}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Represents the response contract expected from the IP information endpoint during tests.
    /// </summary>
    private sealed class IpInformationResponseTestModel
    {
        /// <summary>
        /// Gets or sets the queried IP address.
        /// </summary>
        public string Ip { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country name.
        /// </summary>
        public string CountryName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the two-letter country code.
        /// </summary>
        public string TwoLetterCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the three-letter country code.
        /// </summary>
        public string ThreeLetterCode { get; set; } = string.Empty;
    }
}