using Dapper;
using LogPulse.WebAPI.Context;
using LogPulse.WebAPI.Interfaces;
using LogPulse.WebAPI.Models;
using System.Data;
using MyLogLevel = LogPulse.WebAPI.Models.LogLevel;

namespace LogPulse.WebAPI.Repositories;

public sealed class LogRepository(DapperContext context) : ILogRepository
{
    private readonly DapperContext _context = context;

    public async Task<bool> AddAsync(Log log)
    {
        const string query = @"
            INSERT INTO Logs (Level, Message, SourceService, Timestamp)
            VALUES (@Level, @Message, @SourceService, @Timestamp);";

        using IDbConnection connection = _context.CreateConnection();

        int affectedRows = await connection.ExecuteAsync(query, log);

        return affectedRows > 0;
    }

    public async Task<IEnumerable<Log>> GetRecentLogsAsync(int count = 100)
    {
        const string query = "SELECT * FROM Logs ORDER BY Timestamp DESC LIMIT @Count;";
        using IDbConnection connection = _context.CreateConnection();
        return await connection.QueryAsync<Log>(query, new { Count = count });
    }

    public async Task<Dictionary<string, int>> GetLogLevelCountsAsync()
    {
        const string query = @"SELECT Level, COUNT(*) as Count FROM Logs GROUP BY Level;";

        using IDbConnection connection = _context.CreateConnection();

        var result = await connection.QueryAsync<(int Level, int Count)>(query);

        return result.ToDictionary(
            x => ((MyLogLevel)x.Level).ToString(),
            x => x.Count
        );
    }
}
