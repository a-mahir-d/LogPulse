using LogPulse.WebAPI.Interfaces;
using LogPulse.WebAPI.Models;
using LogPulse.WebAPI.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LogPulse.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IOptions<DemoUserSettings> options, IJwtService jwtService) : ControllerBase
{
    private readonly DemoUserSettings _demoUserSettings = options.Value;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("INVALID_CREDENTIALS");
        }

        if (!_demoUserSettings.Email.Equals(request.Email) || BCrypt.Net.BCrypt.Verify(request.Password, _demoUserSettings.HashedPassword))
        {
            return BadRequest("INVALID_CREDENTIALS");
        }

        var token = jwtService.GenerateToken(_demoUserSettings.Email);
        return Ok(token);
    }
}
