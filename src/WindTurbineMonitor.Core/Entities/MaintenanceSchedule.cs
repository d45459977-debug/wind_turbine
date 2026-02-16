using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Core.Entities;

/// <summary>
/// Jadwal maintenance untuk komponen turbin
/// </summary>
public class MaintenanceSchedule
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
    /// Tipe komponen yang akan di-maintenance
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Tanggal yang dijadwalkan untuk maintenance
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Apakah maintenance sudah selesai
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Tanggal ketika maintenance selesai
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// ID user/teknisi yang melakukan maintenance
    /// </summary>
    public string? CompletedBy { get; set; }

    /// <summary>
    /// Catatan maintenance
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Prioritas maintenance
    /// </summary>
    public int Priority { get; set; } = 1; // 1 = Low, 5 = High

    /// <summary>
    /// Judul/jenis maintenance
    /// </summary>
    public string? Title { get; set; }
}
