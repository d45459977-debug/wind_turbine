namespace WindTurbineMonitor.Core.Enums;

/// <summary>
/// Tingkat severity untuk event log
/// </summary>
public enum EventSeverity
{
    /// <summary>
    /// Informasi biasa
    /// </summary>
    Info = 0,

    /// <summary>
    /// Peringatan
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error
    /// </summary>
    Error = 2,

    /// <summary>
    /// Kritis - membutuhkan perhatian segera
    /// </summary>
    Critical = 3,

    /// <summary>
    /// Event berhasil
    /// </summary>
    Success = 4
}
