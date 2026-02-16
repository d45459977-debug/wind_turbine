using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Core.Entities;

/// <summary>
/// Entity utama untuk turbin angin
/// </summary>
public class Turbine
{
    /// <summary>
    /// Unique identifier untuk turbin
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nama turbin
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Lokasi turbin
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Model turbin
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Kapasitas daya dalam MegaWatt
    /// </summary>
    public decimal Capacity { get; set; }

    /// <summary>
    /// Status operasional saat ini
    /// </summary>
    public TurbineStatus Status { get; set; }

    /// <summary>
    /// Tanggal pembuatan/instalasi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Apakah turbin aktif
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Kumpulan data reading dari turbin
    /// </summary>
    public ICollection<TurbineReading> Readings { get; set; } = new List<TurbineReading>();

    /// <summary>
    /// Kumpulan event log
    /// </summary>
    public ICollection<TurbineEvent> Events { get; set; } = new List<TurbineEvent>();

    /// <summary>
    /// Kumpulan status komponen
    /// </summary>
    public ICollection<ComponentStatus> ComponentStatuses { get; set; } = new List<ComponentStatus>();

    /// <summary>
    /// Kumpulan jadwal maintenance
    /// </summary>
    public ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new List<MaintenanceSchedule>();
}
