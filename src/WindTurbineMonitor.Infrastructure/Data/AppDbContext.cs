using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Core.Entities;

namespace WindTurbineMonitor.Infrastructure.Data;

/// <summary>
/// Database context untuk aplikasi Wind Turbine Monitor
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet untuk Turbin
    /// </summary>
    public DbSet<Turbine> Turbines => Set<Turbine>();

    /// <summary>
    /// DbSet untuk Turbine Reading
    /// </summary>
    public DbSet<TurbineReading> TurbineReadings => Set<TurbineReading>();

    /// <summary>
    /// DbSet untuk Turbine Event
    /// </summary>
    public DbSet<TurbineEvent> TurbineEvents => Set<TurbineEvent>();

    /// <summary>
    /// DbSet untuk Component Status
    /// </summary>
    public DbSet<ComponentStatus> ComponentStatuses => Set<ComponentStatus>();

    /// <summary>
    /// DbSet untuk Maintenance Schedule
    /// </summary>
    public DbSet<MaintenanceSchedule> MaintenanceSchedules => Set<MaintenanceSchedule>();

    /// <summary>
    /// DbSet untuk Audit Log
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Turbine configuration
        modelBuilder.Entity<Turbine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Capacity).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasMaxLength(50).HasConversion<string>();
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.IsActive);
        });

        // TurbineReading configuration
        modelBuilder.Entity<TurbineReading>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RotorSpeedRpm).HasPrecision(10, 2);
            entity.Property(e => e.WindSpeedMs).HasPrecision(10, 2);
            entity.Property(e => e.PowerKw).HasPrecision(15, 2);
            entity.Property(e => e.EfficiencyPercent).HasPrecision(5, 2);
            entity.Property(e => e.WindDirectionDegrees).HasPrecision(5, 2);
            entity.Property(e => e.JsonData).HasColumnType("json");
            entity.HasOne(e => e.Turbine)
                  .WithMany(t => t.Readings)
                  .HasForeignKey(e => e.TurbineId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TurbineId);
            entity.HasIndex(e => e.Timestamp);
        });

        // TurbineEvent configuration
        modelBuilder.Entity<TurbineEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.Severity).HasMaxLength(50).HasConversion<string>();
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.AdditionalData).HasColumnType("json");
            entity.HasOne(e => e.Turbine)
                  .WithMany(t => t.Events)
                  .HasForeignKey(e => e.TurbineId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TurbineId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Severity);
        });

        // ComponentStatus configuration
        modelBuilder.Entity<ComponentStatus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComponentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ComponentType).HasMaxLength(50).HasConversion<string>();
            entity.Property(e => e.HealthPercentage).HasPrecision(5, 2);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.AdditionalData).HasColumnType("json");
            entity.HasOne(e => e.Turbine)
                  .WithMany(t => t.ComponentStatuses)
                  .HasForeignKey(e => e.TurbineId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TurbineId);
            entity.HasIndex(e => e.ComponentType);
        });

        // MaintenanceSchedule configuration
        modelBuilder.Entity<MaintenanceSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ComponentType).HasMaxLength(50).HasConversion<string>();
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.CompletedBy).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.HasOne(e => e.Turbine)
                  .WithMany(t => t.MaintenanceSchedules)
                  .HasForeignKey(e => e.TurbineId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.TurbineId);
            entity.HasIndex(e => e.ScheduledDate);
            entity.HasIndex(e => e.IsCompleted);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).HasMaxLength(50);
            entity.Property(e => e.OldValue).HasColumnType("text");
            entity.Property(e => e.NewValue).HasColumnType("text");
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasColumnType("text");
            entity.Property(e => e.AdditionalData).HasColumnType("json");
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.EntityName);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // This will be configured via dependency injection in Program.cs
            base.OnConfiguring(optionsBuilder);
        }
    }
}
