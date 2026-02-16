using Microsoft.Extensions.Logging;
using WindTurbineMonitor.Core.Entities;
using WindTurbineMonitor.Core.Interfaces;
using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Infrastructure.Services;

/// <summary>
/// Base class untuk sensor data service
/// </summary>
public abstract class SensorDataServiceBase : ISensorDataService
{
    protected readonly ILogger<SensorDataServiceBase> _logger;
    protected readonly Dictionary<Guid, CancellationTokenSource> _subscriptions;

    protected SensorDataServiceBase(ILogger<SensorDataServiceBase> logger)
    {
        _logger = logger;
        _subscriptions = new Dictionary<Guid, CancellationTokenSource>();
    }

    public abstract Task<TurbineReading?> GetLatestReadingAsync(Guid turbineId);

    public abstract Task<List<TurbineReading>> GetAllLatestReadingsAsync();

    public abstract Task<List<TurbineReading>> GetHistoricalReadingsAsync(Guid turbineId, DateTime from, DateTime to);

    public abstract Task<bool> SendControlCommandAsync(Guid turbineId, ControlCommand command, string? userId);

    public abstract Task<List<ComponentStatus>> GetComponentStatusesAsync(Guid turbineId);

    public virtual async Task SubscribeToUpdatesAsync(Guid turbineId, Action<TurbineReading> onUpdate)
    {
        if (_subscriptions.ContainsKey(turbineId))
        {
            _logger.LogWarning("Already subscribed to turbine {TurbineId}", turbineId);
            return;
        }

        var cts = new CancellationTokenSource();
        _subscriptions[turbineId] = cts;

        await Task.Run(async () =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    var reading = await GetLatestReadingAsync(turbineId);
                    if (reading != null)
                    {
                        onUpdate(reading);
                    }

                    await Task.Delay(2000, cts.Token); // Update every 2 seconds
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error subscribing to turbine {TurbineId}", turbineId);
                    await Task.Delay(5000, cts.Token); // Wait before retry
                }
            }
        }, cts.Token);
    }

    public virtual void Unsubscribe(Guid turbineId)
    {
        if (_subscriptions.TryGetValue(turbineId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _subscriptions.Remove(turbineId);
            _logger.LogInformation("Unsubscribed from turbine {TurbineId}", turbineId);
        }
    }

    protected TurbineReading CreateDefaultReading(Guid turbineId, string turbineName)
    {
        return new TurbineReading
        {
            Id = Guid.NewGuid(),
            TurbineId = turbineId,
            Timestamp = DateTime.UtcNow,
            RotorSpeedRpm = 0,
            WindSpeedMs = 0,
            PowerKw = 0,
            EfficiencyPercent = 0,
            WindDirectionDegrees = 0,
            Status = TurbineStatus.Offline,
            GearboxTemperature = 20,
            GeneratorTemperature = 20,
            HydraulicPressure = 100,
            VibrationLevel = 0
        };
    }

    protected List<ComponentStatus> CreateDefaultComponentStatuses(Guid turbineId)
    {
        return new List<ComponentStatus>
        {
            new ComponentStatus
            {
                Id = Guid.NewGuid(),
                TurbineId = turbineId,
                ComponentType = ComponentType.Tower,
                ComponentName = "Main Tower",
                Status = "Healthy",
                HealthPercentage = 100,
                LastUpdated = DateTime.UtcNow
            },
            new ComponentStatus
            {
                Id = Guid.NewGuid(),
                TurbineId = turbineId,
                ComponentType = ComponentType.Nacelle,
                ComponentName = "Nacelle",
                Status = "Healthy",
                HealthPercentage = 100,
                LastUpdated = DateTime.UtcNow
            },
            new ComponentStatus
            {
                Id = Guid.NewGuid(),
                TurbineId = turbineId,
                ComponentType = ComponentType.RotorBlade,
                ComponentName = "Rotor Blades",
                Status = "Healthy",
                HealthPercentage = 100,
                LastUpdated = DateTime.UtcNow
            },
            new ComponentStatus
            {
                Id = Guid.NewGuid(),
                TurbineId = turbineId,
                ComponentType = ComponentType.Gearbox,
                ComponentName = "Gearbox",
                Status = "Healthy",
                HealthPercentage = 100,
                LastUpdated = DateTime.UtcNow
            },
            new ComponentStatus
            {
                Id = Guid.NewGuid(),
                TurbineId = turbineId,
                ComponentType = ComponentType.Generator,
                ComponentName = "Generator",
                Status = "Healthy",
                HealthPercentage = 100,
                LastUpdated = DateTime.UtcNow
            }
        };
    }
}
