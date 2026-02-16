namespace WindTurbineMonitor.Core.Enums;

/// <summary>
/// Status operasional turbin angin
/// </summary>
public enum TurbineStatus
{
    /// <summary>
    /// Turbin sedang offline/tidak aktif
    /// </summary>
    Offline = 0,

    /// <summary>
    /// Turbin sedang dalam proses starting
    /// </summary>
    Starting = 1,

    /// <summary>
    /// Turbin sedang beroperasi normal
    /// </summary>
    Running = 2,

    /// <summary>
    /// Turbin sedang dalam proses stopping
    /// </summary>
    Stopping = 3,

    /// <summary>
    /// Terjadi error pada turbin
    /// </summary>
    Error = 4,

    /// <summary>
    /// Turbin dalam mode maintenance
    /// </summary>
    Maintenance = 5
}
