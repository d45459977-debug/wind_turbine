using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Core.Entities;

/// <summary>
/// Status kesehatan komponen turbin
/// </summary>
public class ComponentStatus
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key ke Turbine
    /// </summary>
    public Guid TurbineId { get; set; }

    /// <summary>
    /// Navigation property ke Turbine
    /// </summary>
    public Turbine? Turbine { get; set; }

    /// <summary>
    /// Tipe komponen
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Nama komponen
    /// </summary>
    public string ComponentName { get; set; } = string.Empty;

    /// <summary>
    /// Status komponen (Healthy, Warning, Error, etc.)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Persentase kesehatan komponen (0-100)
    /// </summary>
    public double HealthPercentage { get; set; }

    /// <summary>
    /// Timestamp terakhir update
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Pesan atau deskripsi status
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data tambahan dalam format JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
