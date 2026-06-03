using LogPulse.WebAPI.Models.Settings;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace LogPulse.WebAPI.Context;

public sealed class DapperContext(IOptions<DbSettings> options)
{
    private readonly DbSettings _settings = options.Value;

    public IDbConnection CreateConnection() => new NpgsqlConnection(_settings.ConnectionString);
}
