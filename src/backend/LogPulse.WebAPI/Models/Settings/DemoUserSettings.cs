using System.ComponentModel.DataAnnotations;

namespace LogPulse.WebAPI.Models.Settings;

public class DemoUserSettings
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string HashedPassword { get; set; }
}
