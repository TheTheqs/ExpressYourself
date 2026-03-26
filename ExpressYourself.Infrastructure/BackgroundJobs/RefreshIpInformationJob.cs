using ExpressYourself.Application.UseCases.RefreshIpInformation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpressYourself.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job responsible for periodically refreshing stored IP information.
/// </summary>
public sealed class RefreshIpInformationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RefreshIpInformationJob> _logger;
    private readonly TimeSpan _interval;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshIpInformationJob"/> class.
    /// </summary>
    /// <param name="scopeFactory">The service scope factory.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The job configuration options.</param>
    public RefreshIpInformationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<RefreshIpInformationJob> logger,
        IOptions<RefreshIpInformationJobOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _interval = options.Value.Interval;

        if (_interval <= TimeSpan.Zero)
        {
            throw new ArgumentException("Refresh interval must be greater than zero.", nameof(options));
        }
    }

    /// <summary>
    /// Executes a single refresh iteration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal async Task ExecuteIterationAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var useCase = scope.ServiceProvider
            .GetRequiredService<RefreshIpInformationUseCase>();

        var result = await useCase.ExecuteAsync(cancellationToken);

        _logger.LogInformation(
            "IP refresh completed. Processed: {Processed}, Updated: {Updated}, Unchanged: {Unchanged}, NotResolved: {NotResolved}",
            result.ProcessedCount,
            result.UpdatedCount,
            result.UnchangedCount,
            result.NotResolvedCount);
    }

    /// <summary>
    /// Executes the background job loop.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token.</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Refresh IP Information Job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteIterationAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during IP refresh job execution.");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Refresh IP Information Job stopped.");
    }
}