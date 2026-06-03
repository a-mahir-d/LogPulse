namespace LogPulse.WebAPI.Interfaces;

public interface IJwtService
{
    string GenerateToken(string email);
}
