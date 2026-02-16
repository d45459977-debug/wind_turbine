using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using WindTurbineMonitor.Shared.DTOs;

namespace WindTurbineMonitor.Server.Services;

/// <summary>
/// Service untuk komunikasi dengan SignalR Hub
/// </summary>
public class TurbineSignalRService
{
    private HubConnection? _hubConnection;
    private readonly ISnackbar _snackbar;
    private readonly ILogger<TurbineSignalRService> _logger;
    public event Action<TurbineReadingDto>? OnDataReceived;
    public event Action<List<TurbineReadingDto>>? OnAllDataReceived;
    public event Action<Guid, TurbineReadingDto>? OnTurbineUpdate;
    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action<Exception>? OnError;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public TurbineSignalRService(ISnackbar snackbar, ILogger<TurbineSignalRService> logger)
    {
        _snackbar = snackbar;
        _logger = logger;
    }

    public async Task StartAsync(string hubUrl)
    {
        if (_hubConnection != null)
        {
            _logger.LogWarning("Hub connection already exists");
            return;
        }

        _hubConnection = new HubConnectionBuilder()
            .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10) })
            .WithUrl(hubUrl)
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
            })
            .Build();

        // Register event handlers
        _hubConnection.On<TurbineReadingDto>("ReceiveTurbineData", (data) =>
        {
            OnDataReceived?.Invoke(data);
        });

        _hubConnection.On<List<TurbineReadingDto>>("ReceiveAllTurbinesData", (data) =>
        {
            OnAllDataReceived?.Invoke(data);
        });

        _hubConnection.On<Guid, TurbineReadingDto>("ReceiveTurbineUpdate", (turbineId, data) =>
        {
            OnTurbineUpdate?.Invoke(turbineId, data);
        });

        _hubConnection.Closed += async (error) =>
        {
            OnDisconnected?.Invoke();
            if (error != null)
            {
                _logger.LogError(error, "Hub connection closed");
                _snackbar.Add("Connection lost. Reconnecting...", Severity.Warning);
                OnError?.Invoke(error);
            }

            // Auto-reconnect is handled by WithAutomaticReconnect()
            await Task.Delay(5000);
        };

        _hubConnection.Reconnecting += async (error) =>
        {
            _logger.LogInformation("Reconnecting to hub...");
            _snackbar.Add("Reconnecting...", Severity.Info);
            await Task.CompletedTask;
        };

        _hubConnection.Reconnected += async (connectionId) =>
        {
            _logger.LogInformation("Reconnected to hub with connection ID: {ConnectionId}", connectionId);
            _snackbar.Add("Reconnected successfully", Severity.Success);
            OnConnected?.Invoke();
            await Task.CompletedTask;
        };

        try
        {
            await _hubConnection.StartAsync();
            _logger.LogInformation("Hub connection started");
            _snackbar.Add("Connected to real-time data stream", Severity.Success);
            OnConnected?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting hub connection");
            _snackbar.Add($"Failed to connect: {ex.Message}", Severity.Error);
            OnError?.Invoke(ex);
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
            _logger.LogInformation("Hub connection stopped");
        }
    }

    public async Task JoinTurbineGroupAsync(Guid turbineId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("JoinTurbineGroup", turbineId);
            _logger.LogInformation("Joined turbine group: {TurbineId}", turbineId);
        }
    }

    public async Task LeaveTurbineGroupAsync(Guid turbineId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.InvokeAsync("LeaveTurbineGroup", turbineId);
            _logger.LogInformation("Left turbine group: {TurbineId}", turbineId);
        }
    }
}
