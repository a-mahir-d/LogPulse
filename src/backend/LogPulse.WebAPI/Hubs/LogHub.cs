using LogPulse.WebAPI.BackgroundServices;
using LogPulse.WebAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LogPulse.WebAPI.Hubs;

[Authorize]
public sealed class LogHub(LogSimulatorWorker simulator) : Hub<ILogClient>
{
    private readonly LogSimulatorWorker _simulator = simulator;
    private static int _activeConnections = 0;

    public override async Task OnConnectedAsync()
    {
        Interlocked.Increment(ref _activeConnections);
        Console.WriteLine($"[SignalR] Client connected. Total: {_activeConnections}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Interlocked.Decrement(ref _activeConnections);
        Console.WriteLine($"[SignalR] Client disconnected. Total: {_activeConnections}");

        if (_activeConnections <= 0)
        {
            _activeConnections = 0;
            _simulator.ChangeSpeed(SimulatorSpeed.Stopped);
            Console.WriteLine("[SignalR Auto-Stop] No active clients left. Simulator auto-stopped to save Neon/Render resources.");
        }

        await base.OnDisconnectedAsync(exception);
    }
}