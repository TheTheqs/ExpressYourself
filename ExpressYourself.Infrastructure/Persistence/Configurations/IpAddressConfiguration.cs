using ExpressYourself.Domain.Entities;
using ExpressYourself.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExpressYourself.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="IpAddress"/> entity.
/// </summary>
public sealed class IpAddressConfiguration : IEntityTypeConfiguration<IpAddress>
{
    /// <summary>
    /// Configures the entity schema definition.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<IpAddress> builder)
    {
        builder.ToTable("IPAddresses");

        builder.HasKey(ipAddress => ipAddress.Id);

        builder.Property(ipAddress => ipAddress.Id)
            .ValueGeneratedOnAdd();

        var ipAddressConverter = new ValueConverter<IpAddressValue, string>(
            ipAddressValue => ipAddressValue.Value,
            databaseValue => IpAddressValue.Create(databaseValue));

        builder.Property(ipAddress => ipAddress.Address)
            .HasColumnName("IP")
            .HasColumnType("varchar(15)")
            .HasMaxLength(15)
            .HasConversion(ipAddressConverter)
            .IsRequired();

        builder.HasIndex(ipAddress => ipAddress.Address)
            .IsUnique()
            .HasDatabaseName("IX_IPAddresses");

        builder.Property(ipAddress => ipAddress.CountryId)
            .HasColumnName("CountryId")
            .IsRequired();

        builder.Property(ipAddress => ipAddress.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.Property(ipAddress => ipAddress.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2(7)")
            .IsRequired();
    }
}