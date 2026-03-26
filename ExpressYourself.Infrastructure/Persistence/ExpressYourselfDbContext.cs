using ExpressYourself.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpressYourself.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context.
/// Responsible for managing entity persistence and mapping configuration.
/// </summary>
public sealed class ExpressYourselfDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressYourselfDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ExpressYourselfDbContext(DbContextOptions<ExpressYourselfDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the countries table representation.
    /// </summary>
    public DbSet<Country> Countries { get; set; } = null!;

    /// <summary>
    /// Gets the IP addresses table representation.
    /// </summary>
    public DbSet<IpAddress> IpAddresses { get; set; } = null!;

    /// <summary>
    /// Configures the entity mappings for the database context.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entity mappings.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpressYourselfDbContext).Assembly);
    }
}