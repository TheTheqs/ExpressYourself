using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExpressYourself.entities;
//Core class of the Entity Framework. All database access will be handled through it via the DB class, the official central hub
// for database access.
public partial class IpLocatorContext : DbContext
{   
    //Constructors
    //The lines Countries = null! and Ipaddresses = null! were added to both constructors to suppress warnings.
    public IpLocatorContext()
    {
        Countries = null!;
        Ipaddresses = null!;
    }

    public IpLocatorContext(DbContextOptions<IpLocatorContext> options)
        : base(options)
    {
        Countries = null!;
        Ipaddresses = null!;
    }
    //Attributes: Represent tables in the database. These attributes provide all CRUD methods for their respective tables.
    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Ipaddress> Ipaddresses { get; set; }

    //Configuration
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=ip_locator;Username=postgres;Password=Marina22");

    //Build Methods: Creates an object based on the data providade from the Bank.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Countries_pkey");

            entity.HasIndex(e => e.Name, "Countries_Name_key").IsUnique();

            entity.HasIndex(e => e.ThreeLetterCode, "Countries_ThreeLetterCode_key").IsUnique();

            entity.HasIndex(e => e.TwoLetterCode, "Countries_TwoLetterCode_key").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ThreeLetterCode)
                .HasMaxLength(3)
                .IsFixedLength();
            entity.Property(e => e.TwoLetterCode)
                .HasMaxLength(2)
                .IsFixedLength();
        });

        modelBuilder.Entity<Ipaddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("IPAddresses_pkey");

            entity.ToTable("IPAddresses");

            entity.HasIndex(e => e.Ip, "IPAddresses_IP_key").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Ip)
                .HasMaxLength(15)
                .HasColumnName("IP");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Country).WithMany(p => p.Ipaddresses)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("IPAddresses_CountryId_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
