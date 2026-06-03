using LogPulse.WebAPI.BackgroundServices;
using LogPulse.WebAPI.Interfaces;
using LogPulse.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogPulse.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class LogsController(ILogRepository logRepository, LogSimulatorWorker simulator) : ControllerBase
{
    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<Log>>> GetRecentLogs([FromQuery] int count = 100)
    {
        if (count <= 0 || count > 500)
        {
            count = 100;
        }

        var logs = await logRepository.GetRecentLogsAsync(count);
        return Ok(logs);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<Dictionary<string, int>>> GetLogStats()
    {
        var stats = await logRepository.GetLogLevelCountsAsync();
        return Ok(stats);
    }

    [HttpPost("simulator/speed")]
    public IActionResult SetSimulatorSpeed([FromQuery] SimulatorSpeed speed)
    {
        bool isChanged = simulator.ChangeSpeed(speed);

        return Ok(new
        {
            Message = isChanged ? $"Simulator speed updated to {speed}" : "Simulator is already at this speed.",
            CurrentSpeed = simulator.GetCurrentSpeed().ToString()
        });
    }

    [HttpGet("simulator/status")]
    public IActionResult GetSimulatorStatus()
    {
        return Ok(new { CurrentSpeed = simulator.GetCurrentSpeed().ToString() });
    }
}
