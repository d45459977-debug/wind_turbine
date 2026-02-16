namespace WindTurbineMonitor.Core.Entities;

/// <summary>
/// Log audit untuk melacak aktivitas pengguna dan perubahan sistem
/// </summary>
public class AuditLog
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID user yang melakukan aksi
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Username user yang melakukan aksi
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Aksi yang dilakukan (Create, Update, Delete, etc.)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Nama entity yang diubah
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// ID entity yang diubah
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Nilai sebelum perubahan (JSON format)
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Nilai setelah perubahan (JSON format)
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Timestamp ketika aksi dilakukan
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// IP address user
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data tambahan dalam format JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
