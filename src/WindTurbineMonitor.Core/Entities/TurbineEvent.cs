using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Core.Entities;

/// <summary>
/// Event log untuk aktivitas dan kejadian pada turbin
/// </summary>
public class TurbineEvent
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
    /// Tipe event
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Pesan event
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Severity level
    /// </summary>
    public EventSeverity Severity { get; set; }

    /// <summary>
    /// Timestamp ketika event terjadi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID user yang melakukan aksi (jika applicable)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Data tambahan dalam format JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
