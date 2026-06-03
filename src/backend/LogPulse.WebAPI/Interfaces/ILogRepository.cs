using LogPulse.WebAPI.Models;

namespace LogPulse.WebAPI.Interfaces;

public interface ILogRepository
{
    Task<bool> AddAsync(Log log);
    Task<IEnumerable<Log>> GetRecentLogsAsync(int count = 100);
    Task<Dictionary<string, int>> GetLogLevelCountsAsync();
}
