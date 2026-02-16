using Microsoft.AspNetCore.SignalR;
using WindTurbineMonitor.Core.Interfaces;
using WindTurbineMonitor.Server.Hubs;
using WindTurbineMonitor.Shared.DTOs;
using WindTurbineMonitor.Core.Entities;
using WindTurbineMonitor.Core.Enums;

namespace WindTurbineMonitor.Server.Services;

/// <summary>
/// Background service untuk broadcast data turbin secara real-time ke clients
/// </summary>
public class TurbineDataBroadcastService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<TurbineHub> _hubContext;
    private readonly ILogger<TurbineDataBroadcastService> _logger;
    private readonly Dictionary<Guid, TurbineReadingDto> _latestReadings;
    private readonly Timer _cleanupTimer;

    public TurbineDataBroadcastService(
        IServiceProvider serviceProvider,
        IHubContext<TurbineHub> hubContext,
        ILogger<TurbineDataBroadcastService> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
        _latestReadings = new Dictionary<Guid, TurbineReadingDto>();

        // Cleanup old data every hour
        _cleanupTimer = new Timer(CleanupOldData, null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Turbine Data Broadcast Service starting");

        // Wait a bit for services to initialize
        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var sensorService = scope.ServiceProvider.GetRequiredService<ISensorDataService>();
                var turbineRepository = scope.ServiceProvider.GetRequiredService<Core.Interfaces.ITurbineRepository>();

                // Get all active turbines
                var turbines = await turbineRepository.GetActiveAsync();

                foreach (var turbine in turbines)
                {
                    try
                    {
                        // Get latest reading
                        var reading = await sensorService.GetLatestReadingAsync(turbine.Id);

                        if (reading != null)
                        {
                            var readingDto = MapToDto(reading, turbine.Name);

                            // Update cache
                            _latestReadings[turbine.Id] = readingDto;

                            // Broadcast to turbine-specific group
                            await _hubContext.Clients.Group($"Turbine_{turbine.Id}")
                                .SendAsync("ReceiveTurbineData", readingDto, stoppingToken);

                            // Also broadcast to all clients for dashboard
                            await _hubContext.Clients.All
                                .SendAsync("ReceiveTurbineUpdate", turbine.Id, readingDto, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error fetching data for turbine {TurbineId}", turbine.Id);
                    }
                }

                // Broadcast summary to all clients
                var summary = _latestReadings.Values.ToList();
                await _hubContext.Clients.All.SendAsync("ReceiveAllTurbinesData", summary, stoppingToken);

                // Wait before next update
                await Task.Delay(2000, stoppingToken); // Update every 2 seconds
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in broadcast service");
                await Task.Delay(5000, stoppingToken); // Wait before retry
            }
        }

        _logger.LogInformation("Turbine Data Broadcast Service stopping");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Turbine Data Broadcast Service stopping...");
        _cleanupTimer.Dispose();
        await base.StopAsync(cancellationToken);
    }

    private TurbineReadingDto MapToDto(TurbineReading reading, string turbineName)
    {
        return new TurbineReadingDto
        {
            Id = reading.Id,
            TurbineId = reading.TurbineId,
            TurbineName = turbineName,
            Timestamp = reading.Timestamp,
            RotorSpeedRpm = reading.RotorSpeedRpm,
            WindSpeedMs = reading.WindSpeedMs,
            PowerKw = reading.PowerKw,
            EfficiencyPercent = reading.EfficiencyPercent,
            WindDirectionDegrees = reading.WindDirectionDegrees,
            Status = reading.Status.ToString(),
            GearboxTemperature = reading.GearboxTemperature,
            GeneratorTemperature = reading.GeneratorTemperature,
            HydraulicPressure = reading.HydraulicPressure,
            VibrationLevel = reading.VibrationLevel
        };
    }

    private void CleanupOldData(object? state)
    {
        try
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            var keysToRemove = _latestReadings
                .Where(kvp => kvp.Value.Timestamp < cutoff)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                _latestReadings.Remove(key);
            }

            _logger.LogDebug("Cleaned up {Count} old turbine readings", keysToRemove.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up old data");
        }
    }

    public TurbineReadingDto? GetLatestReading(Guid turbineId)
    {
        return _latestReadings.GetValueOrDefault(turbineId);
    }

    public List<TurbineReadingDto> GetAllLatestReadings()
    {
        return _latestReadings.Values.ToList();
    }
}
