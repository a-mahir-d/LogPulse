using LogPulse.WebAPI.Hubs;
using LogPulse.WebAPI.Interfaces;
using LogPulse.WebAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace LogPulse.WebAPI.BackgroundServices;

public enum SimulatorSpeed
{
    Stopped = 0,
    Slow = 2,
    Medium = 5,
    Fast = 10
}

public sealed class LogSimulatorWorker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private SimulatorSpeed _currentSpeed = SimulatorSpeed.Stopped;
    private readonly Lock _speedLock = new();

    private readonly string[] _mockMessages = [
        "User logged in successfully.",
        "Database connection pool warning - high load detected.",
        "Failed to process payment transition.",
        "Cache invalidated for user session.",
        "Critical error: Out of memory exception simulated.",
        "API Gateway timed out while reaching AuthService."
    ];

    private readonly string[] _mockServices = ["AuthService", "PaymentAPI", "Gateway", "ProductCatalog"];

    public bool ChangeSpeed(SimulatorSpeed newSpeed)
    {
        lock (_speedLock)
        {
            if (_currentSpeed == newSpeed) return false;

            _currentSpeed = newSpeed;
            Console.WriteLine($"[Simulator] Speed changed to: {_currentSpeed} ({(int)_currentSpeed}x)");
            return true;
        }
    }

    public SimulatorSpeed GetCurrentSpeed() => _currentSpeed;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[Simulator Worker] Background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            SimulatorSpeed activeSpeed;
            lock (_speedLock)
            {
                activeSpeed = _currentSpeed;
            }

            if (activeSpeed == SimulatorSpeed.Stopped)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            await GenerateAndBroadcastLogAsync();

            int delayMilliseconds = 1000 / (int)activeSpeed;
            await Task.Delay(delayMilliseconds, stoppingToken);
        }
    }

    private async Task GenerateAndBroadcastLogAsync()
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<LogHub, ILogClient>>();

        var random = Random.Shared;
        int levelValue = random.Next(1, 5);

        var mockLog = new Log
        {
            Level = (Models.LogLevel)levelValue,
            Message = _mockMessages[random.Next(_mockMessages.Length)],
            SourceService = _mockServices[random.Next(_mockServices.Length)],
            Timestamp = DateTime.UtcNow
        };

        try
        {
            await logRepository.AddAsync(mockLog);
            await hubContext.Clients.All.ReceiveLog(mockLog);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Simulator Error] Failed to process log: {ex.Message}");
        }
    }
}
