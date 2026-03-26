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
    private readonly RefreshIpInformationUseCase _refreshIpInformationUseCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpInformationController"/> class.
    /// </summary>
    /// <param name="getIpInformationUseCase">The use case responsible for retrieving IP information.</param>
    /// <param name="refreshIpInformationUseCase">Temporary for tests</param>
    public IpInformationController(GetIpInformationUseCase getIpInformationUseCase, RefreshIpInformationUseCase refreshIpInformationUseCase)
    {
        _getIpInformationUseCase = getIpInformationUseCase;
        _refreshIpInformationUseCase = refreshIpInformationUseCase;
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
    /// Refreshes stored IP information in batches.
    /// This endpoint is intended for temporary manual validation before automation is enabled.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A summary of the refresh execution.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshIpInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Refresh(
        CancellationToken cancellationToken)
    {
        var result = await _refreshIpInformationUseCase.ExecuteAsync(cancellationToken);

        return Ok(result);
    }

}