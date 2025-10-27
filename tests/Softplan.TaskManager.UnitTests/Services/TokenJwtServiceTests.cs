using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Softplan.TaskManager.Dominio.Settings;
using Softplan.TaskManager.Services;

namespace Softplan.TaskManager.UnitTests.Services;

public class TokenJwtServiceTests
{
    private TokenJwtService CreateService(string secret = "minha-super-chave-secreta-com-mais-de-32-caracteres!")
    {
        var options = Options.Create(new AppSettings { JwtSecret = secret });
        return new TokenJwtService(options);
    }
    
    [Fact]
    public void GenerateToken_DeveGerarTokenValido_ComClaimsEsperados()
    {
        // Arrange
        var service = CreateService();
        var email = "teste@email.com";

        // Act
        var (tokenString, expiresAt) = service.GenerateToken(email);

        // Assert
        tokenString.Should().NotBeNullOrEmpty();
        expiresAt.Should().BeAfter(DateTime.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenString);

        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == email);
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
    }
    
    [Fact]
    public void GenerateToken_DeveExpirarEm1Hora()
    {
        //Arrange
        var service = CreateService();
        
        //Act
        var (_, expiresAt) = service.GenerateToken("user@email.com");
        var diff = expiresAt - DateTime.UtcNow;
        
        //Assert
        diff.TotalMinutes.Should().BeGreaterThan(59).And.BeLessThanOrEqualTo(60);
    }
}