namespace WindTurbineMonitor.Core.Enums;

/// <summary>
/// Tipe komponen pada turbin angin
/// </summary>
public enum ComponentType
{
    /// <summary>
    /// Tower turbin
    /// </summary>
    Tower = 0,

    /// <summary>
    /// Nacelle (rumah gearbox)
    /// </summary>
    Nacelle = 1,

    /// <summary>
    /// Rotor blade
    /// </summary>
    RotorBlade = 2,

    /// <summary>
    /// Gearbox
    /// </summary>
    Gearbox = 3,

    /// <summary>
    /// Generator
    /// </summary>
    Generator = 4,

    /// <summary>
    /// Brake system
    /// </summary>
    Brake = 5,

    /// <summary>
    /// Pitch system
    /// </summary>
    Pitch = 6,

    /// <summary>
    /// Yaw system
    /// </summary>
    Yaw = 7,

    /// <summary>
    /// Transformer
    /// </summary>
    Transformer = 8,

    /// <summary>
    /// Control system
    /// </summary>
    Control = 9
}
