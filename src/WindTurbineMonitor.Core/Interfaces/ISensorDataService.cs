using WindTurbineMonitor.Core.Entities;

namespace WindTurbineMonitor.Core.Interfaces;

/// <summary>
/// Interface untuk service yang berkomunikasi dengan sensor turbin
/// </summary>
public interface ISensorDataService
{
    /// <summary>
    /// Mendapatkan reading terbaru dari turbin
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    /// <returns>Reading terbaru</returns>
    Task<TurbineReading?> GetLatestReadingAsync(Guid turbineId);

    /// <summary>
    /// Mendapatkan reading terbaru dari semua turbin
    /// </summary>
    /// <returns>List reading terbaru</returns>
    Task<List<TurbineReading>> GetAllLatestReadingsAsync();

    /// <summary>
    /// Mendapatkan data historis reading
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    /// <param name="from">Tanggal awal</param>
    /// <param name="to">Tanggal akhir</param>
    /// <returns>List reading dalam range tanggal</returns>
    Task<List<TurbineReading>> GetHistoricalReadingsAsync(Guid turbineId, DateTime from, DateTime to);

    /// <summary>
    /// Mengirim command kontrol ke turbin
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    /// <param name="command">Command yang akan dikirim</param>
    /// <param name="userId">ID user yang mengirim command</param>
    /// <returns>True jika berhasil</returns>
    Task<bool> SendControlCommandAsync(Guid turbineId, ControlCommand command, string? userId);

    /// <summary>
    /// Subscribe untuk update real-time dari sensor
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    /// <param name="onUpdate">Callback ketika ada update</param>
    /// <returns>CancellationToken untuk unsubscribe</returns>
    Task SubscribeToUpdatesAsync(Guid turbineId, Action<TurbineReading> onUpdate);

    /// <summary>
    /// Mendapatkan status komponen terbaru
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    /// <returns>List status komponen</returns>
    Task<List<ComponentStatus>> GetComponentStatusesAsync(Guid turbineId);
}

/// <summary>
/// Tipe command kontrol yang dapat dikirim ke turbin
/// </summary>
public enum ControlCommand
{
    /// <summary>
    /// Start turbine
    /// </summary>
    Start,

    /// <summary>
    /// Stop turbine
    /// </summary>
    Stop,

    /// <summary>
    /// Emergency stop
    /// </summary>
    EmergencyStop,

    /// <summary>
    /// Reset system
    /// </summary>
    Reset,

    /// <summary>
    /// Manual mode
    /// </summary>
    ManualMode,

    /// <summary>
    /// Automatic mode
    /// </summary>
    AutomaticMode
}
