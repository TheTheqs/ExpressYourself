namespace ExpressYourself.Infrastructure.BackgroundJobs;

/// <summary>
/// Represents configuration options for the refresh IP information background job.
/// </summary>
public sealed class RefreshIpInformationJobOptions
{
    /// <summary>
    /// Gets or sets the interval between refresh executions.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
}