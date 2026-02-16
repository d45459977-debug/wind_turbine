using WindTurbineMonitor.Core.Entities;

namespace WindTurbineMonitor.Core.Interfaces;

/// <summary>
/// Interface untuk repository turbin
/// </summary>
public interface ITurbineRepository
{
    /// <summary>
    /// Mendapatkan semua turbin
    /// </summary>
    Task<List<Turbine>> GetAllAsync();

    /// <summary>
    /// Mendapatkan turbin yang aktif saja
    /// </summary>
    Task<List<Turbine>> GetActiveAsync();

    /// <summary>
    /// Mendapatkan turbin berdasarkan ID
    /// </summary>
    Task<Turbine?> GetByIdAsync(Guid id);

    /// <summary>
    /// Menambahkan turbin baru
    /// </summary>
    Task<Turbine> AddAsync(Turbine turbine);

    /// <summary>
    /// Update turbin
    /// </summary>
    Task<Turbine> UpdateAsync(Turbine turbine);

    /// <summary>
    /// Menghapus turbin
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Cek apakah turbin exist
    /// </summary>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Mendapatkan readings terbaru untuk turbin
    /// </summary>
    Task<List<TurbineReading>> GetRecentReadingsAsync(Guid turbineId, int count = 10);

    /// <summary>
    /// Mendapatkan events terbaru untuk turbin
    /// </summary>
    Task<List<TurbineEvent>> GetRecentEventsAsync(Guid turbineId, int count = 50);

    /// <summary>
    /// Mendapatkan maintenance schedules yang akan datang
    /// </summary>
    Task<List<MaintenanceSchedule>> GetUpcomingMaintenanceAsync(Guid turbineId);

    /// <summary>
    /// Menambahkan event log
    /// </summary>
    Task<TurbineEvent> AddEventAsync(TurbineEvent @event);

    /// <summary>
    /// Menambahkan audit log
    /// </summary>
    Task<AuditLog> AddAuditLogAsync(AuditLog auditLog);

    /// <summary>
    /// Mendapatkan turbin dengan reading terbaru
    /// </summary>
    Task<Turbine?> GetWithLatestReadingAsync(Guid turbineId);

    /// <summary>
    /// Mendapatkan readings dalam range tanggal
    /// </summary>
    Task<List<TurbineReading>> GetReadingsInDateRangeAsync(Guid turbineId, DateTime from, DateTime to);

    /// <summary>
    /// Mendapatkan status komponen berdasarkan tipe
    /// </summary>
    Task<ComponentStatus?> GetComponentStatusAsync(Guid turbineId, int componentType);
}
