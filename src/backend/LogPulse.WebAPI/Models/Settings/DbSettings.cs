using System.ComponentModel.DataAnnotations;

namespace LogPulse.WebAPI.Models.Settings;

public class DbSettings
{
    [Required]
    public required string ConnectionString { get; set; }
}
