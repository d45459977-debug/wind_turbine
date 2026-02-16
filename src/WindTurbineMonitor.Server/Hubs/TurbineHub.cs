using Microsoft.AspNetCore.SignalR;

namespace WindTurbineMonitor.Server.Hubs;

/// <summary>
/// SignalR Hub untuk komunikasi real-time dengan clients
/// </summary>
public class TurbineHub : Hub
{
    private readonly ILogger<TurbineHub> _logger;

    public TurbineHub(ILogger<TurbineHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Client bergabung ke group untuk mendapatkan update spesifik turbin
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    public async Task JoinTurbineGroup(Guid turbineId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Turbine_{turbineId}");
        _logger.LogInformation("Connection {ConnectionId} joined turbine group {TurbineId}",
            Context.ConnectionId, turbineId);
    }

    /// <summary>
    /// Client keluar dari group turbin
    /// </summary>
    /// <param name="turbineId">ID turbin</param>
    public async Task LeaveTurbineGroup(Guid turbineId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Turbine_{turbineId}");
        _logger.LogInformation("Connection {ConnectionId} left turbine group {TurbineId}",
            Context.ConnectionId, turbineId);
    }

    /// <summary>
    /// Dipanggil ketika client terhubung
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Dipanggil ketika client terputus
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogWarning("Client {ConnectionId} disconnected: {Error}",
                Context.ConnectionId, exception.Message);
        }
        else
        {
            _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
