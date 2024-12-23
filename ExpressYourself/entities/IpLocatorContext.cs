﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExpressYourself.entities;

public partial class IpLocatorContext : DbContext
{
    public IpLocatorContext()
    {
    }

    public IpLocatorContext(DbContextOptions<IpLocatorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Ipaddress> Ipaddresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=ip_locator;Username=postgres;Password=Marina22");

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