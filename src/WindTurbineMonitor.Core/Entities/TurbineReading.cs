using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Core.Entities;

/// <summary>
/// Data reading real-time dari turbin
/// </summary>
public class TurbineReading
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
    /// Timestamp ketika data diambil
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Kecepatan rotor dalam RPM (Revolutions Per Minute)
    /// </summary>
    public double RotorSpeedRpm { get; set; }

    /// <summary>
    /// Kecepatan angin dalam meter per detik
    /// </summary>
    public double WindSpeedMs { get; set; }

    /// <summary>
    /// Daya listrik yang dihasilkan dalam kiloWatt
    /// </summary>
    public double PowerKw { get; set; }

    /// <summary>
    /// Efisiensi sistem dalam persentase
    /// </summary>
    public double EfficiencyPercent { get; set; }

    /// <summary>
    /// Arah mata angin dalam derajat (0-360)
    /// </summary>
    public double WindDirectionDegrees { get; set; }

    /// <summary>
    /// Status turbin pada saat reading
    /// </summary>
    public TurbineStatus Status { get; set; }

    /// <summary>
    /// Data tambahan dalam format JSON
    /// </summary>
    public string? JsonData { get; set; }

    /// <summary>
    /// Suhu gearbox dalam Celsius
    /// </summary>
    public double? GearboxTemperature { get; set; }

    /// <summary>
    /// Suhu generator dalam Celsius
    /// </summary>
    public double? GeneratorTemperature { get; set; }

    /// <summary>
    /// Tekanan hidrolik dalam Bar
    /// </summary>
    public double? HydraulicPressure { get; set; }

    /// <summary>
    /// Vibrasi level dalam mm/s
    /// </summary>
    public double? VibrationLevel { get; set; }
}
