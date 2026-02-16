using Microsoft.Extensions.Logging;
using WindTurbineMonitor.Core.Entities;
using WindTurbineMonitor.Core.Enums;
using WindTurbineMonitor.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace WindTurbineMonitor.Infrastructure.Services;

/// <summary>
/// Simulator service untuk menghasilkan data turbin secara real-time
/// Untuk testing dan development
/// </summary>
public class SimulatorSensorService : SensorDataServiceBase
{
    private readonly Dictionary<Guid, TurbineSimulationState> _simulationStates;
    private readonly Random _random = new();

    public SimulatorSensorService(ILogger<SimulatorSensorService> logger)
        : base(logger)
    {
        _simulationStates = new Dictionary<Guid, TurbineSimulationState>();
    }

    public override async Task<TurbineReading?> GetLatestReadingAsync(Guid turbineId)
    {
        await Task.Delay(10); // Simulate network delay

        if (!_simulationStates.ContainsKey(turbineId))
        {
            InitializeSimulationState(turbineId, $"Turbine {turbineId.ToString()[..8]}");
        }

        var state = _simulationStates[turbineId];
        UpdateSimulationState(state);

        return new TurbineReading
        {
            Id = Guid.NewGuid(),
            TurbineId = turbineId,
            Timestamp = DateTime.UtcNow,
            RotorSpeedRpm = state.RotorSpeedRpm,
            WindSpeedMs = state.WindSpeedMs,
            PowerKw = state.PowerKw,
            EfficiencyPercent = state.EfficiencyPercent,
            WindDirectionDegrees = state.WindDirectionDegrees,
            Status = state.Status,
            GearboxTemperature = state.GearboxTemperature,
            GeneratorTemperature = state.GeneratorTemperature,
            HydraulicPressure = state.HydraulicPressure,
            VibrationLevel = state.VibrationLevel
        };
    }

    public override async Task<List<TurbineReading>> GetAllLatestReadingsAsync()
    {
        var readings = new List<TurbineReading>();
        foreach (var turbineId in _simulationStates.Keys)
        {
            var reading = await GetLatestReadingAsync(turbineId);
            if (reading != null)
            {
                readings.Add(reading);
            }
        }

        return readings;
    }

    public override async Task<List<TurbineReading>> GetHistoricalReadingsAsync(
        Guid turbineId,
        DateTime from,
        DateTime to)
    {
        await Task.Delay(50); // Simulate database query delay

        var readings = new List<TurbineReading>();
        var current = from;

        if (!_simulationStates.ContainsKey(turbineId))
        {
            InitializeSimulationState(turbineId, $"Turbine {turbineId.ToString()[..8]}");
        }

        while (current <= to)
        {
            readings.Add(new TurbineReading
            {
                Id = Guid.NewGuid(),
                TurbineId = turbineId,
                Timestamp = current,
                RotorSpeedRpm = _random.NextDouble() * 15 + 5,
                WindSpeedMs = _random.NextDouble() * 10 + 3,
                PowerKw = _random.NextDouble() * 1500 + 500,
                EfficiencyPercent = _random.NextDouble() * 20 + 30,
                WindDirectionDegrees = _random.NextDouble() * 360,
                Status = TurbineStatus.Running
            });

            current = current.AddMinutes(5);
        }

        return readings;
    }

    public override async Task<bool> SendControlCommandAsync(
        Guid turbineId,
        ControlCommand command,
        string? userId)
    {
        await Task.Delay(100); // Simulate command execution delay

        if (!_simulationStates.ContainsKey(turbineId))
        {
            _logger.LogWarning("Turbine {TurbineId} not found in simulation", turbineId);
            return false;
        }

        var state = _simulationStates[turbineId];

        switch (command)
        {
            case ControlCommand.Start:
                if (state.Status == TurbineStatus.Offline)
                {
                    state.Status = TurbineStatus.Starting;
                    _logger.LogInformation("Turbine {TurbineId} starting by user {UserId}", turbineId, userId);

                    // Simulate startup sequence
                    await Task.Delay(2000);
                    state.Status = TurbineStatus.Running;
                    state.RotorSpeedRpm = _random.NextDouble() * 5 + 8;
                }
                break;

            case ControlCommand.Stop:
                if (state.Status == TurbineStatus.Running)
                {
                    state.Status = TurbineStatus.Stopping;
                    _logger.LogInformation("Turbine {TurbineId} stopping by user {UserId}", turbineId, userId);

                    // Simulate shutdown sequence
                    await Task.Delay(2000);
                    state.Status = TurbineStatus.Offline;
                    state.RotorSpeedRpm = 0;
                    state.PowerKw = 0;
                }
                break;

            case ControlCommand.EmergencyStop:
                state.Status = TurbineStatus.Error;
                state.RotorSpeedRpm = 0;
                state.PowerKw = 0;
                _logger.LogWarning("Turbine {TurbineId} emergency stop by user {UserId}", turbineId, userId);
                break;
        }

        return true;
    }

    public override async Task<List<ComponentStatus>> GetComponentStatusesAsync(Guid turbineId)
    {
        await Task.Delay(10);

        var baseStatuses = CreateDefaultComponentStatuses(turbineId);

        // Randomly degrade some components for realism
        foreach (var status in baseStatuses)
        {
            var health = _random.NextDouble() * 100;
            status.HealthPercentage = Math.Round(health, 2);

            if (health < 50)
            {
                status.Status = "Warning";
                status.Description = "Component showing signs of wear";
            }
            else if (health < 80)
            {
                status.Status = "Good";
                status.Description = "Component operating normally";
            }
            else
            {
                status.Status = "Healthy";
                status.Description = "Component in excellent condition";
            }

            status.LastUpdated = DateTime.UtcNow;
        }

        return baseStatuses;
    }

    public void InitializeTurbine(Guid turbineId, string name)
    {
        if (!_simulationStates.ContainsKey(turbineId))
        {
            InitializeSimulationState(turbineId, name);
        }
    }

    private void InitializeSimulationState(Guid turbineId, string name)
    {
        _simulationStates[turbineId] = new TurbineSimulationState
        {
            TurbineId = turbineId,
            Name = name,
            Status = TurbineStatus.Offline,
            WindSpeedMs = _random.NextDouble() * 10 + 3,
            WindDirectionDegrees = _random.NextDouble() * 360,
            RotorSpeedRpm = 0,
            PowerKw = 0,
            EfficiencyPercent = 0,
            GearboxTemperature = 25,
            GeneratorTemperature = 30,
            HydraulicPressure = 120,
            VibrationLevel = 0.5
        };

        _logger.LogInformation("Initialized simulation for turbine {TurbineId} ({Name})", turbineId, name);
    }

    private void UpdateSimulationState(TurbineSimulationState state)
    {
        // Simulate natural variation in wind speed
        var windChange = (_random.NextDouble() - 0.5) * 2; // -1 to +1 m/s change
        state.WindSpeedMs = Math.Max(0, Math.Min(30, state.WindSpeedMs + windChange));

        // Update wind direction slowly
        state.WindDirectionDegrees = (state.WindDirectionDegrees + (_random.NextDouble() - 0.5) * 10) % 360;

        // Calculate rotor speed based on wind speed (simplified power curve)
        if (state.Status == TurbineStatus.Running && state.WindSpeedMs >= 3)
        {
            var targetRotorSpeed = Math.Min(15, (state.WindSpeedMs - 3) * 1.2);
            state.RotorSpeedRpm += (targetRotorSpeed - state.RotorSpeedRpm) * 0.1;
        }
        else if (state.Status != TurbineStatus.Running)
        {
            state.RotorSpeedRpm *= 0.9; // Slow down
            if (state.RotorSpeedRpm < 0.1) state.RotorSpeedRpm = 0;
        }

        // Calculate power output based on rotor speed
        if (state.RotorSpeedRpm > 0)
        {
            var efficiency = 0.35 + (_random.NextDouble() - 0.5) * 0.1; // ~35% efficiency
            var power = Math.Pow(state.WindSpeedMs, 3) * efficiency * 0.5; // Simplified power formula
            state.PowerKw = Math.Min(2000, Math.Max(0, power));
            state.EfficiencyPercent = efficiency * 100;
        }
        else
        {
            state.PowerKw = 0;
            state.EfficiencyPercent = 0;
        }

        // Update temperatures (rise with operation)
        if (state.Status == TurbineStatus.Running)
        {
            state.GearboxTemperature = Math.Min(80, 25 + state.RotorSpeedRpm * 2 + _random.NextDouble() * 5);
            state.GeneratorTemperature = Math.Min(90, 30 + state.RotorSpeedRpm * 2.5 + _random.NextDouble() * 5);
        }
        else
        {
            state.GearboxTemperature = Math.Max(20, state.GearboxTemperature - 0.5);
            state.GeneratorTemperature = Math.Max(20, state.GeneratorTemperature - 0.5);
        }

        // Update vibration level
        state.VibrationLevel = state.RotorSpeedRpm > 0 ? 0.5 + _random.NextDouble() * 1.5 : 0;

        // Randomly introduce errors
        if (_random.NextDouble() < 0.001) // 0.1% chance
        {
            state.Status = TurbineStatus.Error;
            _logger.LogWarning("Simulated error on turbine {TurbineId}", state.TurbineId);
        }
    }

    private class TurbineSimulationState
    {
        public Guid TurbineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public TurbineStatus Status { get; set; }
        public double WindSpeedMs { get; set; }
        public double WindDirectionDegrees { get; set; }
        public double RotorSpeedRpm { get; set; }
        public double PowerKw { get; set; }
        public double EfficiencyPercent { get; set; }
        public double GearboxTemperature { get; set; }
        public double GeneratorTemperature { get; set; }
        public double HydraulicPressure { get; set; }
        public double VibrationLevel { get; set; }
    }
}
