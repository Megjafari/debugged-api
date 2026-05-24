namespace Debugged.Infrastructure.Identity;

// Strongly-typed binding for the "Jwt" section in appsettings.json.
// Registered via builder.Services.Configure<JwtSettings>(config.GetSection("Jwt")).
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}