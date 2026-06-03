using LogPulse.WebAPI.Models;

namespace LogPulse.WebAPI.Interfaces;

public interface ILogClient
{
    Task ReceiveLog(Log log);
}
