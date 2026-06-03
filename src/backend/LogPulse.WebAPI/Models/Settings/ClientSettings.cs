using System.ComponentModel.DataAnnotations;

namespace LogPulse.WebAPI.Models.Settings;

public class ClientSettings
{
    [Required]
    public required string BaseUrl { get; set; }
}

