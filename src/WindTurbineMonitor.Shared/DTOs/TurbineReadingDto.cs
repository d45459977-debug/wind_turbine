namespace WindTurbineMonitor.Shared.DTOs;

/// <summary>
/// DTO untuk data reading turbin
/// </summary>
public class TurbineReadingDto
{
    public Guid Id { get; set; }
    public Guid TurbineId { get; set; }
    public string TurbineName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double RotorSpeedRpm { get; set; }
    public double WindSpeedMs { get; set; }
    public double PowerKw { get; set; }
    public double PowerMw => PowerKw / 1000;
    public double EfficiencyPercent { get; set; }
    public double WindDirectionDegrees { get; set; }
    public string Status { get; set; } = string.Empty;
    public string WindDirectionCardinal => GetCardinalDirection(WindDirectionDegrees);
    public double? GearboxTemperature { get; set; }
    public double? GeneratorTemperature { get; set; }
    public double? HydraulicPressure { get; set; }
    public double? VibrationLevel { get; set; }

    private static string GetCardinalDirection(double degrees)
    {
        var directions = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        var index = (int)Math.Round(degrees / 45) % 8;
        return directions[index];
    }
}

/// <summary>
/// DTO untuk data ringkasan turbin
/// </summary>
public class TurbineSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public TurbineReadingDto? LatestReading { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO untuk status komponen
/// </summary>
public class ComponentStatusDto
{
    public Guid Id { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double HealthPercentage { get; set; }
    public DateTime LastUpdated { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// DTO untuk event log
/// </summary>
public class TurbineEventDto
{
    public Guid Id { get; set; }
    public Guid TurbineId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; }
}
