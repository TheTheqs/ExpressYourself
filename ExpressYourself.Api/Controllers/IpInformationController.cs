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

    /// <summary>
    /// Initializes a new instance of the <see cref="IpInformationController"/> class.
    /// </summary>
    /// <param name="getIpInformationUseCase">The use case responsible for retrieving IP information.</param>
    public IpInformationController(GetIpInformationUseCase getIpInformationUseCase)
    {
        _getIpInformationUseCase = getIpInformationUseCase;
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

}