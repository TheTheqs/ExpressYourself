using ExpressYourself.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpressYourself.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the database mapping for the <see cref="Country"/> entity.
/// </summary>
public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    /// <summary>
    /// Configures the entity schema definition.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(country => country.Id);

        builder.Property(country => country.Id)
            .ValueGeneratedOnAdd();

        builder.Property(country => country.Name)
            .HasColumnName("Name")
            .HasColumnType("varchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(country => country.TwoLetterCode)
            .HasColumnName("TwoLetterCode")
            .HasColumnType("char(2)")
            .IsRequired();

        builder.Property(country => country.ThreeLetterCode)
            .HasColumnName("ThreeLetterCode")
            .HasColumnType("char(3)")
            .IsRequired();

        builder.Property(country => country.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2(7)")
            .IsRequired();

        builder.HasMany(country => country.IpAddresses)
            .WithOne()
            .HasForeignKey(ipAddress => ipAddress.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}