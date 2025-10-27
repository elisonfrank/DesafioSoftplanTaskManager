using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Softplan.TaskManager.Dominio.Dto;

namespace Softplan.IntegrationTests.Endpoints;

public class UserEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public UserEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task PostUsers_DeveRetornar201_QuandoUsuarioValido()
    {
        //Arrange
        var newUser = new NewUserDto("teste@email.com", "123456");

        //Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", newUser);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<UserDto>();
        result.Should().NotBeNull();
        result!.Email.Should().Be(newUser.Email);
    }
    
    [Fact]
    public async Task PostUsers_DeveRetornar400_QuandoUsuarioInvalido()
    {
        //Arrange
        var newUser = new NewUserDto("", "");

        //Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", newUser);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task PostLogin_DeveRetornar200_QuandoCredenciaisValidas()
    {
        // Arrange: cria usuário antes
        var newUser = new NewUserDto("login@email.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/users", newUser);

        // Act: tenta login
        var login = new LoginDto(newUser.Email, newUser.Password);
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", login);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResultDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task PostLogin_DeveRetornar401_QuandoSenhaInvalida()
    {
        // Arrange: cria usuário antes
        var newUser = new NewUserDto("login2@email.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/users", newUser);

        // Act: tenta login com senha errada
        var login = new LoginDto(newUser.Email, "senhaErrada");
        var response = await _client.PostAsJsonAsync("/api/v1/users/login", login);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}