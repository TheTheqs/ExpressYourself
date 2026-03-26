using ExpressYourself.Application.UseCases.GetAddressReport;
using ExpressYourself.Application.UseCases.GetIpInformation;
using ExpressYourself.Application.UseCases.RefreshIpInformation;
using Microsoft.AspNetCore.Mvc;

namespace ExpressYourself.Api.Controllers;

/// <summary>
/// Provides endpoints for retrieving IP address information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class IpInformationController : ControllerBase
{
    private readonly GetIpInformationUseCase _getIpInformationUseCase;
    private readonly GetAddressReportUseCase _getAddressReportUseCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpInformationController"/> class.
    /// </summary>
    /// <param name="getIpInformationUseCase">The use case responsible for retrieving IP information.</param>
    /// <param name="getAddressReportUseCase">The use case responsible for retrieving the address report.</param>
    public IpInformationController(GetIpInformationUseCase getIpInformationUseCase, GetAddressReportUseCase getAddressReportUseCase)
    {
        _getIpInformationUseCase = getIpInformationUseCase;
        _getAddressReportUseCase = getAddressReportUseCase;
    }

    /// <summary>
    /// Retrieves country information for a given IP address.
    /// </summary>
    /// <param name="ip">The IP address to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The IP information when found.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetIpInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(
        [FromQuery] string ip,
        CancellationToken cancellationToken)
    {
        var result = await _getIpInformationUseCase.ExecuteAsync(ip, cancellationToken);

        if (result is null)
        {
            throw new KeyNotFoundException($"IP address '{ip}' was not found in the database.");
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the address report grouped by country.
    /// When country codes are provided, only matching countries are included.
    /// When no country codes are provided, all countries are included.
    /// </summary>
    /// <param name="twoLetterCodes">Optional list of two-letter country codes used to filter the report.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection containing the report rows.</returns>
    [HttpGet("report")]
    [ProducesResponseType(typeof(IReadOnlyCollection<GetAddressReportItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReport(
        [FromQuery] string[]? twoLetterCodes,
        CancellationToken cancellationToken)
    {
        var result = await _getAddressReportUseCase.ExecuteAsync(
            twoLetterCodes,
            cancellationToken);

        return Ok(result);
    }

}