using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Softplan.TaskManager.Dominio.Settings;

namespace Softplan.TaskManager.Services;

public class TokenJwtService : ITokenService
{
    private readonly IOptions<AppSettings> _appSettings;

    public TokenJwtService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }
    
    public (string Token, DateTime ExpiresAt) GenerateToken(string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, "User")
        };
        
        var creds = new SigningCredentials(new 
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JwtSecret)),
            SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddHours(1);
        
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: creds);
        
        var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenstring, expiration);
    }
}